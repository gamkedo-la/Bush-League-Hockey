using System;
using System.Collections;
using UnityEngine;

public class MainMenuState : StateMachineBehaviour
{
    private GameSystem gameSystem;
    public static EventHandler<EventArgs> dropPuck;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Main menu start");
        gameSystem = FindObjectOfType<GameSystem>();
        dropPuck.Invoke(this, EventArgs.Empty); // triggers the AI to start playing
        Goal.awayGoalScored += GoalScored;
    }
    public void GoalScored(object sender, EventArgs e)
    {
        gameSystem.CasualGoalScored();
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Main menu exit");
        //Main menu is disabled
        Goal.awayGoalScored -= GoalScored;
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
