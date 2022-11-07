using System;
using UnityEngine;
public class BigCelebration : StateMachineBehaviour
{
    [SerializeField] TimeProvider timeProvider;
    [SerializeField] [Range(0.2f, 6f)] float delayBeforeCelebration;
    [SerializeField] [Range(4f, 12f)] float delayBetweenCelebration;
    public static EventHandler<EventArgs> onStateEnter;
    public static EventHandler<EventArgs> onStateUpdate;
    public static EventHandler<EventArgs> celebrate;
    public static EventHandler<EventArgs> onStateExit;
    private float delayCountdown = 0f;
    private float celebrateCountdown = 0f;
    float entryTime;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       delayCountdown = delayBeforeCelebration;
       onStateEnter?.Invoke(this, EventArgs.Empty);
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(celebrateCountdown <= 0 && delayCountdown <= 0){
            celebrateCountdown = delayBetweenCelebration;
            celebrate?.Invoke(this, EventArgs.Empty);
        }
        onStateUpdate?.Invoke(this, EventArgs.Empty);
        delayCountdown -= timeProvider.fixedDeltaTime;
        celebrateCountdown -= timeProvider.fixedDeltaTime;
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        // stuff
    }
}
