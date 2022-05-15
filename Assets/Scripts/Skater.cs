using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skater : MonoBehaviour
{
    [SerializeField] GameObject puckTarget;
    [SerializeField] Collider skaterPosessionTrigger;
    private Rigidbody skaterRigidBody;
    [Header("Skating Control")]
    [SerializeField] float skaterSpeed;
    [SerializeField] float skaterTurnSpeed;
    private Vector3 movementPointer;
    private Vector3 forwardForce;
    private Vector3 sideForce;
    private Quaternion desiredRotation;
    private Quaternion rotationThisFrame;
    private float stickHandleSpeed = 1.5f; // m/s * Vector3.Normalize(target.transform.position - current.transform.position)
    [Header("Main Camera")]
    private Camera mainCamera;
    private void Awake(){
        skaterRigidBody = GetComponent<Rigidbody>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }
    public void MoveInput(Vector2 inputVector){
        forwardForce = inputVector.y * Vector3.Normalize(transform.position - mainCamera.transform.position);
        sideForce = inputVector.x * Vector3.Cross(mainCamera.transform.forward, -mainCamera.transform.up);
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
