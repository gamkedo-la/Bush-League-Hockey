using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingState : AbstractAIState
{

    public static new string StateName = "Waiting";

    public WaitingState(AIPlayerController aiPlayerController) : base(aiPlayerController)
    {
    }

    public override void OnEnter()
    {

    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        Move(Vector3.zero);
    }

    public override void OnExit()
    {

    }
}
