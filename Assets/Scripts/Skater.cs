using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
public class Skater : MonoBehaviour
{
    [SerializeField] SkaterAnimationScript skaterAnimationScript;
    private GameSystem gameSystem;
    private AudioManager audioManager;
    [Header("Skating")]
    [SerializeField] float skaterAcceleration;
    [SerializeField] float skaterMaximumSpeed;
    [SerializeField] float skaterTurnSpeed;
    [SerializeField] float angleAccelerationLimit;
    [SerializeField] float angleTurnLimit;
    private float angleMovementDelta;
    private Rigidbody skaterRigidBody;
    private Vector3 movementPointer;
    private Vector3 stickControlPointer;
    private Quaternion desiredRotation;
    private Quaternion rotationThisFrame;
    [Header("Shooting")]
    [SerializeField] [Range(0.5f, 4f)] float shotPowerWindUpRate; // extraPower / second
    [SerializeField] [Range(8f, 20f)] float shotPowerMax;
    [SerializeField] [Range(4f, 16f)] float shotPower;
    public bool windingUpShot;
    private float extraShotPower;
    private Vector3 shotDirection;
    [Header("Passing")]
    [SerializeField] [Range(0.5f, 6f)] float passPowerWindUpRate; // extraPassPower / second
    [SerializeField] [Range(6f, 12f)] float passPowerMax;
    [SerializeField] [Range(1f, 8f)] float passPower;
    private float extraPassPower;
    public bool windingUpPass;
    [Header("Colliding/Checking")]
    [SerializeField] GameObject skaterModel;
    [SerializeField] private BodycheckHitZone bodycheckHitZone;
    [SerializeField] private LayerMask skaterMask;
    [SerializeField] private AnimationCurve checkPowerCurve;
    [SerializeField] [Range(1f, 6f)] private float checkPower;
    [SerializeField] [Range(4f, 64f)] private float checkPowerMax;
    [SerializeField] [Range(1f, 12f)] private float checkPowerWindUpRate;
    [SerializeField] [Range(0.2f, 3f)] private float bodycheckCooldownTime;
    public bool windingUpBodycheck;
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
    public void ResetSkaterMotion(){
        skaterRigidBody.velocity = Vector3.zero;
        skaterRigidBody.angularVelocity = Vector3.zero;
    }
    public void ResetSkaterActions(){
        teamMember.hasPosession = false;
        windingUpShot = false;
        windingUpBodycheck = false;
        windingUpPass = false;
        extraShotPower = 0;
        extraBodycheckPower = 0;
        extraPassPower = 0;
        skaterAnimationScript.DeactivateBodycheck();
        skaterAnimationScript.ResetAnimations();
        skaterAnimationScript.ResetRagdoll();
    }
    public void SetPointers(Vector3 movementInput){
        movementPointer = movementInput;
        if(movementInput.magnitude == 0){
            shotDirection = Vector3.Normalize(skaterRigidBody.velocity);
            bodycheckDirection = Vector3.Normalize(skaterRigidBody.velocity);
        } else {
            shotDirection = new Vector3(movementInput.x, 0.2f, movementInput.z);
            bodycheckDirection = movementInput;
        }
    }
    public void SetStickControlPointer(Vector3 stickControlInput){
        stickControlPointer = stickControlInput;
    }
    public bool WindingUp(){
        return windingUpShot || windingUpBodycheck || windingUpPass;
    }
    
