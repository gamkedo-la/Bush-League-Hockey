using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIPlayerController : MonoBehaviour
{

    #region AIParameters

    public float AIUpdateTime = 0.5f;
    public float AIShotDistance = 10f;
    public float AIBodyCheckDistance = 1f;

    #endregion

    private Vector3 movementPointer;
    public Skater selectedSkater;
    public TeamMember selectedTeamMember;
    public Goaltender goaltender;

    public Skater opponentSkater;
    public TeamMember opponentTeamMember;
    public Goaltender opponentGoaltender;

    private Transform puckTransform;
    public Transform homeGoalOrigin, awayGoalOrigin;

    AbstractAIState currentState;

    Dictionary<string, AbstractAIState> stateDictionary;
    private bool isDoingSomething = false;

    public Transform PuckTransform { get => puckTransform; }

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


        gameObject.name = "AI Controller Home";

        InitializeTeamObjects();
    }
    public void SetToAwayTeam()
    {
        selectedSkater = GameObject.FindWithTag("awaySkater").GetComponent<Skater>();
        goaltender = GameObject.FindWithTag("awayGoaltender").GetComponent<Goaltender>();
        opponentSkater = GameObject.FindWithTag("homeSkater").GetComponent<Skater>();
        opponentGoaltender = GameObject.FindWithTag("homeGoaltender").GetComponent<Goaltender>();

        gameObject.name = "AI Controller Away";

        InitializeTeamObjects();
    }
    private void InitializeTeamObjects()
    {
        selectedTeamMember = selectedSkater.gameObject.GetComponent<TeamMember>();
        opponentTeamMember = opponentSkater.gameObject.GetComponent<TeamMember>();
        homeGoalOrigin = GameObject.Find("HomeGoalOrigin").transform;
        awayGoalOrigin = GameObject.Find("AwayGoalOrigin").transform;
        goaltender.FindMyNet();
    }
    private void HandleMovement()
    { // based on 'Skater' -> MovementInputHandler()
        // move in the given direction
        selectedSkater.SetPointers(movementPointer);
    }

    private IEnumerator Shoot()
    {
        StartCoroutine(selectedSkater.WindUpShot());
        yield return null;
        selectedSkater.ShootPuck();
    }

    public void CommandShot()
    {
        StartCoroutine(Shoot());
    }

    public void CommandBodyCheck()
    {
        StartCoroutine(Bodycheck());
    }

    private IEnumerator Bodycheck()
    {
        selectedSkater.WindUpBodyCheck();
        yield return new WaitForSeconds(0.3f);
        selectedSkater.DeliverBodyCheck();
    }

    void Start()
    {
        stateDictionary = new Dictionary<string, AbstractAIState>();
        stateDictionary.Add("Waiting", new WaitingState(this));
        stateDictionary.Add("Chase", new ChaseState(this));
        stateDictionary.Add("Attacking", new AttackingState(this));
        stateDictionary.Add("Shooting", new ShootingState(this));
        stateDictionary.Add("GoalieDefend", new GoalieDefendState(this));

        currentState = stateDictionary[WaitingState.StateName];
        currentState.OnEnter();
    }

    private void OnEnable()
    {
        PuckScript.onPuckSpawned += PuckSpawned;
    }

    private void OnDisable()
    {
        PuckScript.onPuckSpawned -= PuckSpawned;
    }

    private void PuckSpawned(object sender, EventArgs e)
    {
        puckTransform = (sender as PuckScript).transform;
        ChangeState(ChaseState.StateName);
    }

    void Update()
    {

        currentState.OnUpdate();
    }

    public void ChangeState(string newState)
    {
        currentState.OnExit();
        print($"{gameObject.name} changed from {currentState} to {newState}");
        if (stateDictionary.TryGetValue(newState, out AbstractAIState state))
        {
            currentState = state;
            currentState.OnEnter();
            return;
        }


        Debug.Log($"{newState}: Not found in StateDictionary");
    }
}
