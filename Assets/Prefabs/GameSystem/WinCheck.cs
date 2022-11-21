using System;
using UnityEngine;
public class WinCheck : StateMachineBehaviour
{
    [SerializeField] GameplayState currentGameplayState;
    [SerializeField] [Range(0.2f, 6f)] float postWhistleTime;
    public static EventHandler<EventArgs> onStateEnter;
    private float countdownTime;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        countdownTime = postWhistleTime;
        // Period 3?
        // Score?
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(countdownTime > 0){
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
            countdownTime -= Time.fixedDeltaTime;
        }
    }
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       animator.ResetTrigger("WinCheck");
    }
}
