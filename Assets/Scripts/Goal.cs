using System;
using UnityEngine;
public class Goal : MonoBehaviour
{
    public static EventHandler<EventArgs> homeGoalTrigger;
    public static EventHandler<EventArgs> awayGoalTrigger;
    private GameSystem gameSystem;
    [HideInInspector] private bool goalIsActive = true;
    private void Awake(){
        gameSystem = FindObjectOfType<GameSystem>();
        MainMenuState.onStateEnter += ActivateGoal;
        CountGoals.onStateEnter += ActivateGoal;
        BeginGameState.onStateEnter += DeactivateGoal;
        CountGoals.onStateExit += DeactivateGoal;
        RunClockState.timerDone += DeactivateGoal;
    }
    private void ActivateGoal(object sender, EventArgs e){
        goalIsActive = true;
    }
    private void DeactivateGoal(object sender, EventArgs e){
        goalIsActive = false;
    }
    private void OnTriggerEnter(Collider other){
        if(other.tag == "puck" && goalIsActive){
            goalIsActive = false;
            Debug.Log($"Goal scored");
            if(gameObject.tag == "homeNet"){
                awayGoalTrigger?.Invoke(this, EventArgs.Empty);
            }
            else{
                homeGoalTrigger?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
