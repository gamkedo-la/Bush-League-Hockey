using System;
using UnityEngine;
public class GameOnState : StateMachineBehaviour
{
    private GameSystem gameSystem;
    private TimeProvider gameTimeProvider;
    private Animator masterStateMachine;
    [SerializeField] GameplayState currentGameplayState;
    public static EventHandler<EventArgs> onStateEnter;
    public static EventHandler<EventArgs> onStateExit;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Game On");
        gameSystem = FindObjectOfType<GameSystem>();
        masterStateMachine = animator;
        gameTimeProvider = gameSystem.timeManager.gameTime;
        onStateEnter?.Invoke(this, EventArgs.Empty);
        PlayAreaBoundary.outOfBounds += OutOfBounds;
    }
    private void OutOfBounds(object sender, EventArgs e){
        Debug.Log($"Out of Bounds");
        masterStateMachine.SetTrigger("OutOfBounds");
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("GameOn Update");
        //Update game timer
        currentGameplayState.gameClockTime -= gameTimeProvider.fixedDeltaTime;
        // instant replay frame recording
        if(currentGameplayState.gameClockTime <= 0){
            gameSystem.DeactivateGoals();
            //change state to period end
            animator.SetTrigger("EndOfPeriod");
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Game Stopped");
        animator.SetBool("GameOn", false);
        onStateExit?.Invoke(this, EventArgs.Empty);
        //save GameplayState
    }
}
