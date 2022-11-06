using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCheck : StateMachineBehaviour
{
    [SerializeField] GameplayState currentGameplayState;
    [SerializeField] [Range(0.2f, 6f)] float postWhistleTime;
    private float  entryTime;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        entryTime = Time.time;
        // Period 3?
        // Score?
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(Time.time - entryTime > postWhistleTime){
            if(currentGameplayState.gameClockTime <= 0){
                if(currentGameplayState.homeScore == currentGameplayState.awayScore){
                    animator.SetBool("SuddenDeath", true);
                }
                else{
                    animator.SetTrigger("EndOfGame");
                }
            } else{
                animator.SetTrigger("FaceOff");
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
