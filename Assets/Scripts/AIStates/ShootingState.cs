using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingState : AbstractAIState
{

    public static new string StateName = "Shooting";

    float positioningTime = 0.1f;
    float timeSpentPositioning = 0f;
    bool positioning = false;
    Vector3 shotTarget;

    public ShootingState(AIPlayerController aiPlayerController) : base(aiPlayerController)
    {
    }

    public override void OnEnter()
    {
        timeSpentPositioning = 0;
        positioning = false;
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        if (!positioning)
        {
            shotTarget = GetRandomPointInOpponentGoal();
            selectedSkater.SetPointers(GetMovementPointer(shotTarget));
            positioning = true;
            timeSpentPositioning = 0;
            return;
        }

        timeSpentPositioning += Time.deltaTime;

        if(timeSpentPositioning < positioningTime){
            return;
        }

        aiPlayerController.CommandShot();
        aiPlayerController.ChangeState(ChaseState.StateName);
    }
}
