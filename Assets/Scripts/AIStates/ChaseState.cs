using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : AbstractAIState
{

    public static new string StateName = "Chase";

    public ChaseState(AIPlayerController aiPlayerController) : base(aiPlayerController)
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

        if (selectedTeamMember.hasPosession)
        {
            aiPlayerController.ChangeState(AttackingState.StateName);
            return;
        }

        if (ShouldControlGoaltender())
        {
            aiPlayerController.ChangeState(GoalieDefendState.StateName);
        }

        if (Vector3.Distance(opponentSkater.transform.position, selectedSkater.transform.position) < aiPlayerController.AIBodyCheckDistance){
            aiPlayerController.CommandBodyCheck();
        }

        Vector3 targetPoint = GetMovementPointer(puckTransform.position);

        if (BehindGoal())
        {
            targetPoint = selectedSkater.transform.position;
            targetPoint.z = targetPoint.z + 4 * (targetPoint.z > 0 ? 1 : -1);
            targetPoint = GetMovementPointer(targetPoint);
        }

        Move(targetPoint);

    }

    
}
