using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TeamMember : MonoBehaviour
{
    [HideInInspector] public bool windingUp = false;
    private GameSystem gameSystem;
    private AudioManager audioManager;
    [SerializeField] public bool isHomeTeam;
    private Rigidbody thisPlayersRigidBody;
    [Header("Puck Control")]
    [SerializeField] Collider skaterPosessionTrigger;
    [SerializeField] GameObject puckPositionMarker;
    private FixedJoint puckHandleJoint;
    [HideInInspector] public bool canTakePosession = true;
    [HideInInspector] public bool hasPosession = false;
    private float posessionCooldownTime = 0.5f;
    [Header("Passing")]
    [SerializeField] [Range(0.5f, 6f)] float passPowerWindUpRate; // extraPower / second
    [SerializeField] [Range(6f, 12f)] float passPowerMax;
    [SerializeField] [Range(1f, 8f)] float passPower;
    private float extraPower;
    private Vector3 puckLaunchDirection;
    [Header("Animation")]
    [SerializeField] SkaterAnimationScript skaterAnimationScript;
    private void Awake(){
        gameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }
    public void SetIsHomeTeam(bool isHome){
        isHomeTeam = isHome;
    }
    public void ControlPuck(){
        if(canTakePosession){
            canTakePosession = false;
            hasPosession = true;
            gameSystem.puckObject.transform.position = puckPositionMarker.transform.position;
            if(!puckPositionMarker.GetComponent<FixedJoint>()){
                puckPositionMarker.GetComponent<PuckHandleJoint>().AttachPuckToHandleJoint(gameSystem.puckRigidBody);
            }
        }
    }
    public bool HasPuck(){ return hasPosession; }
    public IEnumerator LostPosession(){
        hasPosession = false;
        yield return new WaitForSeconds(posessionCooldownTime);
        canTakePosession = true;
    }
    public void BreakPosession(){
        puckPositionMarker.GetComponent<PuckHandleJoint>().BreakFixedJoint();
    }
    public string getOppositionTag(){
        return isHomeTeam ? "awaySkater" : "homeSkater";
    }
    public void SetPassDirection(Vector3 movementInput){
        if(movementInput.magnitude == 0){puckLaunchDirection = transform.forward;}
        else{puckLaunchDirection = movementInput;}
    }
    public IEnumerator WindUpPass(){
        windingUp = true;
        extraPower = 0f;
        skaterAnimationScript?.skaterAnimator.SetTrigger("AnimatePassWindUp");
        while(windingUp){
            yield return new WaitForSeconds((Time.deltaTime));
            if(passPower + extraPower < passPowerMax){extraPower += (passPowerWindUpRate * Time.deltaTime);}
        }
    }
    public void PassPuck(){
        windingUp = false;
        skaterAnimationScript?.skaterAnimator.ResetTrigger("AnimatePassFollowThru");
        if(hasPosession){
            BreakPosession();
            skaterAnimationScript?.skaterAnimator.SetTrigger("AnimatePassFollowThru");
            audioManager.PlayPassSFX();
            gameSystem.puckObject.GetComponent<Rigidbody>().AddForce(puckLaunchDirection * (passPower + extraPower), ForceMode.Impulse);
        } else{
            skaterAnimationScript?.ResetAnimations();
        }
    }
}