    public IEnumerator WindUpPass(){
        // blocked when: already winding up, knocked down
        if(WindingUp() || isKnockedDown) yield break;
        windingUpPass = true;
        extraPassPower = 0f;
        skaterAnimationScript.skaterAnimator.SetBool("AnimatePassWindUp", true);
        skaterAnimationScript.DisableRigExceptHead();
        while(WindingUp()){
            yield return new WaitForSeconds((Time.deltaTime));
            if(passPower + extraPassPower < passPowerMax){extraPassPower += (passPowerWindUpRate * Time.deltaTime);}
        }
    }
    public void PassPuck(){
        // blocked when: no wind up, knocked down
        if(!windingUpPass || isKnockedDown) return;
        windingUpPass = false;
        if(teamMember.hasPosession){
            StartCoroutine(teamMember.BreakPosession());
            StartCoroutine(audioManager.PlayPassSFX());
            skaterAnimationScript.skaterAnimator.SetTrigger("AnimatePassFollowThru");
            gameSystem.puckObject.GetComponent<Rigidbody>().AddForce(bodycheckDirection * (passPower + extraPassPower), ForceMode.Impulse);
            if (gameObject.tag == "awaySkater"){
                gameSystem.awayPasses++;
            }
            else if (gameObject.tag == "homeSkater"){
                gameSystem.homePasses++;
            }
        } else{
            ResetSkaterActions();
        }
    }
    public IEnumerator WindUpShot(){
        // blocked when: already winding up, knocked down
        if(WindingUp() || isKnockedDown) yield break;
        windingUpShot = true;
        extraShotPower = 0f;
        skaterAnimationScript.skaterAnimator.SetBool("AnimateShotWindUp", true);
        skaterAnimationScript.DisableRigExceptHead();
        while(windingUpShot){
            yield return new WaitForSeconds((Time.deltaTime));
            if(shotPower + extraShotPower < shotPowerMax){extraShotPower += (shotPowerWindUpRate * Time.deltaTime);}
        }
    }
    public void ShootPuck(){
        // blocked when: no wind up, knocked down
        if(!windingUpShot || isKnockedDown) return;
        windingUpShot = false;
        if(teamMember.hasPosession){
            StartCoroutine(teamMember.BreakPosession());
            audioManager.PlayShotSFX();
            skaterAnimationScript.skaterAnimator.SetTrigger("AnimateShotFollowThru");
            gameSystem.puckObject.GetComponent<Rigidbody>().AddForce(shotDirection * (shotPower + extraShotPower), ForceMode.Impulse);
        } else{
            ResetSkaterActions();
        }
    }
    public IEnumerator CooldownBodycheck(){
        bodycheckReady = false;
        yield return new WaitForSeconds(bodycheckCooldownTime);
        bodycheckReady = true;
    }
    public IEnumerator WindUpBodyCheck(){
        // blocked when: already winding up, bodycheck on cooldown, knocked down, has posession
        if(WindingUp() || !bodycheckReady || isKnockedDown || teamMember.hasPosession) yield break;
        skaterAnimationScript.ResetAnimations();
        windingUpBodycheck = true;
        teamMember.canTakePosession = false;
        extraBodycheckPower = 0f;
        skaterAnimationScript.skaterAnimator.SetBool("AnimateBodychecking", true);
        skaterAnimationScript.DisableRigExceptHead();
        while(windingUpBodycheck){
            yield return new WaitForSeconds((Time.deltaTime));
            if(checkPower + extraBodycheckPower < checkPowerMax){extraBodycheckPower += (checkPowerWindUpRate * Time.deltaTime);}
        }
    }
    public void DeliverBodyCheck(){
        if(!bodycheckReady || !windingUpBodycheck || isKnockedDown || teamMember.hasPosession) return;
        windingUpBodycheck = false;
        teamMember.canTakePosession = true;
        skaterAnimationScript.skaterAnimator.SetTrigger("AnimateBodycheckFollowThru");
        audioManager.PlayBodyCheckGrunt();
        StartCoroutine(CooldownBodycheck());
        bodycheckHitZone.hitPower = checkPower + extraBodycheckPower + (skaterRigidBody.velocity.magnitude*0.6f);
        bodycheckHitZone.hitDirection = (bodycheckDirection + Vector3.up*0.75f).normalized;
        Debug.Log($"P: {checkPower}   EP: {extraBodycheckPower}   V: {skaterRigidBody.velocity.magnitude}");
        skaterRigidBody.AddForce(bodycheckDirection*((checkPower + extraBodycheckPower)/3), ForceMode.VelocityChange);        
    }
    public void ReceiveBodyCheck(float incomingHitPower, Vector3 hitDirection){
        ResetSkaterActions();
        ResetSkaterMotion();
        isKnockedDown = true;
        teamMember.windingUp = false;
        GetComponent<Collider>().enabled = false;
        teamMember.DisableInteractions();
        StartCoroutine(audioManager.PlayBodycheckHitAndReaction());
        StartCoroutine(teamMember.BreakPosession());
        StartCoroutine(skaterAnimationScript.RagdollThenReset(incomingHitPower, hitDirection, 3f));
        //if home hit homehits++
        ///else if away hit awayhits++
        if (gameObject.tag == "awaySkater"){
            gameSystem.homeHits++;
        }
        else if (gameObject.tag == "homeSkater"){
            gameSystem.awayHits++;
        }
        // at the end of this, in RagdollThenReset() we trigger an instant replay
    }
    private float GetBodyCheckPower(){
        var t = checkPower / checkPowerMax;
        var curveT = checkPowerCurve.Evaluate(t);
        return ((checkPowerMax - checkPower) * curveT) + checkPower;
    }
    public IEnumerator Speedboost(){
        Debug.Log($"Skater Speed boost");
        skaterAcceleration *= 1.5f;
        yield return new WaitForSeconds(1);
        skaterAcceleration /= 1.5f;
    }
    public void HandleMove(){
        if(movementPointer.magnitude <= 0.1f || WindingUp() || isKnockedDown) return;
        if(skaterRigidBody.angularVelocity.magnitude > 0){skaterRigidBody.angularVelocity = Vector3.zero;}
        angleMovementDelta = Vector3.Angle(skaterRigidBody.velocity, movementPointer);
        if(angleMovementDelta <= angleAccelerationLimit || angleMovementDelta >= angleTurnLimit){
            skaterRigidBody.AddForce(
                movementPointer
                *skaterAcceleration
                *(1 - skaterRigidBody.velocity.magnitude/skaterMaximumSpeed)
            );
            // Procedural skate cycle is on
        }
        if(angleMovementDelta < angleTurnLimit){
            skaterRigidBody.velocity = Vector3.RotateTowards(
                skaterRigidBody.velocity,
                movementPointer*skaterRigidBody.velocity.magnitude,
                skaterTurnSpeed * Time.deltaTime,
                skaterAcceleration
            );
        }
        if(angleMovementDelta >= angleTurnLimit){
            skaterRigidBody.velocity *= 0.985f;
        }
    }
    public void HandleRotation(){
        if(isKnockedDown)return;
        if(stickControlPointer.magnitude > 0.1f){
            desiredRotation = Quaternion.LookRotation(stickControlPointer);
        }else if(WindingUp() && movementPointer.magnitude > 0.1f){
            desiredRotation = Quaternion.LookRotation(movementPointer);
        } else if(skaterRigidBody.velocity.magnitude > 0.1f) {
            desiredRotation = Quaternion.LookRotation(skaterRigidBody.velocity);
        }
        if(!gameSystem.IsZeroQuaternion(desiredRotation)){
            // player is turning, modify skate cycle
            // modify waypoint position based on magnitude of change
            rotationThisFrame = Quaternion.Lerp(transform.rotation, desiredRotation, skaterTurnSpeed * Time.deltaTime);
            transform.rotation = rotationThisFrame;
        }
    }
    private void Update(){
        HandleMove();
        HandleRotation();
    }
}
