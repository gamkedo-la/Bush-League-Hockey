using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skater : MonoBehaviour
{
    private GameSystem gameSystem;
    [Header("Skating Control")]
    private Rigidbody skaterRigidBody;
    [SerializeField] float skaterSpeed;
    [SerializeField] float skaterTurnSpeed;
    private Vector3 movementPointer;
    private Vector3 forwardForce;
    private Vector3 sideForce;
    private Quaternion desiredRotation;
    private Quaternion rotationThisFrame;
    [Header("Puck Control")]
    [SerializeField] Collider skaterPosessionTrigger;
    [SerializeField] GameObject puckPositionMarker;
    private bool isFirstTouch = true;
    private float stickHandleFrequency = 0.5f; // m/s * Vector3.Normalize(target.transform.position - current.transform.position)
    private Vector3 puckToMarkerPointer;
    private void Awake(){
        skaterRigidBody = GetComponent<Rigidbody>();
        gameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
    }
    private IEnumerator StickHandlePuck(){
        bool forceToggle = true;
        while(!isFirstTouch){
            puckToMarkerPointer = Vector3.Normalize(gameSystem.puckObject.transform.position - puckPositionMarker.transform.position);
            if(forceToggle){
                // add force on puck towards positionmarker
                gameSystem.puckRigidBody.AddForce(puckToMarkerPointer, ForceMode.VelocityChange);
            } else{
                // normalize for on puck to match player
                // gameSystem.puckRigidBody.velocity compare 
            }
            yield return new WaitForSeconds(stickHandleFrequency);
            forceToggle = !forceToggle;
        }
    }
    public void ControlPuck(){
        if(isFirstTouch){
            gameSystem.puckRigidBody.AddForce(skaterRigidBody.velocity, ForceMode.VelocityChange);
            isFirstTouch = false;
        }
    }
    public void ResetFirstTouch(){
        isFirstTouch = true;
    }
    public void MoveInput(Vector2 inputVector){
        forwardForce = inputVector.y * Vector3.Normalize(transform.position - gameSystem.mainCamera.transform.position);
        sideForce = inputVector.x * Vector3.Cross(gameSystem.mainCamera.transform.forward, -gameSystem.mainCamera.transform.up);
        movementPointer = Vector3.Normalize(new Vector3((forwardForce.x + sideForce.x), 0f, (forwardForce.z + sideForce.z)));
    }
    public void HandleMove(){
        // Acceleration goes down as velocity increases
        // player can stop quickly if the intended direction more than 120deg either side
        if(movementPointer.magnitude > 0){ skaterRigidBody.AddForce(movementPointer * skaterSpeed); }
        if(skaterRigidBody.velocity.magnitude > 0.1f){
            desiredRotation = Quaternion.LookRotation(skaterRigidBody.velocity, Vector3.up);
            rotationThisFrame = Quaternion.Lerp(transform.rotation, desiredRotation, skaterTurnSpeed);
            if(rotationThisFrame.eulerAngles.magnitude > 1){
                transform.rotation = Quaternion.Euler(0f, rotationThisFrame.eulerAngles.y, 0f);
                skaterRigidBody.angularVelocity = Vector3.zero;
            }
        }
    }
    private void Update() {
        HandleMove();
    }
}
