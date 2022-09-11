using System;
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
    [SerializeField] Transform desiredPuckPosition;
    [SerializeField] GameObject skaterPosessionTrigger;
    [SerializeField] GameObject puckPositionMarker;
    [SerializeField] float puckControlSpeed;
    private FixedJoint puckHandleJoint;
    public static EventHandler<EventArgs> takenPosession;
    [HideInInspector] public bool canTakePosession = true;
    [HideInInspector] public bool hasPosession = false;
    private float posessionCooldownTime = 0.2f;
    private void Awake(){
        gameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }
    public void ControlPuck(){
        if(canTakePosession){
            canTakePosession = false;
            hasPosession = true;
            // teleports the puck to the position marker.
            // allows it to teleport through the objects.
            puckPositionMarker.transform.position = gameSystem.puckObject.transform.position;
            // invoke gained posession event
            // event args, home/away
            takenPosession?.Invoke(this, EventArgs.Empty);
            if(!puckPositionMarker.GetComponent<FixedJoint>()){
                audioManager.PlayTakePossessionSFX();
                puckPositionMarker.GetComponent<PuckHandleJoint>().AttachPuckToHandleJoint(gameSystem.puckRigidBody);
            }
        }
    }
    public void DisableInteractions(){
        StartCoroutine(BreakPosession());
        windingUp = false;
        skaterPosessionTrigger.SetActive(false);
    }
    public void EnableInteractions(){
        skaterPosessionTrigger.SetActive(true);
        canTakePosession = true;
    }
    public IEnumerator BreakPosession(){
        if(!gameSystem.puckObject){yield break;}
        puckPositionMarker.GetComponent<PuckHandleJoint>().BreakFixedJoint();
        hasPosession = false;
        canTakePosession = false;
        yield return new WaitForSeconds(posessionCooldownTime);
        canTakePosession = true;
    }
    public void PuckPositionHandler(){
        if(hasPosession){
            // move towards the desired position
            puckPositionMarker.transform.position = Vector3.MoveTowards(puckPositionMarker.transform.position, desiredPuckPosition.position, puckControlSpeed*Time.deltaTime);
        }
    }
    private void Update() {
        PuckPositionHandler();
    }
}
