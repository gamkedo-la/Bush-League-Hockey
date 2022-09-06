using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goaltender : MonoBehaviour
{
    private GameSystem gameSystem;
    private AudioManager audioManager;
    public TeamMember teamMember;
    [Header("Movement")]
    [HideInInspector] public float movementSpeed = 6f;
    private Quaternion goalieRotation;
    private float goaltenderForwardYRotation;
    private float netToGoalieYRotation;
    private Rect homeGoalCrease = new Rect(-15.95f,-1.75f, 2.1f, 4.52f);
    private Rect awayGoalCrease = new Rect(14.02f,-1.75f,2.1f, 4.52f);
    private Rect myMovementCrease;
    [HideInInspector] public GameObject myNet;
    [HideInInspector] public GameObject myOriginPoint;
    private Vector3 displacementVector;
    private Rigidbody goaltenderRigidBody;
    [Header("Shooting")]
    [SerializeField] [Range(0.5f, 6f)] float shotPowerWindUpRate; // extraPower / second
    [SerializeField] [Range(8f, 20f)] float shotPowerMax;
    [SerializeField] [Range(1f, 5f)] float shotPower;
    private Vector3 shotDirection;
    private float extraPower;
    private bool windingUpShot;
    [Header("Passing")]
    [SerializeField] [Range(0.5f, 6f)] float passPowerWindUpRate; // extraPassPower / second
    [SerializeField] [Range(6f, 12f)] float passPowerMax;
    [SerializeField] [Range(1f, 8f)] float passPower;
    private Vector3 passDirection;
    private float extraPassPower;
    private bool windingUpPass;
    [Header("Bodycheck")]
    [SerializeField] [Range(1f, 8f)] float goalieHitPower;

    private void Awake(){
        gameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        teamMember = gameObject.GetComponent<TeamMember>();
        goaltenderRigidBody = GetComponent<Rigidbody>();
    }
    public void FindMyNet(){
        Awake();
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
        if(movementPointer.magnitude == 0){
            shotDirection = transform.forward;
            passDirection = transform.forward;
        }
        else{
            shotDirection = new Vector3(movementPointer.x, 0.5f, movementPointer.z);
            passDirection = new Vector3(movementPointer.x, 0, movementPointer.z);
        }
    }
    private void HandleMove(){
        if(myNet && !WindingUp()){
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
    public bool WindingUp(){
        return windingUpShot || windingUpPass;
    }
    public IEnumerator WindUpPass(){
        // blocked when: already winding up
        if(WindingUp()) yield break;
        windingUpPass = true;
        extraPassPower = 0f;
        while(WindingUp()){
            yield return new WaitForSeconds((Time.deltaTime));
            if(passPower + extraPassPower < passPowerMax){extraPassPower += (passPowerWindUpRate * Time.deltaTime);}
        }
    }
    public void PassPuck(){
        // blocked when: no wind up
        if(!windingUpPass) return;
        windingUpPass = false;
        if(teamMember.hasPosession){
            StartCoroutine(teamMember.BreakPosession());
            StartCoroutine(audioManager.PlayPassSFX());
            gameSystem.puckObject.GetComponent<Rigidbody>().AddForce(passDirection * (passPower + extraPassPower), ForceMode.Impulse);
        }
    }
    public IEnumerator WindUpShot(){
        // blocked when: already winding up
        if(teamMember.windingUp) yield break;
        extraPower = 0f;
        teamMember.windingUp = true;
        while(teamMember.windingUp){
            yield return new WaitForSeconds((Time.deltaTime));
            if(shotPower + extraPower < shotPowerMax){extraPower += (shotPowerWindUpRate * Time.deltaTime);}
        }
    }
    public void ShootPuck(){
        // blocked when: no wind up
        if(!teamMember.windingUp) return;
        teamMember.windingUp = false;
        if(teamMember.hasPosession){
            StartCoroutine(teamMember.BreakPosession());
            audioManager.PlayShotSFX();
            gameSystem.puckObject.GetComponent<Rigidbody>().AddForce(shotDirection * (shotPower + extraPower), ForceMode.Impulse);
        }
    }
    private void OnTriggerEnter(Collider other){
        if(other.gameObject.name.Contains("Skater") && other.gameObject.GetComponent<TeamMember>()?.isHomeTeam != teamMember.isHomeTeam){
            other.gameObject.GetComponent<Skater>().ReceiveBodyCheck(
                goalieHitPower,
                (other.transform.position - transform.position).normalized + Vector3.up*2f
            );
        }
    }
    private void Update(){
        HandleMove();
    }
}
