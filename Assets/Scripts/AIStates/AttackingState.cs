using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingState : AbstractAIState
{

    public static new string StateName = "Attacking";
    Vector3 targetPoint;

    public AttackingState(AIPlayerController aiPlayerController) : base(aiPlayerController)
    {
    }

    public override void OnEnter()
    {
        // make an attack route (waypoints, shot position)
    }

    public override void OnExit()
    {
        // reset the attack route
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (!selectedTeamMember.hasPosession)
        {
            aiPlayerController.ChangeState(ChaseState.StateName);
            return;
        }
        if(BehindGoal()){
            targetPoint = selectedSkater.transform.position;
            targetPoint.z = targetPoint.z + 4 * (targetPoint.z > 0 ? 1 : -1);
            targetPoint = GetMovementPointer(targetPoint);
            return;
        }
        if (Vector3.Distance(opponentGoaltender.myNet.transform.position, selectedSkater.transform.position) < aiPlayerController.AIShotDistance)
        {
            aiPlayerController.ChangeState(ShootingState.StateName);
            return;
        }
        targetPoint = GetMovementPointer(GetRandomAttackPointer());
        Move(targetPoint);
    }


}
