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
    public IEnumerator LostPosession(){
        hasPosession = false;
        yield return new WaitForSeconds(posessionCooldownTime);
        canTakePosession = true;
    }
    public void BreakPosession(){
        puckPositionMarker.GetComponent<PuckHandleJoint>().BreakFixedJoint();
        StartCoroutine(LostPosession());
    }
    public string getOppositionTag(){
        return isHomeTeam ? "awaySkater" : "homeSkater";
    }
}
