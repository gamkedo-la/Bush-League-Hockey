using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private GameSystem gameSystem;
    [HideInInspector] public bool goalIsActive = false;
    private void Awake(){
        gameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
    }
    private void OnTriggerEnter(Collider other){
        Debug.Log($"Goal Active? {goalIsActive}");
        if(other.tag == "puck" && goalIsActive){
            goalIsActive = false;
            gameSystem.GoalScored(gameObject.tag == "homeNet");
        }
    }
}
