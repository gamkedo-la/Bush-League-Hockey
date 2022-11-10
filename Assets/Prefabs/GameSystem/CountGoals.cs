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
    private float hitCooldownInitial = 0.2f;
    private float saveCooldownTime = 0; 
    private float hitCooldownTime = 0;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Goal.awayGoalTrigger += AwayGoalScored;
        Goal.homeGoalTrigger += HomeGoalScored;
        // subscribe to stats counting events
        PuckScript.saveHomeTeam += SaveHomeTeam;
        PuckScript.saveAwayTeam += SaveAwayTeam;
        Skater.homeHitLanded += HomeHitLanded;
        Skater.awayHitLanded += AwayHitLanded;
        Skater.homePass += HomePass;
        Skater.awayPass += AwayPass;
        Goaltender.homePass += HomePass;
        Goaltender.awayPass += AwayPass;
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
    public void CountHit(bool homeHit){
        saveCooldownTime = saveCooldownInitial;
        if(homeHit){
            currentGameplayState.homeHits++;
        } else {
            currentGameplayState.awayHits++;
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
        if(hitCooldownTime <= 0){
            CountHit(true);
        }
    }
    private void AwayHitLanded(object sender, EventArgs e)
    {
        if(hitCooldownTime <= 0){
            CountHit(false);
        }
    }
    private void HomePass(object sender, EventArgs e)
    {
        currentGameplayState.homePasses++;
    }
    private void AwayPass(object sender, EventArgs e)
    {
        currentGameplayState.awayPasses++;
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if(saveCooldownTime > 0){
            saveCooldownTime -= Time.fixedDeltaTime;
        }
        if(hitCooldownTime > 0){
            hitCooldownTime -= Time.fixedDeltaTime;
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Goal.awayGoalTrigger -= AwayGoalScored;
        Goal.homeGoalTrigger -= HomeGoalScored;
        PuckScript.saveHomeTeam -= SaveHomeTeam;
        PuckScript.saveAwayTeam -= SaveAwayTeam;
        Skater.homeHitLanded -= HomeHitLanded;
        Skater.awayHitLanded -= AwayHitLanded;
        Skater.homePass -= HomePass;
        Skater.awayPass -= AwayPass;
        Goaltender.homePass -= HomePass;
        Goaltender.awayPass -= AwayPass;
        onStateExit?.Invoke(this, EventArgs.Empty);
    }
}
