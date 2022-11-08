using System;
using UnityEngine;
public class InstantReplayState : StateMachineBehaviour
{
    [SerializeField] GameplayState currentGameplayState;
    private Animator masterStateMachine;
    public static EventHandler<EventArgs> onStateEnter;
    public static EventHandler<EventArgs> onStateUpdate;
    public static EventHandler<EventArgs> onStateExit;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       // Replay begins
       masterStateMachine = animator;
       onStateEnter?.Invoke(this, EventArgs.Empty);
       InstantReplay.replayEnd += ReplayDone;
    }
    private void ReplayDone(object sender, EventArgs e)
    {
        masterStateMachine.SetBool("InstantReplay", false);
        masterStateMachine.SetTrigger("WinCheck");
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       onStateUpdate?.Invoke(this, EventArgs.Empty);
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       masterStateMachine.SetBool("InstantReplay", false);
    }
}
