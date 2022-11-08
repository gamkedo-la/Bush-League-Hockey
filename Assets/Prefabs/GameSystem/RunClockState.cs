using System;
using UnityEngine;

public class RunClockState : StateMachineBehaviour
{
    [SerializeField] GameplayState currentGameplayState;
    public static EventHandler<EventArgs> onStateUpdate;
    public static EventHandler<EventArgs> timerDone;
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
            timerDone?.Invoke(this, EventArgs.Empty);
            animator.SetBool("GameOn", false);
            animator.SetTrigger("WinCheck");
        }
        onStateUpdate?.Invoke(this, EventArgs.Empty);
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