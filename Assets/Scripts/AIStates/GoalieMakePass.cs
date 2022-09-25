using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalieMakePass : AbstractAIState
{

    public static new string StateName = "GoalieMakePass";
    Vector3 targetPoint;
    Vector3 aimTargetPosition;
    bool willPass;
    int moveDirection;
    // When the goalie gets possession they should set up a pass
    // goalie will choose top middle or bottom of the ice to send it
    // will move the skater towards that point
    // will send the puck when they get close
    public GoalieMakePass(AIPlayerController aiPlayerController) : base(aiPlayerController)
    {
    }
    public override void OnEnter()
    {
        // choose pass or shot
        willPass = Random.Range(0, 2) > 0;
        moveDirection = Random.Range(0, 3);
    }

    public override void OnExit()
    {
    }
    private void TriggerMovePuck(){
        if(willPass){
            aiPlayerController.CommandPass();
        } else {
            aiPlayerController.CommandShot();
        }
    }
    private Vector3 GetAimVector(){
        Vector3 aim = (aimTargetPosition - goaltender.transform.position);
        aim.y = 0;
        return aim.normalized;
    }
    private void SetPointer(){
        switch (moveDirection){
            case 0:
                aimTargetPosition = new Vector3(0,0,9.25f);
                break;
            case 1:
                aimTargetPosition = Vector3.zero;
                break;
            case 2:
                aimTargetPosition = new Vector3(0,0,-9.25f);
                break;
            default:
                break;
        }
        Move(GetAimVector());
    }
    public override void OnUpdate()
    {
        // do we still have possession?
        if (!goaltender.GetComponent<TeamMember>().hasPosession)
        {
            aiPlayerController.ChangeState(ChaseState.StateName);
            return;
        } else {
            // set movement pointers
            SetPointer();
            // skater is past blue line
            if(Vector3.Distance(goaltender.myNet.transform.position, selectedSkater.transform.position) > 12){
                TriggerMovePuck();
            }
        }
        //begin windup
        // if the skater is past the blue line, make the pass
        // point the skater toward the top mid or bottom
    }
}
