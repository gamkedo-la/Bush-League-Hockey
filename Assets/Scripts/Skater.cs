using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
public class Skater : MonoBehaviour
{
    private GameSystem gameSystem;
    private AudioManager audioManager;
    [Header("Animation")]
    [SerializeField] SkaterAnimationScript skaterAnimationScript;
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
    [Header("Shooting")]
    [SerializeField] [Range(0.5f, 6f)] float shotPowerWindUpRate; // extraPower / second
    [SerializeField] [Range(8f, 20f)] float shotPowerMax;
    [SerializeField] [Range(0.0f, 10f)] float shotPower;
    private float extraPower;
    private Vector3 puckLaunchDirection;
    [Header("Colliding/Checking")]
    [SerializeField] private GameObject bodycheckHitZone;
    [SerializeField] private LayerMask skaterMask;
    [SerializeField] private AnimationCurve checkPowerCurve;
    [SerializeField] [Range(5f, 10f)] private float checkPower;
    [SerializeField] [Range(10f, 20f)] private float checkPowerMax;
    [SerializeField] [Range(1f, 5f)] private float checkPowerWindUpRate;
    [HideInInspector] public bool isKnockedDown;
    private Vector3 bodycheckForce;
    private Collider[] boxCastHits;
    private TeamMember teamMember;
    private void Awake(){
        skaterRigidBody = GetComponent<Rigidbody>();
        gameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        boxCastHits = new Collider[3];
        teamMember = GetComponent<TeamMember>();
    }
    public void SetMovementPointers(Vector2 movementInput){
        if(movementInput.magnitude == 0){puckLaunchDirection = Vector3.Normalize(transform.forward);}
        else{puckLaunchDirection = new Vector3(movementInput.x, 0.25f, movementInput.y);}
    }
    public IEnumerator WindUpShot(){
        teamMember.windingUp = true;
        extraPower = 0f;
        skaterAnimationScript.skaterAnimator.SetTrigger("AnimateShotWindUp");
        while(teamMember.windingUp){
            yield return new WaitForSeconds((Time.deltaTime));
            if(shotPower + extraPower < shotPowerMax){extraPower += (shotPowerWindUpRate * Time.deltaTime);}
            // charge up animation should be a function of extraPower
            // can the animation be manually stepped forward / back?
        }
    }
    public void ShootPuck(){
        teamMember.windingUp = false;
        skaterAnimationScript.skaterAnimator.ResetTrigger("AnimateShotFollowThru");
        if(teamMember.hasPosession){
            teamMember.BreakPosession();
            skaterAnimationScript.skaterAnimator.SetTrigger("AnimateShotFollowThru");
            audioManager.PlayShotSFX();
            gameSystem.puckObject.GetComponent<Rigidbody>().AddForce(puckLaunchDirection * (shotPower + extraPower), ForceMode.Impulse);
        } else{
            skaterAnimationScript.StopWindUpAnimation();
        }
    }
    public IEnumerator WindUpBodyCheck(){
        teamMember.windingUp = true;
        extraPower = 0f;
        //skaterAnimationScript.skaterAnimator.SetTrigger("AnimateBodyCheckWindUp");
        while(teamMember.windingUp && !teamMember.hasPosession){
            yield return new WaitForSeconds((Time.deltaTime));
            if(checkPower + extraPower < checkPowerMax){extraPower += (checkPowerWindUpRate * Time.deltaTime);}
        }
    }
    public IEnumerator DeliverBodyCheck(){
        // send player in aim direction
        teamMember.windingUp = false;
        bodycheckHitZone.GetComponent<BodycheckHitZone>().hitForce = puckLaunchDirection * (checkPower + extraPower);
        bodycheckHitZone.SetActive(true);
        yield return new WaitForSeconds(1f);
        bodycheckHitZone.SetActive(false);
    }
    public void ReceiveBodyCheck(Vector3 hitForce){
        teamMember.BreakPosession();
        teamMember.windingUp = false;
        StartCoroutine(skaterAnimationScript.RagdollThenReset(3f, hitForce));
    }
    // Maps shotPower to a value between minCheckPower and maxCheckPower
    // by sampling the checkPowerCurve.
    private float GetBodyCheckPower(){
        var t = checkPower / checkPowerMax;
        var curveT = checkPowerCurve.Evaluate(t);
        return ((checkPowerMax - checkPower) * curveT) + checkPower;
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
        if(movementPointer.magnitude > 0.1f && !teamMember.windingUp && !isKnockedDown){
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
