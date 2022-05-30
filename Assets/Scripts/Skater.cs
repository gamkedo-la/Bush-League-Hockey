using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
public class Skater : MonoBehaviour
{
    private GameSystem gameSystem;
    [Header("Skating Control")]
    private Rigidbody skaterRigidBody;
    [SerializeField] float skaterSpeed;
    [SerializeField] float skaterMaximumSpeed;
    [SerializeField] float skaterTurnSpeed;
    private Vector3 movementPointer;
    private Vector3 forwardForce;
    private Vector3 sideForce;
    private Quaternion desiredRotation;
    private Quaternion rotationThisFrame;
    [Header("Puck Control")]
    [SerializeField] Collider skaterPosessionTrigger;
    [SerializeField] GameObject puckPositionMarker;
    private FixedJoint puckHandleJoint;
    private bool isFirstTouch = true;
    private float stickHandleFrequency = 0.5f; // m/s * Vector3.Normalize(target.transform.position - current.transform.position)
    private float stickHandleForce = 3f;
    private float posessionCooldownTime = 1.5F;
    private Vector3 puckToMarkerPointer;
    private Vector3 puckToMarkerForce;
    [Header("Shooting / Passing")]
    private float shotPower;
    private Vector3 puckLaunchDirection;
    private void Awake(){
        skaterRigidBody = GetComponent<Rigidbody>();
        gameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
    }
    public void ControlPuck(){
        if(isFirstTouch){
            isFirstTouch = false;
            gameSystem.puckObject.transform.position = puckPositionMarker.transform.position;
            if(!puckPositionMarker.GetComponent<FixedJoint>()){
                ObjectFactory.AddComponent<FixedJoint>(puckPositionMarker);
                puckHandleJoint = puckPositionMarker.GetComponent<FixedJoint>();
                puckHandleJoint.connectedBody = gameSystem.puckRigidBody;
                puckHandleJoint.breakForce = 2000f;
            }
        }
    }
    public IEnumerator CooldownFirstTouch(){
        yield return new WaitForSeconds(posessionCooldownTime);
        isFirstTouch = true;
    }
    public void ShotInput(InputAction.CallbackContext ctx){
        // player has posession?
        if(!isFirstTouch){
            if(ctx.performed){
                // release puck, current shot meter, current movementPointer
                // remove fixed joint
                Destroy(puckHandleJoint);
                shotPower = 10f;
                gameSystem.puckObject.GetComponent<Rigidbody>().AddForce(puckLaunchDirection * shotPower, ForceMode.Impulse);
            }
            if(ctx.started){
                Debug.Log("Winding up shot");
            }
        } else {
            // one timer wind up
            // cannot affect puck unless it's in a "strike-zone" 
        }
    }
    public void MoveInput(Vector2 inputVector){
        forwardForce = inputVector.y * Vector3.Normalize(transform.position - gameSystem.mainCamera.transform.position);
        sideForce = inputVector.x * Vector3.Cross(gameSystem.mainCamera.transform.forward, -gameSystem.mainCamera.transform.up);
        movementPointer = Vector3.Normalize(new Vector3((forwardForce.x + sideForce.x), 0f, (forwardForce.z + sideForce.z)));
        puckLaunchDirection = new Vector3(inputVector.x, 0.2f, inputVector.y);
    }
    public void HandleMove(){
        // Find angle between forward and movementPointer
        // direction and acceleration are a function of y axis rotation
        // +-90: can change direction not accelerate
        // +-90 to +-155: backward component of force is amplified, side force as usual
        // +-155 to +-180: 
        if(movementPointer.magnitude > 0.1f){
            skaterRigidBody.AddForce(movementPointer * skaterSpeed);
        }
        if(skaterRigidBody.velocity.magnitude > 0.1f){
            desiredRotation = Quaternion.LookRotation(skaterRigidBody.velocity, Vector3.up);
            rotationThisFrame = Quaternion.Lerp(transform.rotation, desiredRotation, skaterTurnSpeed);
            if(rotationThisFrame.eulerAngles.magnitude > .1f){
                transform.rotation = Quaternion.Euler(0f, rotationThisFrame.eulerAngles.y, 0f);
                skaterRigidBody.angularVelocity = Vector3.zero;
            }
        }
    }
    private void Update(){
        HandleMove();
    }
}
