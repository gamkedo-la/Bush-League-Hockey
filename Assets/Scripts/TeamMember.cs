using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TeamMember : MonoBehaviour
{
    [HideInInspector] public bool windingUp = false;
    private GameSystem gameSystem;
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
    [SerializeField] float passPowerWindUpRate; // power / second
    [SerializeField] float passPowerMax;
    private float passPower = 4f;
    private Vector3 puckLaunchDirection;

    private void Awake(){
        gameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
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
    public string getOppositionTag()
    {
        return isHomeTeam ? "awaySkater" : "homeSkater";
    }
    public void SetPassDirection(Vector2 movementInput){
        if(movementInput.magnitude == 0){puckLaunchDirection = Vector3.Normalize(transform.forward);}
        else{puckLaunchDirection = new Vector3(movementInput.x, 0, movementInput.y);}
    }
    public IEnumerator WindUpPass(){
        while(windingUp){
            yield return new WaitForSeconds((0.25f));
            if(passPower < passPowerMax){passPower += (passPowerWindUpRate * 0.25f);}
            Debug.Log("Wind Up - " + passPower);
        }
    }
    public void PassPuck(){
        windingUp = false;
        if(hasPosession){
            BreakPosession();
            gameSystem.puckObject.GetComponent<Rigidbody>().AddForce(puckLaunchDirection * passPower, ForceMode.Impulse);
        }
        passPower = 4f;
    }
}
