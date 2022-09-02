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
    [SerializeField] GameObject skaterPosessionTrigger;
    [SerializeField] GameObject puckPositionMarker;
    private FixedJoint puckHandleJoint;
    [HideInInspector] public bool canTakePosession = true;
    [HideInInspector] public bool hasPosession = false;
    private float posessionCooldownTime = 0.3f;
    private void Awake(){
        gameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }
    public void ControlPuck(){
        if(canTakePosession){
            canTakePosession = false;
            hasPosession = true;
            gameSystem.puckObject.transform.position = puckPositionMarker.transform.position;
            if(!puckPositionMarker.GetComponent<FixedJoint>()){
                audioManager.PlayTakePossessionSFX();
                puckPositionMarker.GetComponent<PuckHandleJoint>().AttachPuckToHandleJoint(gameSystem.puckRigidBody);
            }
        }
    }
    public void DisableInteractions(){
        BreakPosession();
        windingUp = false;
        skaterPosessionTrigger.SetActive(false);
    }
    public void EnableInteractions(){
        skaterPosessionTrigger.SetActive(true);
        canTakePosession = true;
    }
    public IEnumerator PreventPosession(){
        canTakePosession = false;
        yield return new WaitForSeconds(posessionCooldownTime);
        canTakePosession = true;
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
}
