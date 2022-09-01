using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIPlayerController : MonoBehaviour
{
    private const float AIUpdateTime = 0.5f;
    private Vector3 movementPointer;
    private Skater selectedSkater;
    private TeamMember selectedTeamMember;
    private Goaltender goaltender;

    private Skater opponentSkater;
    private TeamMember opponentTeamMember;
    private Goaltender opponentGoaltender;

    private Transform puckTransform;

    IEnumerator aiUpdateTime;

    private bool isDoingSomething = false;
    // If one team doesn't have a player, this object is assigned to that team.
    // Whereas the 'PlayerController' prefab uses player input to trigger functions resulting in behaviour
    // This object will use it's own logic to emulate those same input values.
    // Look at the 'PlayerController' script to see how the pieces in the game are controlled.
    // 'PlayerController' calls 'Skater', 'Goaltender', 'TeamMember' scripts to trigger behavior.
    public void SetToHomeTeam()
    {
        selectedSkater = GameObject.FindWithTag("homeSkater").GetComponent<Skater>();
        goaltender = GameObject.FindWithTag("homeGoaltender").GetComponent<Goaltender>();
        opponentSkater = GameObject.FindWithTag("awaySkater").GetComponent<Skater>();
        opponentGoaltender = GameObject.FindWithTag("awayGoaltender").GetComponent<Goaltender>();

        InitializeTeamObjects();
    }
    public void SetToAwayTeam()
    {
        selectedSkater = GameObject.FindWithTag("awaySkater").GetComponent<Skater>();
        goaltender = GameObject.FindWithTag("awayGoaltender").GetComponent<Goaltender>();
        opponentSkater = GameObject.FindWithTag("homeSkater").GetComponent<Skater>();
        opponentGoaltender = GameObject.FindWithTag("homeGoaltender").GetComponent<Goaltender>();
        InitializeTeamObjects();
    }
    private void InitializeTeamObjects()
    {
        selectedTeamMember = selectedSkater.gameObject.GetComponent<TeamMember>();
        opponentTeamMember = opponentSkater.gameObject.GetComponent<TeamMember>();
        goaltender.FindMyNet();
    }
    private void HandleMovement()
    { // based on 'Skater' -> MovementInputHandler()
        // move in the given direction
        selectedSkater.SetPointers(movementPointer);
    }
    private IEnumerator ChaseDownPuck()
    {

        isDoingSomething = true;
        Debug.Log($"AI pursuing puck");
        while (!selectedTeamMember.hasPosession)
        {

            bool closeToGoal = !selectedTeamMember.hasPosession && Vector3.Distance(opponentSkater.transform.position, goaltender.transform.position) < 10f;

            Vector3 targetPoint = GetMovementPointer(puckTransform.position, selectedSkater.transform);
            if (closeToGoal)
            {
                if (opponentTeamMember.hasPosession)
                {
                    print("A");
                    targetPoint = GetGoalTenderMovementPointer(puckTransform.position);
                }
                else
                {
                    RaycastHit hitInfo;
                    print("B");
                    if (Physics.Raycast(puckTransform.position, puckTransform.GetComponent<Rigidbody>().velocity, out hitInfo, 10f))
                    {
                        print("C");
                        if (hitInfo.collider.tag == "homeNet" || hitInfo.collider.tag == "awayNet")
                        {
                            print("D");
                            targetPoint = GetGoalTenderMovementPointer(puckTransform.position);
                        }
                    }
                }

            }

            selectedSkater.SetPointers(targetPoint);
            goaltender.SetPointers(targetPoint);


            if (Vector3.Distance(opponentSkater.transform.position, selectedSkater.transform.position) < 1.5f)
            {
                selectedSkater.WindUpBodyCheck();
                yield return new WaitForSeconds(0.1f);
                selectedSkater.DeliverBodyCheck();
            }

            yield return new WaitForSeconds(AIUpdateTime); // commit to doing this for a some time
        }
        movementPointer = Vector3.zero;
        isDoingSomething = false;
    }

    private IEnumerator AttackGoal()
    {
        isDoingSomething = true;
        Debug.Log($"AI attacking goal");
        while (selectedTeamMember.hasPosession && Vector3.Distance(opponentGoaltender.myNet.transform.position, selectedSkater.transform.position) > 10f)
        {
            if (goaltender.teamMember.hasPosession)
            {
                goaltender.WindUpPass();
                yield return null;
                goaltender.PassPuck();
            }
            Vector3 targetPoint = GetMovementPointer(GetRandomAttackPointer(), selectedTeamMember.transform);
            selectedSkater.SetPointers(targetPoint);
            goaltender.SetPointers(targetPoint);
            yield return new WaitForSeconds(AIUpdateTime); // commit to doing this for a some time

        }

        movementPointer = Vector3.zero;
        isDoingSomething = false;

        if (selectedTeamMember.hasPosession)
        {
            StartCoroutine(Shoot());
        }

    }

    private IEnumerator Shoot()
    {
        isDoingSomething = true;
        //Look at shot point
        Vector3 shotTarget = GetRandomPointInOpponentGoal();
        selectedSkater.SetPointers(GetMovementPointer(shotTarget, selectedTeamMember.transform));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(selectedSkater.WindUpShot());
        yield return null;
        selectedSkater.ShootPuck();
        isDoingSomething = false;
    }

    void Start()
    {

    }

    private void OnEnable()
    {
        PuckScript.onPuckSpawned += PuckSpawned;
    }

    private void OnDisable()
    {
        PuckScript.onPuckSpawned -= PuckSpawned;
    }

    private void PossessionChange(object sender, bool e)
    {
        isDoingSomething = false;
        StopAllCoroutines();
    }

    private void PuckSpawned(object sender, EventArgs e)
    {
        puckTransform = (sender as PuckScript).transform;
    }

    void Update()
    {

        if (puckTransform == null)
        {
            return;
        }

        // read the situation, decide what to do
        // call coroutines StartCoroutine(DoSomething())
        // am I already doing something?

        if (!isDoingSomething)
        {
            // do I have posession?
            if (!selectedTeamMember.hasPosession)
            {
                StartCoroutine(ChaseDownPuck());
                return;
            }
            // or try to score
            if (!selectedSkater.WindingUp())
                StartCoroutine(AttackGoal());
        }


        // Is there something more important I should do?
        // cancel the current action and respond to it
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "awayNet" || other.tag == "homeNet")
        {
            selectedSkater.SetPointers(GetMovementPointer(GetRandomAttackPointer(), selectedTeamMember.transform));
        }
    }

    private Vector3 GetMovementPointer(Vector3 targetPosition, Transform playerOrGoaltender)
    {
        Vector3 movementVector = targetPosition - playerOrGoaltender.position;
        // zero the y axis (only moving in x, z)
        movementVector.y = 0;
        // normalize the vector
        movementVector.Normalize();
        // Set movementPointer to the normalized vector
        return movementVector;
    }

    private Vector3 GetGoalTenderMovementPointer(Vector3 targetPosition)
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
        Debug.DrawLine(goaltender.transform.position, goaltender.transform.position + movementVector);
        // Set movementPointer to the normalized vector
        return movementVector;
    }

    private Vector3 GetRandomPointInOpponentGoal()
    {
        Bounds bounds = opponentGoaltender.myNet.GetComponent<Collider>().bounds;

        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    private Vector3 GetRandomAttackPointer()
    {
        return new Vector3(
            opponentGoaltender.myNet.transform.position.x + 5,
            0,
            Random.Range(-7f, 7f)
        );
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = selectedTeamMember.isHomeTeam ? Color.green : Color.red;
        Gizmos.DrawSphere(new Vector3(selectedTeamMember.transform.position.x + movementPointer.x, movementPointer.y, selectedTeamMember.transform.position.z + movementPointer.z), 0.2f);
    }
}
