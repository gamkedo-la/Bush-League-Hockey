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
    private bool canTakePosession = true;
    [HideInInspector] public bool hasPosession = false;
    private float posessionCooldownTime = 0.5f;
    [Header("Passing")]
    [SerializeField] [Range(0.5f, 6f)] float passPowerWindUpRate; // extraPower / second
    [SerializeField] [Range(6f, 12f)] float passPowerMax;
    [SerializeField] [Range(1f, 8f)] float passPower;
    private float extraPower;
    private Vector3 puckLaunchDirection;

    private void Awake(){
        gameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }
    public void SetIsHomeTeam(bool isHome){
        isHomeTeam = isHome;
    }
    public void ControlPuck(){
        // relative velocity
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
    public void SetPassDirection(Vector2 movementInput){
        if(movementInput.magnitude == 0){puckLaunchDirection = transform.forward;}
        else{puckLaunchDirection = new Vector3(movementInput.x, 0, movementInput.y);}
    }
    public IEnumerator WindUpPass(){
        extraPower = 0f;
        while(windingUp){
            yield return new WaitForSeconds((Time.deltaTime));
            if(passPower + extraPower < passPowerMax){extraPower += (passPowerWindUpRate * Time.deltaTime);}
            Debug.Log($"Pass Windup: {passPower + extraPower}");
        }
    }
    public void PassPuck(){
        windingUp = false;
        if(hasPosession){
            BreakPosession();
            audioManager.PlayPassSFX();
            gameSystem.puckObject.GetComponent<Rigidbody>().AddForce(puckLaunchDirection * (passPower + extraPower), ForceMode.Impulse);
        }
    }
}
