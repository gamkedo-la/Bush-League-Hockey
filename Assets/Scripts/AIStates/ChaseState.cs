using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ChaseState : AbstractAIState
{
    public static new string StateName = "Chase";
    private static float bodycheckDistance = 5f;
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
    private bool ShouldBodyCheck()
    {
        // within range?
        // opponent is in front?  (angle between forward and line to opponent)
        // opponent isn't knocked down
        return (
            Vector3.Distance(selectedTeamMember.transform.position, opponentTeamMember.transform.position) < bodycheckDistance
            && Vector3.Angle(selectedTeamMember.transform.forward, opponentTeamMember.transform.position - selectedTeamMember.transform.position) < 90
            && !opponentSkater.isKnockedDown
        );
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (selectedTeamMember.hasPosession)
        {
            aiPlayerController.ChangeState(AttackingState.StateName);
            return;
        }
        if (goaltender.GetComponent<TeamMember>().hasPosession){
            aiPlayerController.ChangeState(GoalieMakePass.StateName);
            return;
        }
        if (ShouldControlGoaltender())
        {
            aiPlayerController.ChangeState(GoalieDefendState.StateName);
        }
        if (ShouldBodyCheck()){
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
