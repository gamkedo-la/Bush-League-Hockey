using System;
using UnityEngine;

public class OutOfBoundsListener : StateMachineBehaviour
{
    private Animator masterStateMachine;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayAreaBoundary.outOfBounds += OutOfBounds;
        masterStateMachine = animator;
    }
    private void OutOfBounds(object sender, EventArgs e)
    {
        Debug.Log($"Out of Bounds");
        masterStateMachine.SetTrigger("OutOfBounds");
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayAreaBoundary.outOfBounds -= OutOfBounds;
    }
}
