using System;
using System.Collections;
using UnityEngine;
public class MainMenuState : StateMachineBehaviour
{
    private GameSystem gameSystem;
    public static EventHandler<EventArgs> onEnter;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Main menu start");
        gameSystem = FindObjectOfType<GameSystem>();
        Goal.awayGoalScored += GoalScored;
        Goal.homeGoalScored += GoalScored;
        onEnter.Invoke(this, EventArgs.Empty);
        // Setup the AI to play casually
        gameSystem.SetPlayersToTeams();
        gameSystem.SetupPlayersForFaceOff();
        gameSystem.PuckToCenterOrigin();
        gameSystem.ActivateGoals();
    }
    public void GoalScored(object sender, EventArgs e)
    {
        gameSystem.CasualGoalScored();
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("Main menu update");
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Main menu exit");
        Goal.awayGoalScored -= GoalScored;
        Goal.homeGoalScored -= GoalScored;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
