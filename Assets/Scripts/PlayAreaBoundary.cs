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
        Debug.Log("Puck has gone OB");
        if(other.gameObject.tag == "Puck")
        {
            gameSystem.DropPuck();
        }
    }
}
