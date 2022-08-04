using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goaltender : MonoBehaviour
{
    private GameSystem gameSystem;
    private AudioManager audioManager;
    private TeamMember teamMember;
    [Header("Movement")]
    [HideInInspector] public float movementSpeed = 6f;
    private Quaternion goalieRotation;
    const float goaltenderDefaultYRotation = -90;
    private float goaltenderForwardYRotation;
    private float netToGoalieYRotation;
    private Rect homeGoalCrease = new Rect(-15.95f,-1.75f, 2.1f, 4.52f);
    private Rect awayGoalCrease = new Rect(14.02f,-1.75f,2.1f, 4.52f);
    private Rect myMovementCrease;
    [HideInInspector] public GameObject myNet;
    [HideInInspector] public GameObject myOriginPoint;
    private Vector3 displacementVector;
    [Header("Shooting")]
    [SerializeField] [Range(0.5f, 6f)] float shotPowerWindUpRate; // extraPower / second
    [SerializeField] [Range(8f, 20f)] float shotPowerMax;
    [SerializeField] [Range(0.0f, 10f)] float shotPower;
    private float extraPower;
    private Vector3 puckLaunchDirection;
    private Rigidbody goaltenderRigidBody;
    private void Awake(){
        gameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        teamMember = gameObject.GetComponent<TeamMember>();
        goaltenderRigidBody = GetComponent<Rigidbody>();
    }
    public void FindMyNet(){
        if(teamMember.isHomeTeam){
            myNet = gameSystem.homeNet;
            myMovementCrease = homeGoalCrease;
            goaltenderForwardYRotation = -90;
            myOriginPoint = gameSystem.homeGoalOrigin.gameObject;
        } else{
            myNet = gameSystem.awayNet;
            myMovementCrease = awayGoalCrease;
            goaltenderForwardYRotation = -90;
            myOriginPoint = gameSystem.awayGoalOrigin.gameObject;
        }
        transform.position = myOriginPoint.transform.position;
    }
    public void SetPointers(Vector3 movementPointer){
        displacementVector = movementSpeed * movementPointer * Time.deltaTime;
    }
    private void HandleMove(){
        if(myNet){
            transform.position = new Vector3(
                Mathf.Clamp((transform.position.x + displacementVector.x), myMovementCrease.x, (myMovementCrease.x + myMovementCrease.width)),
                transform.position.y,
                Mathf.Clamp((transform.position.z + displacementVector.z), myMovementCrease.y, (myMovementCrease.y + myMovementCrease.height))
            );
            goalieRotation = Quaternion.LookRotation(transform.position - myNet.transform.position);
            goalieRotation.x = 0;
            goalieRotation.z = 0;
            goalieRotation *= Quaternion.Euler(0, -90, 0);
            transform.rotation = Quaternion.RotateTowards(
                myOriginPoint.transform.rotation,
                goalieRotation,
                35
            );
        }
    }
    public void SetShotDirection(Vector2 movementInput){
        if(movementInput.magnitude == 0){puckLaunchDirection = transform.forward;}
        else{puckLaunchDirection = new Vector3(movementInput.x, 0.25f, movementInput.y);}
    }
    public IEnumerator WindUpShot(){
        extraPower = 0f;
        while(teamMember.windingUp){
            yield return new WaitForSeconds((Time.deltaTime));
            if(shotPower + extraPower < shotPowerMax){extraPower += (shotPowerWindUpRate * Time.deltaTime);}
        }
    }
    public void ShootPuck(){
        teamMember.windingUp = false;
        if(teamMember.hasPosession){
            teamMember.BreakPosession();
            audioManager.PlayShotSFX();
            gameSystem.puckObject.GetComponent<Rigidbody>().AddForce(puckLaunchDirection * (shotPower + extraPower), ForceMode.Impulse);
        }
    }
    private void Update(){
        HandleMove();
    }
}
