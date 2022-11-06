using System;
using UnityEngine;

public class SuddenDeathMessage : StateMachineBehaviour
{
    private GameSystem gameSystem;
    public static EventHandler<EventArgs> onStateEnter;
    public static EventHandler<EventArgs> onStateExit;
    private float entryTime;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        entryTime = Time.time;
        gameSystem = FindObjectOfType<GameSystem>();
        onStateEnter?.Invoke(this, EventArgs.Empty);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Time.time - entryTime > 4)
        {
            animator.SetTrigger("FaceOff");
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
