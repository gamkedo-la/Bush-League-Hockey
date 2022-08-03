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
        if(collision.gameObject.tag == "homeNet"){
            Destroy(gameObject);
            gameSystem.GoalScored(true);
        } else if(collision.gameObject.tag == "awayNet"){
            Destroy(gameObject);
            gameSystem.GoalScored(false);
        }
    }
}
