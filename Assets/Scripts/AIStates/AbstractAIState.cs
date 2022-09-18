using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractAIState
{
    public static string StateName;
    protected AIPlayerController aiPlayerController;
    protected Vector3 movementPointer;
    protected Skater selectedSkater;
    protected TeamMember selectedTeamMember;
    protected Goaltender goaltender;
    protected Skater opponentSkater;
    protected TeamMember opponentTeamMember;
    protected Goaltender opponentGoaltender;
    protected Transform puckTransform;
    protected float timeSinceLastUpdate;
    protected AbstractAIState(AIPlayerController aiPlayerController)
    {
        this.aiPlayerController = aiPlayerController;

        selectedSkater = aiPlayerController.selectedSkater;
        goaltender = aiPlayerController.goaltender;
        opponentSkater = aiPlayerController.opponentSkater;
        opponentGoaltender = aiPlayerController.opponentGoaltender;
        selectedTeamMember = aiPlayerController.selectedTeamMember;
        opponentTeamMember = aiPlayerController.opponentTeamMember;
        timeSinceLastUpdate = aiPlayerController.AIUpdateTime;
    }
    public abstract void OnEnter();
    public abstract void OnExit();
        
    public virtual void OnUpdate(){
        timeSinceLastUpdate += Time.deltaTime;

        if(timeSinceLastUpdate < aiPlayerController.AIUpdateTime){
            return;
        }

        timeSinceLastUpdate = 0;
    }
    protected Vector3 GetMovementPointer(Vector3 targetPosition)
    {
        Vector3 movementVector = targetPosition - selectedTeamMember.transform.position;
        // zero the y axis (only moving in x, z)
        movementVector.y = 0;
        // normalize the vector
        movementVector.Normalize();
        // Set movementPointer to the normalized vector
        return movementVector;
    }
    protected Vector3 GetRandomAttackPointer()
    {
        return new Vector3(
            opponentGoaltender.myNet.transform.position.x + 5,
            0,
            Random.Range(-7f, 7f)
        );
    }
    protected Vector3 GetRandomPointInOpponentGoal()
    {
        Bounds bounds = opponentGoaltender.myNet.GetComponent<Collider>().bounds;

        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }
    protected Vector3 GetGoalTenderMovementPointer()
    {
        Vector3 netToPuck = puckTransform.position - goaltender.myNet.transform.position;
        Vector3 netToGoalie = goaltender.transform.position - goaltender.myNet.transform.position;
        Vector3 goalieToPuck = puckTransform.position - goaltender.transform.position;
        netToPuck.y = 0;
        netToGoalie.y = 0;
        goalieToPuck.y = 0;
        Vector3 angleCross = Vector3.Cross(goalieToPuck, netToPuck);
        Debug.Log($"cross: {angleCross.y}");
        movementPointer = Vector3.Cross(netToGoalie, Vector3.up) * (angleCross.y/Mathf.Abs(angleCross.y)); //(netToPuckYAngle - goalieToPuckYAngle)/(Mathf.Abs(netToPuckYAngle - goalieToPuckYAngle));
        //movementPointer = movementPointer.normalized + goalieToPuck.normalized;
        Debug.Log($"net movement: {movementPointer}");
        return movementPointer.normalized;
    }
    protected void Move(Vector3 targetPoint)
    {
        selectedSkater.SetPointers(targetPoint);
        goaltender.SetPointers(targetPoint);
    }
    protected bool ShouldControlGoaltender()
    {
        // other skater has posession on your side of the ice
        // Nobody has posession, puck is going towards your net
        // other player or goalie is winding up a shot
        return (
            (opponentTeamMember.hasPosession && (Vector3.Distance(goaltender.myNet.transform.position, opponentSkater.transform.position) < 16))
            || (!aiPlayerController.SomeoneHasPosession() && aiPlayerController.PuckIsGoingToMyNet())
        );
    }
    protected bool BehindGoal()
    {
        return (selectedSkater.transform.position.x < aiPlayerController.homeGoalOrigin.position.x || selectedSkater.transform.position.x > aiPlayerController.awayGoalOrigin.position.x) && Mathf.Abs(selectedSkater.transform.position.z) < 4;
    }
}
