using System;
using UnityEngine;

public class ShowScores : StateMachineBehaviour
{
    [SerializeField] [Range(0.2f, 6f)] float delayBetweenScores;
    public static EventHandler<EventArgs> onStateEnter;
    public static EventHandler<EventArgs> onStateExit;
    float entryTime;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        onStateEnter?.Invoke(this, EventArgs.Empty);
        entryTime = Time.time;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(Time.time - entryTime > delayBetweenScores){
            animator.SetTrigger("BigCelebration");
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
