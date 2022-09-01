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

    protected Vector3 GetGoalTenderMovementPointer(Vector3 targetPosition)
    {
        Vector3 goalToPuck = targetPosition - goaltender.myNet.transform.position;
        // zero the y axis (only moving in x, z)
        goalToPuck.y = 0;
        // normalize the vector
        goalToPuck.Normalize();

        Vector3 movementVector = goalToPuck - goaltender.transform.position;
        // zero the y axis (only moving in x, z)
        movementVector.y = 0;
        // normalize the vector
        movementVector.Normalize();
        // Set movementPointer to the normalized vector
        return movementVector;
    }

    protected void Move(Vector3 targetPoint)
    {
        selectedSkater.SetPointers(targetPoint);
        goaltender.SetPointers(targetPoint);
    }

    protected bool ShouldControlGoaltender()
    {

        if (Vector3.Distance(puckTransform.position, goaltender.transform.position) < (aiPlayerController.AIShotDistance + 1))
        {

            if (opponentTeamMember.hasPosession)
            {
                return true;
            }

            RaycastHit hitInfo;
            if (Physics.Raycast(puckTransform.position, puckTransform.GetComponent<Rigidbody>().velocity, out hitInfo, 10f))
            {
                if (hitInfo.collider.tag == "homeNet" || hitInfo.collider.tag == "awayNet")
                {
                    return true;
                }
            }

        }
        return false;
    }

    protected bool BehindGoal()
    {
        return (selectedSkater.transform.position.x < aiPlayerController.homeGoalOrigin.position.x || selectedSkater.transform.position.x > aiPlayerController.awayGoalOrigin.position.x) && Mathf.Abs(selectedSkater.transform.position.z) < 4;
    }

}
