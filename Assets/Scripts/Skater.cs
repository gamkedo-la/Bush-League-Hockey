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
    private Vector3 stickControlPointer;
    private Vector3 cameraForward;
    private Vector3 cameraRight;
    private Quaternion desiredRotation;
    private Quaternion rotationThisFrame;
    [Header("Shooting")]
    [SerializeField] [Range(0.5f, 4f)] float shotPowerWindUpRate; // extraPower / second
    [SerializeField] [Range(8f, 20f)] float shotPowerMax;
    [SerializeField] [Range(4f, 16f)] float shotPower;
    private bool windingUpShot;
    private float extraShotPower;
    private Vector3 shotDirection;
    [Header("Passing")]
    [SerializeField] [Range(0.5f, 6f)] float passPowerWindUpRate; // extraPassPower / second
    [SerializeField] [Range(6f, 12f)] float passPowerMax;
    [SerializeField] [Range(1f, 8f)] float passPower;
    private float extraPassPower;
    private bool windingUpPass;
    [Header("Colliding/Checking")]
    [SerializeField] GameObject skaterModel;
    [SerializeField] private BodycheckHitZone bodycheckHitZone;
    [SerializeField] private LayerMask skaterMask;
    [SerializeField] private AnimationCurve checkPowerCurve;
    [SerializeField] [Range(10f, 30f)] private float checkPower;
    [SerializeField] [Range(15f, 50f)] private float checkPowerMax;
    [SerializeField] [Range(1f, 10f)] private float checkPowerWindUpRate;
    [SerializeField] [Range(0.2f, 3f)] private float bodycheckCooldownTime;
    private bool windingUpBodycheck;
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
            shotDirection = Vector3.Normalize(skaterRigidBody.velocity);
            bodycheckDirection = Vector3.Normalize(skaterRigidBody.velocity);
        }
        else{
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
        skaterAnimationScript?.skaterAnimator.SetTrigger("AnimatePassWindUp");
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
            teamMember.BreakPosession();
            audioManager.PlayPassSFX();
            skaterAnimationScript.skaterAnimator.SetTrigger("AnimatePassFollowThru");
            gameSystem.puckObject.GetComponent<Rigidbody>().AddForce(bodycheckDirection * (passPower + extraPassPower), ForceMode.Impulse);
        } else{
            skaterAnimationScript?.ResetAnimations();
        }
    }
    public IEnumerator WindUpShot(){
        // blocked when: already winding up, knocked down
        if(WindingUp() || isKnockedDown) yield break;
        windingUpShot = true;
        skaterAnimationScript.skaterAnimator.SetBool("AnimateShotWindUp", true);
        extraShotPower = 0f;
        while(windingUpShot){
            yield return new WaitForSeconds((Time.deltaTime));
            if(shotPower + extraShotPower < shotPowerMax){extraShotPower += (shotPowerWindUpRate * Time.deltaTime);}
            Debug.Log($"ShotPower: {shotPower + extraShotPower}");
            // charge up animation should be a function of extraPower
            // can the animation be manually stepped forward / back?
        }
    }
    public void ShootPuck(){
        // blocked when: no wind up, knocked down
        if(!windingUpShot || isKnockedDown) return;
        windingUpShot = false;
        if(teamMember.hasPosession){
            teamMember.BreakPosession();
            audioManager.PlayShotSFX();
            skaterAnimationScript.skaterAnimator.SetTrigger("AnimateShotFollowThru");
            gameSystem.puckObject.GetComponent<Rigidbody>().AddForce(shotDirection * (shotPower + extraShotPower), ForceMode.Impulse);
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
        // blocked when: already winding up, bodycheck on cooldown, knocked down, has posession
        if(WindingUp() || !bodycheckReady || isKnockedDown || teamMember.hasPosession) yield break;
        skaterAnimationScript.ResetAnimations();
        windingUpBodycheck = true;
        teamMember.canTakePosession = false;
        skaterAnimationScript.skaterAnimator.SetBool("AnimateBodychecking", true);
        extraBodycheckPower = 0f;
        while(teamMember.windingUp){
            yield return new WaitForSeconds((Time.deltaTime));
            if(checkPower + extraBodycheckPower < checkPowerMax){extraBodycheckPower += (checkPowerWindUpRate * Time.deltaTime);}
        }
    }
    public void DeliverBodyCheck(){
        // blocked when: no bodycheck windup, bodycheck on cooldown, knocked down, has posession
        if(!bodycheckReady || !windingUpBodycheck || isKnockedDown || teamMember.hasPosession) return;
        windingUpBodycheck = false;
        teamMember.canTakePosession = true;
        skaterAnimationScript.skaterAnimator.SetTrigger("AnimateBodycheckFollowThru");
        StartCoroutine(CooldownBodycheck());
        bodycheckHitZone.hitPower = checkPower + extraBodycheckPower + (skaterRigidBody.velocity.magnitude/2);
        bodycheckHitZone.hitDirection = (bodycheckDirection + Vector3.up).normalized;
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
        if(movementPointer.magnitude <= 0.1f || WindingUp() || isKnockedDown) return;
        if(skaterRigidBody.angularVelocity.magnitude > 0){skaterRigidBody.angularVelocity = Vector3.zero;}
        float moveDirectionDelta = Vector3.Angle(skaterRigidBody.velocity, movementPointer);
        Debug.Log($"moveDelta: {moveDirectionDelta}");
        skaterRigidBody.AddForce(movementPointer * skaterAcceleration);
        if (moveDirectionDelta > 160){
            skaterRigidBody.velocity *= 0.985f;
        }
        // change direction, velocity constant
        // change direction, fast stop
        // direction and acceleration are a function of y axis rotation
        // +-90: can change direction with constant speed
        // +-90 to +-155: Carving, decelerate hard along forward axis 
        // +-155 to +-180: Hard stop, quickly decelerate to 0
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
        rotationThisFrame = Quaternion.Lerp(transform.rotation, desiredRotation, skaterTurnSpeed*Time.deltaTime);
        transform.rotation = rotationThisFrame;
    }
    private void Update(){
        HandleMove();
        HandleRotation();
    }
}
