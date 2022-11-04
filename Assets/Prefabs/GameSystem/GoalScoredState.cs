using System;
using UnityEngine;

public class GoalScoredState : StateMachineBehaviour
{
    public static EventHandler<EventArgs> onStateEnter;
    public static EventHandler<EventArgs> onStateExit;
    [Range(0.5f, 4f)] public float idleTime;
    private float startTime;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // stay here and idle for a few seconds
        startTime = Time.time;
        onStateEnter?.Invoke(this, EventArgs.Empty);
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // timer ticks
        if(Time.time - startTime >= idleTime){
            animator.SetTrigger("InstantReplay");
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       onStateExit?.Invoke(this, EventArgs.Empty);
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
