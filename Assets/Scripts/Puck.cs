using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Not being used, possible considerations later
public class Puck : MonoBehaviour
{
    private GameSystem gameSystem;
    private void Awake()
    {
        gameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
    }
    void OnCollisionEnter(Collision collision){
        Debug.Log("Puck colliding with: " + collision.gameObject);
        if(collision.gameObject.tag == "homeNet"){
            Debug.Log("Goal: " + collision.gameObject);
            Destroy(gameObject);
            gameSystem.GoalScored(true);
        } else if(collision.gameObject.tag == "awayNet"){
            Destroy(gameObject);
            gameSystem.GoalScored(false);
        }
    }
}
