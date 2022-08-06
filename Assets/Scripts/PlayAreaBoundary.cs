using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAreaBoundary : MonoBehaviour
{
    private GameSystem gameSystem;
    private void Awake(){
        gameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
    }
    void OnTriggerExit(Collider other){
        if(other.gameObject.tag == "puck" || other.gameObject.tag == "homeSkater" || other.gameObject.tag == "awaySkater"){
            gameSystem.PuckOutOfBounds();
        }
    }
}
