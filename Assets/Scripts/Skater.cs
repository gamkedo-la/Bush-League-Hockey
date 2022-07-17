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
    [SerializeField] Animator skaterAnimator;
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
    [SerializeField] private Transform boxCastOrigin;
    [SerializeField] private Vector3 boxCastHalfExtents;
    [SerializeField] private LayerMask skaterMask;
    [SerializeField] private AnimationCurve checkPowerCurve;
    [SerializeField] private float minCheckPower;
    [SerializeField] private float maxCheckPower;
    private Collider[] boxCastHits;
    private TeamMember teamMember;
    private void Awake(){
        skaterRigidBody = GetComponent<Rigidbody>();
        gameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        boxCastHits = new Collider[3];
        teamMember = GetComponent<TeamMember>();
    }
    public void SetShotDirection(Vector2 movementInput){
        if(movementInput.magnitude == 0){puckLaunchDirection = Vector3.Normalize(transform.forward);}
        else{puckLaunchDirection = new Vector3(movementInput.x, 0.25f, movementInput.y);}
    }
    public IEnumerator WindUpShot(){
        extraPower = 0f;
        skaterAnimator.SetBool("AnimateShotWindUp", true);
        while(teamMember.windingUp){
            yield return new WaitForSeconds((Time.deltaTime));
            if(shotPower + extraPower < shotPowerMax){extraPower += (shotPowerWindUpRate * Time.deltaTime);}
            Debug.Log($"Winding Up:  {shotPower + extraPower}");
            // charge up animation should be a function of extraPower
            // can the animation be manually stepped forward / back?
        }
    }
    public void ShootPuck(){
        skaterAnimator.SetBool("AnimateShotWindUp", false);
        teamMember.windingUp = false;
        skaterAnimator.SetTrigger("AnimateShotFollowThru");
        if(teamMember.hasPosession){
            teamMember.BreakPosession();
            audioManager.PlayShotSFX();
            gameSystem.puckObject.GetComponent<Rigidbody>().AddForce(puckLaunchDirection * (shotPower + extraPower), ForceMode.Impulse);
        }
    }
    public void BodyCheck()
    {
        teamMember.windingUp = false;
        // @TODO: perform bodycheck animation
        // Check for BodyCheck target
        var hitCount = Physics.OverlapBoxNonAlloc(
            boxCastOrigin.position,
            boxCastHalfExtents,
            boxCastHits,
            Quaternion.identity,
            skaterMask);
        Debug.Log($"Bodycheck hitcount: {hitCount}");
        var oppositionTag = teamMember.getOppositionTag();
        // Look for correct tag in bodycheck list. Break on first match.
        for (var i = 0; i < hitCount; i++)
        {
            var hit = boxCastHits[i];
            if (hit.CompareTag(oppositionTag))
            {
                var force = transform.forward * GetBodyCheckPower();
                var contactPoint = hit.ClosestPoint(boxCastOrigin.position);
                hit.attachedRigidbody.AddForceAtPosition(force, contactPoint, ForceMode.Impulse);
                Debug.Log($"Bodycheck {hit.name} with force {force}");
                break;
            }
        }
    }
    // Maps shotPower to a value between minCheckPower and maxCheckPower
    // by sampling the checkPowerCurve. 
    private float GetBodyCheckPower()
    {
        var t = shotPower / shotPowerMax;
        var curveT = checkPowerCurve.Evaluate(t);
        return ((maxCheckPower - minCheckPower) * curveT) + minCheckPower;
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
        if(movementPointer.magnitude > 0.1f && !teamMember.windingUp){
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
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCastOrigin.position, boxCastHalfExtents*2);
    }
}
