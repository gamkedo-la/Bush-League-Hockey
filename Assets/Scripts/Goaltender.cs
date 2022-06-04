using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goaltender : MonoBehaviour
{
    private GameSystem gameSystem;
    [HideInInspector] public float movementSpeed = 4f, yRotation;
    [HideInInspector] private Rect homeGoalCrease = new Rect(-15.95f,-2.62f,2.15f,4.52f);
    [HideInInspector] private Rect awayGoalCrease = new Rect(14.02f,-2.62f,2.15f,4.52f);
    private Rect myMovementCrease;
    [HideInInspector] public GameObject myNet;
    private Vector3 displacementVector;
    private Vector3 goalieDirectionPointer;
    private Quaternion goalieDirectionRotation;
    private void Awake(){
        gameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
    }
    public void FindMyNet(){
        if(gameObject.GetComponent<TeamMember>().isHomeTeam){
            myNet = GameObject.FindWithTag("homeNet");
            myMovementCrease = homeGoalCrease;
            transform.position = gameSystem.homeGoalOrigin.position;
        } else{
            myNet = GameObject.FindWithTag("awayNet");
            myMovementCrease = awayGoalCrease;
            transform.position = gameSystem.awayGoalOrigin.position;
        }
        
    }
    public void MoveGoalie(Vector3 movementPointer){
        displacementVector = movementSpeed * movementPointer * Time.deltaTime;
    }
    private void HandleMove(){
        if(myNet){
            transform.position = new Vector3(
                Mathf.Clamp((transform.position.x + displacementVector.x), myMovementCrease.x, (myMovementCrease.x + myMovementCrease.width)),
                transform.position.y,
                Mathf.Clamp((transform.position.z + displacementVector.z), myMovementCrease.y, (myMovementCrease.y + myMovementCrease.height))
            );
            goalieDirectionPointer = Vector3.Normalize(transform.position - myNet.transform.position);
            goalieDirectionRotation = Quaternion.Euler(0, Quaternion.LookRotation(goalieDirectionPointer, Vector3.up).eulerAngles.y, 0);
            transform.rotation = Quaternion.Lerp(goalieDirectionRotation, Quaternion.Euler(0, 270, 0), 0.5f);
        }
    }
    private void Update(){
        HandleMove();
    }
}
