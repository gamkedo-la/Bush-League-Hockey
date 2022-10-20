using System;
using UnityEngine;
public class GameOnState : StateMachineBehaviour
{
    private GameSystem gameSystem;
    public static EventHandler<EventArgs> onStateEnter;
    public static EventHandler<EventArgs> onStateExit;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Game On");
        gameSystem = FindObjectOfType<GameSystem>();
        Goal.awayGoalScored += AwayGoalScored;
        Goal.homeGoalScored += HomeGoalScored;
        onStateEnter.Invoke(this, EventArgs.Empty);
    }
    public void AwayGoalScored(object sender, EventArgs e)
    {
        gameSystem.GoalScored(true);
    }
    public void HomeGoalScored(object sender, EventArgs e)
    {
        gameSystem.GoalScored(false);
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("GameOn Update");
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Game Stopped");
        Goal.awayGoalScored -= AwayGoalScored;
        Goal.homeGoalScored -= HomeGoalScored;
        onStateExit.Invoke(this, EventArgs.Empty);
        // save GameplayState
    }
}
