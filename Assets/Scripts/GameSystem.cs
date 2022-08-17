using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
public class GameSystem : MonoBehaviour
{
    [Header("Main Camera")]
    [HideInInspector] public Camera mainCamera;
    private Vector3 cameraPausePosition;
    private Quaternion cameraPauseRotation;
    [SerializeField] [Range(15f, .5f)] float zoom;
    private GameObject focalObject;
    [SerializeField] [Range(0.0f, 0.2f)] float cameraRotationSpeed;
    private Vector3 lineToDesiredTarget;
    private Quaternion desiredCameraRotation;
    [Header("Game Management")]
    private AudioManager audioManager;
    [SerializeField] private GameObject crowdReactionManager;
    [SerializeField] GameObject homeSkater;
    [SerializeField] GameObject awaySkater;
    [SerializeField] GameObject puckPrefab;
    [SerializeField] public Transform homeGoalOrigin;
    [SerializeField] public Transform homeFaceOffOrigin;
    [SerializeField] public Transform awayGoalOrigin;
    [SerializeField] public Transform awayFaceOffOrigin;
    [SerializeField] public Transform puckDropOrigin;
    [SerializeField] public GameObject homeGoaltender;
    [SerializeField] public GameObject awayGoaltender;
    [HideInInspector] public GameObject puckObject;
    [HideInInspector] public Rigidbody puckRigidBody;
    [Header("Controls Management")]
    [HideInInspector] public List<GameObject> localPlayerControllers;
    private int homeScore = 0;
    private int awayScore = 0;
    private bool gameOn = false;
    [Header("Scorekeeping")]
    [SerializeField] public GameObject homeNet;
    [SerializeField] public GameObject awayNet;
    [SerializeField] TextMeshProUGUI homeScoreText;
    [SerializeField] TextMeshProUGUI awayScoreText;
    [Header("Jumbotron Message")]
    [SerializeField] public GameObject GoalScoredDisplay;
    [SerializeField] public GameObject FaceOffMessageDisplay;
    [SerializeField] public GameObject OutOfBoundsMessageDisplay;
    [SerializeField] public GameObject instantReplayController;
    private void Awake(){
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        cameraPausePosition = new Vector3 (0,0,0);
    }
    public bool IsZeroQuaternion(Quaternion q){
        return q.x == 0 && q.y == 0 && q.z == 0 && q.w == 0;
    }
    private IEnumerator TemporaryFaceOffMessage(){
        FaceOffMessageDisplay.SetActive(true);
        yield return new WaitForSeconds(2);
        FaceOffMessageDisplay.SetActive(false);
    }
    private void SetupPlayersForFaceOff(){
        // reset animations windups etc
        homeSkater.GetComponent<Skater>().ResetSkaterActions();
        homeSkater.GetComponent<Skater>().ResetSkaterMotion();
        awaySkater.GetComponent<Skater>().ResetSkaterActions();
        awaySkater.GetComponent<Skater>().ResetSkaterMotion();
        // break posession
        homeSkater.GetComponent<TeamMember>().BreakPosession();
        awaySkater.GetComponent<TeamMember>().BreakPosession();
        homeGoaltender.GetComponent<TeamMember>().BreakPosession();
        awayGoaltender.GetComponent<TeamMember>().BreakPosession();
        // position players
        homeSkater.transform.position = homeFaceOffOrigin.position;
        homeSkater.transform.rotation = homeFaceOffOrigin.rotation;
        awaySkater.transform.position = awayFaceOffOrigin.position;
        awaySkater.transform.rotation = awayFaceOffOrigin.rotation;
        homeGoaltender.transform.position = homeGoalOrigin.position;
        awayGoaltender.transform.position = awayGoalOrigin.position;
    }
    public void DropPuck(){
        GoalScoredDisplay.SetActive(false);
        SetupPlayersForFaceOff();
        StartCoroutine(TemporaryFaceOffMessage());
        if(puckObject){
            puckObject.transform.position = puckDropOrigin.position;
            puckObject.transform.rotation = puckDropOrigin.rotation;
            puckObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            puckObject.GetComponent<TrailRenderer>().Clear();
        }
        else{
            puckObject = Instantiate(puckPrefab, puckDropOrigin.position, puckDropOrigin.rotation);
            puckRigidBody = puckObject.GetComponent<Rigidbody>();
        }
        gameOn = true;
        homeNet.GetComponent<Goal>().GameOn();
        awayNet.GetComponent<Goal>().GameOn();
        audioManager.PlayFaceOffSound();
        focalObject = puckObject;
        // deactivate HUD messages
    }
    private void Start(){
        audioManager.PlayBaseCrowdTrack();
        DropPuck();
    }
    public void JoinNewPlayer(PlayerInput playerInput){
        var newPlayerInput = playerInput.gameObject.GetComponent<PlayerController>();
        Debug.Log($"new input:  {newPlayerInput}");
        if(localPlayerControllers.Count % 2 == 0){
            newPlayerInput.SetIsHomeTeam(false);
        } else{
            newPlayerInput.SetIsHomeTeam(true);
        }
        if(!localPlayerControllers.Contains(playerInput.gameObject)){localPlayerControllers.Add(playerInput.gameObject);}
    }
    private void HandleCameraPositioning(){
        if(puckObject){
            mainCamera.transform.position = new Vector3((puckObject.transform.position.x / 1.75f), mainCamera.transform.position.y, mainCamera.transform.position.z);
        }
    }
    private void HandleCameraFocus(){
        if(focalObject){
            lineToDesiredTarget = Vector3.Normalize(focalObject.transform.position - mainCamera.transform.position);
            desiredCameraRotation = Quaternion.LookRotation(lineToDesiredTarget, Vector3.up);
            mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, desiredCameraRotation, cameraRotationSpeed);
        }
    }
    private void HandleCameraZoom(){
        // total horizontal distance between the players and the puck
        // far zoom: entire rink visible
        // near zoom: TBD
        // (max: rink width, min: everone at center ice)
    }
    private IEnumerator TemporaryGoalMessage(){
        GoalScoredDisplay.SetActive(true);
        yield return new WaitForSeconds(1.2f);
        GoalScoredDisplay.SetActive(false);
    }
    private IEnumerator CelebrateThenReset(){
        yield return StartCoroutine(TemporaryGoalMessage());
        instantReplayController?.GetComponent<InstantReplay>()?.startInstantReplay();
        // point a spotlight on the player who scored
        yield return new WaitForSeconds(3);
        DropPuck();
    }
    public void GoalScored(bool scoredOnHomeNet){
        audioManager.PlayGoalHorn();
        audioManager.PlayCrowdCelebration();
        if(scoredOnHomeNet)
        {
            awayScore++; 
            StartCoroutine(crowdReactionManager.transform.GetComponent<CrowdReactionManagerScriptComponent>().HandleAwayTeamScoringAGoal());
        }
        else
        {
            homeScore++;
            StartCoroutine(crowdReactionManager.transform.GetComponent<CrowdReactionManagerScriptComponent>().HandleHomeTeamScoringAGoal());
        }
        homeScoreText.text = homeScore.ToString();
        awayScoreText.text = awayScore.ToString();
        gameOn = false;
        StartCoroutine(CelebrateThenReset());
    }
    private IEnumerator OutOfBoundsReset(){
        OutOfBoundsMessageDisplay.SetActive(true);
        yield return new WaitForSeconds(2);
        OutOfBoundsMessageDisplay.SetActive(false);
        DropPuck();
    }
    public void PuckOutOfBounds(){
        StartCoroutine(OutOfBoundsReset());
        // Trigger crowd effects
    }
    void Update(){
        HandleCameraPositioning();
        HandleCameraFocus();
        HandleCameraZoom();
    }
    public void QuitGame(){
        Application.Quit();
    }
}
