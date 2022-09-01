using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalieDefendState : AbstractAIState
{
    public static new string StateName = "GoalieDefend";

    public GoalieDefendState(AIPlayerController aiPlayerController) : base(aiPlayerController)
    {

    }


    public override void OnEnter()
    {
        puckTransform = aiPlayerController.PuckTransform;
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if(!ShouldControlGoaltender()){
            aiPlayerController.ChangeState(ChaseState.StateName);
        }

        Vector3 targetPoint = GetGoalTenderMovementPointer(puckTransform.position);

        Move(targetPoint);

    }


}
