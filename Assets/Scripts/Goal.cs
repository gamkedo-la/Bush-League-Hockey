using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private GameSystem gameSystem;
    [HideInInspector] public bool goalHasCounted = false;
    private void Awake(){
        gameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
    }
    public void GameOn(){
        goalHasCounted = false;
    }
    private void OnTriggerEnter(Collider other){
        if(other.tag == "puck" && !goalHasCounted){
            Debug.Log("Goal: " + other);
            goalHasCounted = true;
            gameSystem.GoalScored(gameObject.tag == "awayNet");
        }
    }
}
