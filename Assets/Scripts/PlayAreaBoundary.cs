using System;
using UnityEngine;

public class PlayAreaBoundary : MonoBehaviour
{
    public static EventHandler<EventArgs> outOfBounds;
    private GameSystem gameSystem;
    private void Awake(){
        gameSystem = FindObjectOfType<GameSystem>();
    }
    void OnTriggerExit(Collider other){
        if(other.gameObject.tag == "puck" || other.gameObject.tag == "homeSkater" || other.gameObject.tag == "awaySkater"){
            outOfBounds?.Invoke(this, EventArgs.Empty);
        }
        // if the object is a puck position marker, move somewhere in bounds
    }
}
