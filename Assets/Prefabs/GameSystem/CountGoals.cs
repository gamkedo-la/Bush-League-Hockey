using System;
using UnityEngine;

public class CountGoals : StateMachineBehaviour
{
    [SerializeField] GameplayState currentGameplayState;
    private Animator masterStateMachine;
    public static EventHandler<EventArgs> onStateEnter;
    public static EventHandler<EventArgs> onStateExit;
    public static EventHandler<EventArgs> homeGoalScored;
    public static EventHandler<EventArgs> awayGoalScored;
    private float saveCooldownInitial = 0.4f;
    private float saveCooldownTime = 0; 
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Goal.awayGoalTrigger += AwayGoalScored;
        Goal.homeGoalTrigger += HomeGoalScored;
        // subscribe to stats counting events
        PuckScript.saveHomeTeam += SaveHomeTeam;
        PuckScript.saveAwayTeam += SaveAwayTeam;
        Skater.homeHitLanded += HomeHitLanded;
        Skater.awayHitLanded += AwayHitLanded;
        masterStateMachine = animator;
        onStateEnter?.Invoke(this, EventArgs.Empty);
    }
    private void AwayGoalScored(object sender, EventArgs e)
    {
        Debug.Log($"AwayGoal");
        awayGoalScored?.Invoke(this, EventArgs.Empty);
        masterStateMachine.SetTrigger("GoalScored");
    }
    private void HomeGoalScored(object sender, EventArgs e)
    {
        Debug.Log($"HomeGoal");
        homeGoalScored?.Invoke(this, EventArgs.Empty);
        masterStateMachine.SetTrigger("GoalScored");
    }
    public void CountSave(bool homeSave){
        saveCooldownTime = saveCooldownInitial;
        if(homeSave){
            currentGameplayState.homeSaves++;
        } else {
            currentGameplayState.awaySaves++;
        }
    }
    private void SaveHomeTeam(object sender, EventArgs e)
    {
        if(saveCooldownTime <= 0){
            CountSave(true);
        }
    }
    private void SaveAwayTeam(object sender, EventArgs e)
    {
        if(saveCooldownTime <= 0){
            CountSave(false);
        }
    }
    private void HomeHitLanded(object sender, EventArgs e)
    {
        currentGameplayState.homeHits++;
    }
    private void AwayHitLanded(object sender, EventArgs e)
    {
        currentGameplayState.awayHits++;
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if(saveCooldownTime > 0){
            saveCooldownTime -= Time.fixedDeltaTime;
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Goal.awayGoalTrigger -= AwayGoalScored;
        Goal.homeGoalTrigger -= HomeGoalScored;
        PuckScript.saveHomeTeam -= SaveHomeTeam;
        PuckScript.saveHomeTeam -= SaveAwayTeam;
        onStateExit?.Invoke(this, EventArgs.Empty);
    }
}
