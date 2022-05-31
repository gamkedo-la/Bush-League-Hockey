using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goaltender : MonoBehaviour
{
    private float movementSpeed = 4f,
        minXPosition = -15.8f,
        maxXPosition = -14.072f,
        minZPosition = -1.8f,
        maxZPosition = 1.75f,
        yRotation;
    [SerializeField] GameObject myNet;
    private Vector3 displacementVector;
    private Vector3 goalieDirectionPointer;
    private Quaternion goalieDirectionRotation;
    public void MoveGoalie(Vector3 movementPointer){
        displacementVector = movementPointer * movementSpeed * Time.deltaTime;
    }
    private void HandleMove(){
        transform.position = new Vector3 (
            Mathf.Clamp((transform.position.x + displacementVector.x), minXPosition, maxXPosition),
            transform.position.y,
            Mathf.Clamp((transform.position.z + displacementVector.z), minZPosition, maxZPosition)
        );
    }
    private void HandleFaceDirection(){
        goalieDirectionPointer = Vector3.Normalize(transform.position - myNet.transform.position);
        goalieDirectionRotation = Quaternion.Euler(0, Quaternion.LookRotation(goalieDirectionPointer, Vector3.up).eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(goalieDirectionRotation, Quaternion.Euler(0, 270, 0), 0.5f);
    }
    private void Update(){
        HandleMove();
        HandleFaceDirection();
    }
}
