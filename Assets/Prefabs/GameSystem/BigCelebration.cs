using System;
using UnityEngine;

public class BigCelebration : StateMachineBehaviour
{
    [SerializeField] [Range(0.2f, 6f)] float delayBeforeCelebration;
    public static EventHandler<EventArgs> onStateEnter;
    public static EventHandler<EventArgs> celebrate;
    public static EventHandler<EventArgs> onStateExit;
    float entryTime;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       entryTime = Time.time;
       onStateEnter?.Invoke(this, EventArgs.Empty);
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(Time.time - entryTime > delayBeforeCelebration){
            entryTime = Time.time;
            celebrate?.Invoke(this, EventArgs.Empty);
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        // stuff
    }
}
