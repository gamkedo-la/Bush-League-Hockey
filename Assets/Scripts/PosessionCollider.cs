using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PosessionCollider : MonoBehaviour
{
    [SerializeField] GameObject thisPlayerObject;
    private GameSystem gameSystem;
    private TeamMember teamMember;
    private void Awake(){
        gameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        teamMember = thisPlayerObject.GetComponent<TeamMember>();
    }
    private void OnTriggerStay(Collider other){
        if(other.tag == "puck"){
            teamMember?.ControlPuck();
        }
    }
    void OnTriggerExit(Collider other){
        if(other.tag == "puck"){
            StartCoroutine(teamMember.LostPosession());
        }
    }
}