using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AIPlayerController : MonoBehaviour
{
    #region AIParameters
    public float AIUpdateTime = 0.5f;
    public float AIShotDistance = 10f;
    public float AIBodyCheckDistance = 7f;

    #endregion
    private GameSystem gameSystem;
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
    private void Awake() {
        gameSystem = FindObjectOfType<GameSystem>();
        puckTransform = gameSystem.puckObject.transform;
    }
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
    public bool SomeoneHasPosession(){
        bool hasPosession = false;
        foreach (TeamMember tm in gameSystem.allTeamMemberScripts)
        {
            if (tm.hasPosession)
            {
                hasPosession = true;
                break;
            }
        }
        return hasPosession;
    }
    public bool PuckIsGoingToMyNet(){
        // Debug.Log(
        //     $"Line:{(goaltender.myNet.transform.position - puckTransform.position).normalized}\n"
        //     + $"velocity:{puckTransform.gameObject.GetComponent<Rigidbody>().velocity}"
        // );
        float angle = Vector3.Angle(goaltender.myNet.transform.position - puckTransform.position, puckTransform.gameObject.GetComponent<Rigidbody>().velocity);
        return angle < 35 && puckTransform.gameObject.GetComponent<Rigidbody>().velocity.magnitude > 2f;
    }
    private void HandleMovement()
    { // based on 'Skater' -> MovementInputHandler()
        // move in the given direction
        selectedSkater.SetPointers(movementPointer);
    }
    private IEnumerator Shoot()
    {
        StartCoroutine(selectedSkater.WindUpShot());
        StartCoroutine(goaltender.WindUpShot());
        yield return null;
        selectedSkater.ShootPuck();
        goaltender.ShootPuck();
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
        StartCoroutine(selectedSkater.WindUpBodyCheck());
        yield return new WaitForSeconds(0.1f);
        selectedSkater.DeliverBodyCheck();
    }
    public void CommandPass()
    {
        StartCoroutine(Pass());
    }
    private IEnumerator Pass()
    {
        StartCoroutine(selectedSkater.WindUpPass());
        StartCoroutine(goaltender.WindUpPass());
        yield return new WaitForSeconds(0.4f);
        selectedSkater.PassPuck();
        goaltender.PassPuck();
    }
    void Start()
    {
        stateDictionary = new Dictionary<string, AbstractAIState>();
        stateDictionary.Add("Waiting", new WaitingState(this));
        stateDictionary.Add("Chase", new ChaseState(this));
        stateDictionary.Add("Attacking", new AttackingState(this));
        stateDictionary.Add("GoalieMakePass", new GoalieMakePass(this));
        stateDictionary.Add("Shooting", new ShootingState(this));
        stateDictionary.Add("GoalieDefend", new GoalieDefendState(this));
        currentState = stateDictionary[WaitingState.StateName];
        currentState.OnEnter();
    }
    private void OnEnable()
    {
        GameSystem.dropPuck += DropPuck;
        TeamMember.takenPosession += PosessionChanged;
    }
    private void OnDisable()
    {
        GameSystem.dropPuck -= DropPuck;
        TeamMember.takenPosession -= PosessionChanged;
    }
    private void DropPuck(object sender, EventArgs e)
    {
        print("DropPuck detected");
        puckTransform = gameSystem.puckObject.transform;
        ChangeState(ChaseState.StateName);
    }
    private bool IsOnMyTeam(TeamMember tm){
        return(
            (tm.gameObject.tag.Contains("home") && selectedTeamMember.gameObject.tag.Contains("home"))
            || (tm.gameObject.tag.Contains("away") && selectedTeamMember.gameObject.tag.Contains("away"))
        );
    }
    private void PosessionChanged(object sender, EventArgs e)
    {
        // did my team get it?
        if(IsOnMyTeam(sender as TeamMember)){
            if((sender as TeamMember).tag.Contains("Goaltender")){
                ChangeState(GoalieMakePass.StateName);
            }
            if((sender as TeamMember).tag.Contains("Skater")){
                ChangeState(AttackingState.StateName);
            }
        } else {
            ChangeState(ChaseState.StateName);
        }
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
    }
}
