using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PosessionCollider : MonoBehaviour
{
    [SerializeField] GameObject thisPlayer;
    private GameSystem gameSystem;
    private TeamMember teamMember;
    private void Awake(){
        gameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        teamMember = thisPlayer.GetComponent<TeamMember>();
    }
    private void OnTriggerEnter(Collider other){
        if (other.tag == "puck" && !teamMember.memberHasPosession) {
            teamMember.TakePosession();
        }
    }
    private void OnTriggerStay(Collider other){
        if(other.tag == "puck"){
            Debug.Log("Still Have It");
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "puck" && teamMember.memberHasPosession){
            teamMember.LosePosession();
        }
    }
}