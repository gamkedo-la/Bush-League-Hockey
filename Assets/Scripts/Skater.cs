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
    private float posessionCooldownTime = 1.5F;
    [Header("Shooting / Passing")]
    private float shotPower;
    private Vector3 puckLaunchDirection;
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
                ObjectFactory.AddComponent<FixedJoint>(puckPositionMarker);
                puckHandleJoint = puckPositionMarker.GetComponent<FixedJoint>();
                puckHandleJoint.connectedBody = gameSystem.puckRigidBody;
                puckHandleJoint.breakForce = 2500f;
            }
        }
    }
    public IEnumerator LostPosession(){
        hasPosession = false;
        yield return new WaitForSeconds(posessionCooldownTime);
        canTakePosession = true;
    }
    public void SetShotDirection(Vector2 movementInput){
        puckLaunchDirection = new Vector3(movementInput.x, 0.15f, movementInput.y);
    }
    public void ShootPuck(){
        if(hasPosession){
            Destroy(puckHandleJoint);
            shotPower = 7.5f;
            gameSystem.puckObject.GetComponent<Rigidbody>().AddForce(puckLaunchDirection * shotPower, ForceMode.Impulse);
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
            skaterRigidBody.AddForce(movementPointer * skaterAcceleration);
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
