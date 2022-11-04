using System;
using UnityEngine;

public class CountGoals : StateMachineBehaviour
{
    private Animator masterStateMachine;
    public static EventHandler<EventArgs> homeGoalScored;
    public static EventHandler<EventArgs> awayGoalScored;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log($"This state counts goals");
        Goal.awayGoalTrigger += AwayGoalScored;
        Goal.homeGoalTrigger += HomeGoalScored;
        masterStateMachine = animator;
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
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Goal.awayGoalTrigger -= AwayGoalScored;
        Goal.homeGoalTrigger -= HomeGoalScored;
    }
}
