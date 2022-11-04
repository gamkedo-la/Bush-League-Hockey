using System;
using UnityEngine;
public class GameOnState : StateMachineBehaviour
{
    public static EventHandler<EventArgs> onStateEnter;
    public static EventHandler<EventArgs> onStateUpdate;
    public static EventHandler<EventArgs> onStateExit;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Game On");
        onStateEnter?.Invoke(this, EventArgs.Empty);
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("GameOn Update");
        onStateUpdate?.Invoke(this, EventArgs.Empty);
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        onStateExit?.Invoke(this, EventArgs.Empty);
        //save GameplayState
    }
}
