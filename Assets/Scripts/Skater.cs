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
    private float extraShotPower;
    private Vector3 puckLaunchDirection;
    [Header("Colliding/Checking")]
    [SerializeField] GameObject skaterModel;
    [SerializeField] private BodycheckHitZone bodycheckHitZone;
    [SerializeField] private LayerMask skaterMask;
    [SerializeField] private AnimationCurve checkPowerCurve;
    [SerializeField] [Range(10f, 30f)] private float checkPower;
    [SerializeField] [Range(15f, 50f)] private float checkPowerMax;
    [SerializeField] [Range(1f, 10f)] private float checkPowerWindUpRate;
    [SerializeField] [Range(0.2f, 3f)] private float bodycheckCooldownTime;
    private float extraBodycheckPower;
    private bool bodycheckReady = true;
    [HideInInspector] public bool isKnockedDown;
    private Vector3 bodycheckDirection;
    private Collider[] boxCastHits;
    private TeamMember teamMember;
    private void Awake(){
        skaterRigidBody = GetComponent<Rigidbody>();
        gameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        boxCastHits = new Collider[3];
        teamMember = GetComponent<TeamMember>();
    }
    public void SetPointers(Vector3 movementInput){
        movementPointer = movementInput;
        if(movementInput.magnitude == 0){
            puckLaunchDirection = Vector3.Normalize(skaterRigidBody.velocity);
            bodycheckDirection = Vector3.Normalize(skaterRigidBody.velocity);
        }
        else{
            puckLaunchDirection = new Vector3(movementInput.x, 0.2f, movementInput.z);
            bodycheckDirection = movementInput;
        }
    }
    public IEnumerator WindUpShot(){
        teamMember.windingUp = true;
        extraShotPower = 0f;
        skaterAnimationScript.skaterAnimator.SetBool("AnimateShotWindUp", true);
        while(teamMember.windingUp){
            yield return new WaitForSeconds((Time.deltaTime));
            if(shotPower + extraShotPower < shotPowerMax){extraShotPower += (shotPowerWindUpRate * Time.deltaTime);}
            // charge up animation should be a function of extraPower
            // can the animation be manually stepped forward / back?
        }
    }
    public void ShootPuck(){
        teamMember.windingUp = false;
        if(teamMember.hasPosession){
            teamMember.BreakPosession();
            skaterAnimationScript.skaterAnimator.SetTrigger("AnimateShotFollowThru");
            audioManager.PlayShotSFX();
            gameSystem.puckObject.GetComponent<Rigidbody>().AddForce(puckLaunchDirection * (shotPower + extraShotPower), ForceMode.Impulse);
        } else{
            skaterAnimationScript.ResetAnimations();
        }
    }
    public IEnumerator CooldownBodycheck(){
        bodycheckReady = false;
        yield return new WaitForSeconds(bodycheckCooldownTime);
        bodycheckReady = true;
    }
    public IEnumerator WindUpBodyCheck(){
        if(!bodycheckReady || teamMember.windingUp) yield break;
        skaterAnimationScript.ResetAnimations();
        teamMember.windingUp = true;
        teamMember.canTakePosession = false;
        skaterAnimationScript.skaterAnimator.SetBool("AnimateBodychecking", true);
        extraBodycheckPower = 0f;
        while(teamMember.windingUp){
            yield return new WaitForSeconds((Time.deltaTime));
            if(checkPower + extraBodycheckPower < checkPowerMax){extraBodycheckPower += (checkPowerWindUpRate * Time.deltaTime);}
        }
    }
    public void DeliverBodyCheck(){
        if(!bodycheckReady || !teamMember.windingUp) return;
        teamMember.windingUp = false;
        teamMember.canTakePosession = true;
        skaterAnimationScript.skaterAnimator.SetTrigger("AnimateBodycheckFollowThru");
        StartCoroutine(CooldownBodycheck());
        bodycheckHitZone.hitPower = checkPower + extraBodycheckPower + skaterRigidBody.velocity.magnitude/5;
        bodycheckHitZone.hitDirection = Vector3.Normalize(bodycheckDirection + Vector3.up*1.2f);
        skaterRigidBody.AddForce(bodycheckDirection*((checkPower + extraBodycheckPower)/4), ForceMode.VelocityChange);
    }
    public void ReceiveBodyCheck(float incomingHitPower, Vector3 hitDirection){
        isKnockedDown = true;
        teamMember.windingUp = false;
        GetComponent<Collider>().enabled = false;
        audioManager.PlayBodycheckSFX();
        teamMember.BreakPosession();
        StartCoroutine(skaterAnimationScript.RagdollThenReset(incomingHitPower, hitDirection, 3f));
    }
    private float GetBodyCheckPower(){
        var t = checkPower / checkPowerMax;
        var curveT = checkPowerCurve.Evaluate(t);
        return ((checkPowerMax - checkPower) * curveT) + checkPower;
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
    }
    public void HandleRotation(){
        // follow movementPointer, 
        if(teamMember.windingUp && movementPointer.magnitude > 0.1f){
            desiredRotation = Quaternion.LookRotation(movementPointer, Vector3.up);
            rotationThisFrame = Quaternion.Lerp(transform.rotation, desiredRotation, skaterTurnSpeed);
        } else {
            desiredRotation = Quaternion.LookRotation(skaterRigidBody.velocity, Vector3.up);
            rotationThisFrame = Quaternion.Lerp(transform.rotation, desiredRotation, skaterTurnSpeed);
        }
        transform.rotation = rotationThisFrame;
    }
    private void Update(){
        HandleMove();
        HandleRotation();
    }
}
