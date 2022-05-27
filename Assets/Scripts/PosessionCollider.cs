using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PosessionCollider : MonoBehaviour
{
    [SerializeField] GameObject thisPlayerObject;
    private GameSystem gameSystem;
    private TeamMember teamMember;
    private Skater skater;
    private void Awake(){
        gameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        teamMember = thisPlayerObject.GetComponent<TeamMember>();
        skater = thisPlayerObject.GetComponent<Skater>();
    }
    private void OnTriggerStay(Collider other){
        if(other.tag == "puck"){
            Debug.Log("I can control it; see!");
            skater?.ControlPuck();
        }
    }
    void OnTriggerExit(Collider other){
        if(other.tag == "puck"){
            Debug.Log("I can't control it anymore :(");
            StartCoroutine(skater?.CooldownFirstTouch());
        }
    }
}