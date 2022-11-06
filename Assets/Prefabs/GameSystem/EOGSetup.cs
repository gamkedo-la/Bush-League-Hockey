using System;
using UnityEngine;

public class EOGSetup : StateMachineBehaviour
{
    [SerializeField] [Range(0.2f, 6f)] float delayBeforeScores;
    public static EventHandler<EventArgs> onStateEnter;
    float entryTime;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        onStateEnter?.Invoke(this, EventArgs.Empty);
        entryTime = Time.time;
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(Time.time - entryTime > delayBeforeScores){
            animator.SetTrigger("ShowScores");
        }
    }
}
