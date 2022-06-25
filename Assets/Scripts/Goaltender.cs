using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goaltender : MonoBehaviour
{
    private GameSystem gameSystem;
    private TeamMember teamMember;
    [Header("Movement")]
    [HideInInspector] public float movementSpeed = 6f;
    private Quaternion goalieRotation;
    const float goaltenderDefaultYRotation = -90;
    private float goaltenderForwardYRotation;
    private float netToGoalieYRotation;
    [HideInInspector] private Rect homeGoalCrease = new Rect(-15.95f,-2.62f,2.15f,4.52f);
    [HideInInspector] private Rect awayGoalCrease = new Rect(14.02f,-2.62f,2.15f,4.52f);
    private Rect myMovementCrease;
    [HideInInspector] public GameObject myNet;
    private Vector3 displacementVector;
    [Header("Shooting / Passing")]
    [SerializeField] float shotPowerWindUpRate; // power / second
    [SerializeField] float shotPowerMax;
    private float shotPower = 6f;
    private Vector3 puckLaunchDirection;
    private Rigidbody goaltenderRigidBody;
    private void Awake(){
        gameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        teamMember = gameObject.GetComponent<TeamMember>();
        goaltenderRigidBody = GetComponent<Rigidbody>();
    }
    public void FindMyNet(){
        if(teamMember.isHomeTeam){
            myNet = GameObject.FindWithTag("homeNet");
            myMovementCrease = homeGoalCrease;
            goaltenderForwardYRotation = 0;
            transform.position = gameSystem.homeGoalOrigin.position;
        } else{
            myNet = GameObject.FindWithTag("awayNet");
            myMovementCrease = awayGoalCrease;
            goaltenderForwardYRotation = 180;
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
            netToGoalieYRotation = Quaternion.LookRotation((transform.position - myNet.transform.position), Vector3.up).eulerAngles.y;
            // average between that and forward
            // goalieRotation = (netToGoalieYRotation + goaltenderForwardYRotation) / 2;
            transform.rotation = Quaternion.Euler(0, netToGoalieYRotation + goaltenderDefaultYRotation, 0);
        }
    }
    public void SetShotDirection(Vector2 movementInput){
        if(movementInput.magnitude == 0){puckLaunchDirection = -transform.right;}
        else{puckLaunchDirection = new Vector3(movementInput.x, 0.25f, movementInput.y);}
    }
    public IEnumerator WindUpShot(){
        while(teamMember.windingUp){
            yield return new WaitForSeconds((0.25f));
            if(shotPower < shotPowerMax){shotPower += (shotPowerWindUpRate * 0.25f);}
            Debug.Log("Wind Up - " + shotPower);
        }
    }
    public void ShootPuck(){
        teamMember.windingUp = false;
        if(teamMember.hasPosession){
            teamMember.BreakPosession();
            Debug.Log($"Shot Direction Magnitude: {puckLaunchDirection.magnitude}");
            gameSystem.puckObject.GetComponent<Rigidbody>().AddForce(puckLaunchDirection * shotPower, ForceMode.Impulse);
        }
        shotPower = 6f;
    }
    private void Update(){
        HandleMove();
    }
}
