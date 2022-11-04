using System;
using UnityEngine;

public class RegulationState : StateMachineBehaviour
{
    [SerializeField] GameplayState currentGameplayState;
    private GameSystem gameSystem;
    private TimeProvider gameTimeProvider;
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        gameSystem = FindObjectOfType<GameSystem>();
        gameTimeProvider = gameSystem.timeManager.gameTime;
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        currentGameplayState.gameClockTime -= gameTimeProvider.fixedDeltaTime;
        if(currentGameplayState.gameClockTime <= 0){
            animator.SetTrigger("EndOfPeriod");
        }
    }
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
