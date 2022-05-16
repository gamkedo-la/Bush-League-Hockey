using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PosessionCollider : MonoBehaviour
{
    [SerializeField] GameObject thisPlayer;
    private GameSystem gameSystem;
    private TeamMember teamMember;
    private Skater skater;
    private void Awake(){
        gameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        teamMember = thisPlayer.GetComponent<TeamMember>();
        skater = thisPlayer.GetComponent<Skater>();
    }
    private void OnTriggerStay(Collider other){
        if(other.tag == "puck"){
            Debug.Log("I can control it; see!");
            skater?.ControlPuck();
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "puck"){
            Debug.Log("I can't control it anymore :(");
            skater?.ResetFirstTouch();
        }
    }
}