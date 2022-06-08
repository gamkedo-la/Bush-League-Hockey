using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
public class Skater : MonoBehaviour
{
    private GameSystem gameSystem;
    [Header("Skating")]
    private Rigidbody skaterRigidBody;
    [SerializeField] float skaterAcceleration;
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
    private bool canTakePosession = true, hasPosession = false;
    private float posessionCooldownTime = 0.75f;
    [Header("Shooting / Passing")]
    [SerializeField] float shotPowerWindUpRate; // power / second
    [SerializeField] float shotPowerMax;
    private float shotPower = 0;
    private Vector3 puckLaunchDirection;
    [HideInInspector] public bool windingUp = false;
    private void Awake(){
        skaterRigidBody = GetComponent<Rigidbody>();
        gameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
    }
    public void ControlPuck(){
        if(canTakePosession){
            canTakePosession = false;
            hasPosession = true;
            gameSystem.puckObject.transform.position = puckPositionMarker.transform.position;
            if(!puckPositionMarker.GetComponent<FixedJoint>()){
                puckPositionMarker.GetComponent<PuckHandleJoint>().AttachPuckToHandleJoint(gameSystem.puckRigidBody);
            }
        }
    }
    public IEnumerator LostPosession(){
        hasPosession = false;
        yield return new WaitForSeconds(posessionCooldownTime);
        canTakePosession = true;
    }
    public void SetShotDirection(Vector2 movementInput){
        puckLaunchDirection = new Vector3(movementInput.x, 0.25f, movementInput.y);
    }
    public IEnumerator WindUpShot(){
        while(windingUp){
            yield return new WaitForSeconds((0.25f));
            if(shotPower < shotPowerMax){shotPower += (shotPowerWindUpRate * 0.25f);}
            Debug.Log("Wind Up - " + shotPower);
        }
    }
    public void ShootPuck(){
        windingUp = false;
        if(hasPosession){
            puckPositionMarker.GetComponent<PuckHandleJoint>().BreakFixedJoint();
            gameSystem.puckObject.GetComponent<Rigidbody>().AddForce(puckLaunchDirection * shotPower, ForceMode.Impulse);
        }
        shotPower = 0;
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
        if(skaterRigidBody.angularVelocity.magnitude > 0){skaterRigidBody.angularVelocity = Vector3.zero;}
        if(movementPointer.magnitude > 0.1f){
            skaterRigidBody.AddForce(movementPointer * skaterAcceleration);
        }
        if(skaterRigidBody.velocity.magnitude > 0.1f){
            desiredRotation = Quaternion.LookRotation(skaterRigidBody.velocity, Vector3.up);
            rotationThisFrame = Quaternion.Lerp(transform.rotation, desiredRotation, skaterTurnSpeed);
            if(rotationThisFrame.eulerAngles.magnitude > .1f){
                transform.rotation = Quaternion.Euler(0f, rotationThisFrame.eulerAngles.y, 0f);
            }
        }
    }
    private void Update(){
        HandleMove();
    }
}
