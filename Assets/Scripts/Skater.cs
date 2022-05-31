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
    private Vector3 cameraForward;
    private Vector3 cameraRight;
    private Quaternion desiredRotation;
    private Quaternion rotationThisFrame;
    [Header("Puck Control")]
    [SerializeField] Collider skaterPosessionTrigger;
    [SerializeField] GameObject puckPositionMarker;
    private FixedJoint puckHandleJoint;
    private bool isFirstTouch = true;
    private float posessionCooldownTime = 1.5F;
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
                puckHandleJoint.breakForce = 2500f;
            }
        }
    }
    public IEnumerator CooldownFirstTouch(){
        yield return new WaitForSeconds(posessionCooldownTime);
        isFirstTouch = true;
    }
    public void SetShotDirection(Vector2 movementInput){
        puckLaunchDirection = new Vector3(movementInput.x, 0.15f, movementInput.y);
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
    public void MoveSkater(Vector3 pointer){
        movementPointer = pointer;
    }
    public void HandleMove(){
        // Find angle between forward and movementPointer
        // direction and acceleration are a function of y axis rotation
        // +-90: can change direction with constant speed
        // +-90 to +-155: Carving, decelerate hard along forward axis 
        // +-155 to +-180: Hard stop, quickly decelerate to 0
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
