using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PosessionCollider : MonoBehaviour
{
    [SerializeField] GameObject thisPlayerObject;
    [SerializeField] SkaterAnimationScript skaterAnimationScript;
    private GameSystem gameSystem;
    private TeamMember teamMember;
    private void Awake(){
        gameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        teamMember = thisPlayerObject.GetComponent<TeamMember>();
    }
    private void OnTriggerEnter(Collider other){
        if(other.tag == "puck" && !teamMember.hasPosession){
            // dispossess the other player
            // break the fixed joint
            teamMember.ControlPuck();
        }
    }
    void OnTriggerExit(Collider other){
        if(other.tag == "puck" && teamMember.hasPosession){
            StartCoroutine(teamMember.BreakPosession());
        }
    }
}