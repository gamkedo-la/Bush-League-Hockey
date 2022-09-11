using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAreaBoundary : MonoBehaviour
{
    private GameSystem gameSystem;
    private void Awake(){
        gameSystem = FindObjectOfType<GameSystem>();
    }
    void OnTriggerExit(Collider other){
        if(other.gameObject.tag == "puck" || other.gameObject.tag == "homeSkater" || other.gameObject.tag == "awaySkater"){
            gameSystem.PuckOutOfBounds();
        }
        // if the object is a puck position marker, move somewhere in bounds
    }
}
