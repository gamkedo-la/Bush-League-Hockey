using System;
using UnityEngine;
public class Goal : MonoBehaviour
{
    public static EventHandler<EventArgs> homeGoalScored;
    public static EventHandler<EventArgs> awayGoalScored;
    private GameSystem gameSystem;
    [HideInInspector] public bool goalIsActive = true;
    private void Awake(){
        gameSystem = FindObjectOfType<GameSystem>();
    }
    private void OnTriggerEnter(Collider other){
        if(other.tag == "puck" && goalIsActive){
            goalIsActive = false;
            if(gameObject.tag == "homeGoal"){
                awayGoalScored?.Invoke(this, EventArgs.Empty);
            }
            else{
                homeGoalScored?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
