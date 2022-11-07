using System;
using UnityEngine;
public class BeginGameState : StateMachineBehaviour
{
    [SerializeField] GameplayState currentGameplayState;
    [SerializeField] GameplayState defaultGameplayState; // TODO make this SO
    private GameSystem gameSystem;
    private AudioManager audioManager;
    public static EventHandler<EventArgs> onStateEnter;
    public static EventHandler<EventArgs> onStateExit;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Begin Game");
        currentGameplayState.currentPeriod = 1;
        currentGameplayState.homeScore = 0;
        currentGameplayState.awayScore = 0;
        currentGameplayState.gameClockTime = 15f;
        gameSystem = FindObjectOfType<GameSystem>();
        gameSystem.SetPlayersToTeams();
        gameSystem.SetAllActionMapsToPlayer();
        onStateEnter?.Invoke(this, EventArgs.Empty);
        animator.SetBool("GameOn", false);
        animator.SetBool("GameOn", false);
        animator.SetBool("SuddenDeath", false);
        animator.SetTrigger("FaceOff");
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       Debug.Log($"Begin Game Exit");
    }
}
