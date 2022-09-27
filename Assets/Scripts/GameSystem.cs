using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;
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
    public TimeManager timeManager;
    public TimeProvider activeTimeProvider;
    private AudioManager audioManager;
    [SerializeField] private GameObject crowdReactionManager;
    [SerializeField] public GameObject homeSkater;
    [SerializeField] public GameObject awaySkater;
    [SerializeField] public GameObject homeGoaltender;
    [SerializeField] public GameObject awayGoaltender;
    [SerializeField] public GameObject puckObject;
    [SerializeField] GameObject AIControllerPrefab;
    [SerializeField] public Transform homeGoalOrigin;
    [SerializeField] public Transform homeFaceOffOrigin;
    [SerializeField] public Transform awayGoalOrigin;
    [SerializeField] public Transform awayFaceOffOrigin;
    [SerializeField] public Transform puckDropOrigin;
    
    [HideInInspector] public Rigidbody puckRigidBody;
    [Header("Controls Management")]
    [SerializeField] public GameObject ps4ControllerIcon;
    [SerializeField] public GameObject keyboardIcon;
    [SerializeField] public GameObject genericControllerIcon;
    [SerializeField] public GameObject xboxControllerIcon;
    [Header("Scorekeeping")]
    [SerializeField] public GameObject homeNet;
    [SerializeField] public GameObject awayNet;
    [SerializeField] TextMeshProUGUI homeScoreText;
    [SerializeField] TextMeshProUGUI awayScoreText;
    [SerializeField] TextMeshProUGUI timerText;
    public int homeScore = 0;
    public int awayScore = 0;
    public int homeHits = 0;
    public int awayHits = 0;
    public int homeSaves = 0;
    public int awaySaves = 0;
    public int homePasses = 0;
    public int awayPasses = 0;
    private float timeRemaining = 300;
    private bool gameOn = false;
    private bool clockIsRunning = false;
    private bool isSuddenDeath = false;
    private bool saveCooldownDone = true;
    [Header("Onscreen Messages / Menus")]
    [SerializeField] public GameObject GoalScoredDisplay;
    [SerializeField] public GameObject FaceOffMessageDisplay;
    [SerializeField] public GameObject OutOfBoundsMessageDisplay;
    [SerializeField] public GameObject instantReplayController;
    [SerializeField] public GameObject suddenDeathDisplay;
    [SerializeField] public GameObject gamMenuButtonPanel;
    [SerializeField] public GameObject rematchButton;
    [SerializeField] public GameObject countdownDisplayPanel;
    [SerializeField] TextMeshProUGUI countdownCountText;
    [SerializeField] public GameObject endOfGameMenu;
    [SerializeField] public GameObject endOfGameHomeScoreBox;
    [SerializeField] public GameObject endOfGameAwayScoreBox;
    [SerializeField] TextMeshProUGUI endOfGameHomeScoreText;
    [SerializeField] TextMeshProUGUI endOfGameAwayScoreText;
    [SerializeField] public GameObject endOfGameHomeWinnerTag;
    [SerializeField] public GameObject endOfGameAwayWinnerTag;
    private void Awake(){
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        audioManager = FindObjectOfType<AudioManager>();
        cameraPausePosition = Vector3.zero;
        timeManager = FindObjectOfType<TimeManager>();
        puckRigidBody = puckObject.GetComponent<Rigidbody>();
    }
    public void PreserveKeyGameElements(){
        foreach(PlayerInput ctrl in FindObjectsOfType<PlayerInput>()){
            DontDestroyOnLoad(ctrl.gameObject);
        }
    }
    public bool IsZeroQuaternion(Quaternion q){
        return q.x == 0 && q.y == 0 && q.z == 0 && q.w == 0;
    }
    public void JoinNewPlayer(PlayerInput playerInput){
        Debug.Log($"New Player: {playerInput.currentControlScheme}");
        switch (playerInput.currentControlScheme){
            case "Keyboard&Mouse":
                playerInput.GetComponent<MenuController>().chooseSidesMenuIcon = keyboardIcon;
                break;
            case "PS4":
                playerInput.GetComponent<MenuController>().chooseSidesMenuIcon = ps4ControllerIcon;
                break;
            case "XBox":
                playerInput.GetComponent<MenuController>().chooseSidesMenuIcon = xboxControllerIcon;
                break;
            case "Gamepad":
                playerInput.GetComponent<MenuController>().chooseSidesMenuIcon = genericControllerIcon;
                break;
        }
        playerInput.GetComponent<MenuController>().InitializeController();
    }
    private IEnumerator SaveCooldown(){
        saveCooldownDone = false;
        yield return new WaitForSeconds(.4f);
        saveCooldownDone = true;
    }
    public void CountSave(bool homeSave){
        if(saveCooldownDone){
            StartCoroutine(SaveCooldown());
            if(homeSave){
                homeSaves++;
            } else {
                awaySaves++;
            }
        }
    }
    public void SetPlayersToTeams(){
        int homeTeamMemberCount = 0;
        int awayTeamMemberCount = 0;
        foreach(MenuController ctrl in FindObjectsOfType<MenuController>()){
            Debug.Log($"Setting player {ctrl} to {ctrl.teamSelectionStatus}");
            switch (ctrl.teamSelectionStatus){
                case "home":
                    ctrl.GetComponent<PlayerController>().SetToHomeTeam();
                    homeTeamMemberCount++;
                    break;
                case "away":
                    ctrl.GetComponent<PlayerController>().SetToAwayTeam();
                    awayTeamMemberCount++;
                    break;
                case "neutral":
                    ctrl.GetComponent<PlayerController>().SetToNeutralTeam();
                    break;
                default:
                    break;
            }
        }
        foreach (AIPlayerController aI in FindObjectsOfType<AIPlayerController>()){
            DestroyImmediate(aI.gameObject);
        }
        if(homeTeamMemberCount == 0){
            Instantiate(AIControllerPrefab).GetComponent<AIPlayerController>().SetToHomeTeam();
        }
        if(awayTeamMemberCount == 0){
            Instantiate(AIControllerPrefab).GetComponent<AIPlayerController>().SetToAwayTeam();
        }
    }
    public void SetAllActionMapsToUI(){
        foreach (PlayerInput ctrl in FindObjectsOfType<PlayerInput>()){
            ctrl.SwitchCurrentActionMap("UI");
        }
    }
    public void SetAllActionMapsToPlayer(){
        foreach (PlayerInput ctrl in FindObjectsOfType<PlayerInput>()){
            ctrl.SwitchCurrentActionMap("Player");
        }
    }
    public void SetAllActionMapsToReplay(){
        foreach (PlayerInput ctrl in FindObjectsOfType<PlayerInput>()){
            ctrl.SwitchCurrentActionMap("Replay");
        }
    }
    public void SetAIActiveState(bool activeState){
        foreach (AIPlayerController aI in Resources.FindObjectsOfTypeAll<AIPlayerController>()){
            aI.gameObject.SetActive(activeState);
        }
    }
    public void UnFreeze(){
        timeManager.gameTime.timeScale = 1;
    }
    public void FreezeGame(){
        // doesn't actually effect game physics or coroutines?
        timeManager.gameTime.timeScale = 0;
    }
    public void QuitGame(){
        Application.Quit();
    }
    public void RestartScene(){
        PreserveKeyGameElements();
        SceneManager.LoadScene("Hat-Trick");
    }
    public void LoadMainMenu(){
        // PreserveKeyGameElements();
        SceneManager.LoadScene("StartMenu");
        // what is the current scene?
        // SceneLoader current scene     
    }
    public IEnumerator FlashingOnScreenMessage(GameObject messageDisplay, int cycles){
        for (int i = 0; i < cycles; i++){
            messageDisplay.SetActive(true);
            yield return new WaitForSeconds(0.15f);
            messageDisplay.SetActive(false);
            yield return new WaitForSeconds(0.075f);
        }
    }
    private IEnumerator TemporaryGoalMessage(){
        GoalScoredDisplay.SetActive(true);
        yield return new WaitForSeconds(1.2f);
        GoalScoredDisplay.SetActive(false);
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
        StartCoroutine(homeSkater.GetComponent<TeamMember>().BreakPosession());
        StartCoroutine(awaySkater.GetComponent<TeamMember>().BreakPosession());
        StartCoroutine(homeGoaltender.GetComponent<TeamMember>().BreakPosession());
        StartCoroutine(awayGoaltender.GetComponent<TeamMember>().BreakPosession());
        // position players
        homeSkater.transform.position = homeFaceOffOrigin.position;
        homeSkater.transform.rotation = homeFaceOffOrigin.rotation;
        awaySkater.transform.position = awayFaceOffOrigin.position;
        awaySkater.transform.rotation = awayFaceOffOrigin.rotation;
        homeGoaltender.transform.position = homeGoalOrigin.position;
        awayGoaltender.transform.position = awayGoalOrigin.position;
    }
    public void DeactivateGoals(){
        homeNet.GetComponent<Goal>().goalIsActive = false;
        awayNet.GetComponent<Goal>().goalIsActive = false;
    }
    public void ActivateGoals(){
        homeNet.GetComponent<Goal>().goalIsActive = true;
        awayNet.GetComponent<Goal>().goalIsActive = true;
    }
    public void DropPuck(){
        instantReplayController?.GetComponent<InstantReplay>()?.StopInstantReplay();
        GoalScoredDisplay.SetActive(false);
        SetupPlayersForFaceOff();
        StartCoroutine(TemporaryFaceOffMessage());
        puckObject.transform.position = puckDropOrigin.position;
        puckObject.transform.rotation = puckDropOrigin.rotation;
        puckObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        puckObject.GetComponent<TrailRenderer>().Clear();
        gameOn = true;
        audioManager.PlayFaceOffSound();
        focalObject = puckObject;
        ActivateGoals();
    }
    public IEnumerator CountDownAndDropPuck(){
        countdownDisplayPanel.SetActive(true);
        audioManager.PlayReadySound();
        for (int i = 3; i > 0; i--){
            countdownCountText.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
        countdownDisplayPanel.SetActive(false);
        DropPuck();
    }
    private IEnumerator CelebrateThenReset(){
        InstantReplay instantReplay = FindObjectOfType<InstantReplay>();
        yield return StartCoroutine(TemporaryGoalMessage());
        // Switch to replay state
        // GameTime freezes
        // save the current state of objects in the scene
        // disable all controllers (player and AI)
        // begin the replay:  Should play back at ReplayTime.deltatime
        // replay finishes or is cancelled: return to the saved state
        // point a spotlight on the player who scored
        yield return StartCoroutine(instantReplay.startInstantReplay()); // new WaitForSeconds(3);
        // is it sudden death? declare the winner
        if(isSuddenDeath){
            isSuddenDeath = false;
            StartCoroutine(EndOfGameHandler());
        }else{StartCoroutine(CountDownAndDropPuck());}
    }
    public void UpdateScoreBoard(){
        homeScoreText.text = endOfGameHomeScoreText.text = homeScore.ToString();
        awayScoreText.text = endOfGameAwayScoreText.text = awayScore.ToString();
        if(!isSuddenDeath){
            string minutes = $"{(int)timeRemaining/60}";
            string seconds = (int)timeRemaining % 60 < 10 ? $"0{(int)timeRemaining % 60}" : $"{(int)timeRemaining % 60}";
            timerText.text = $"{minutes}:{seconds}";
        }
    }
    public void GoalScored(bool scoredOnHomeNet){
        DeactivateGoals();
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
        UpdateScoreBoard();
        gameOn = false;
        StartCoroutine(CelebrateThenReset());
    }
    private IEnumerator OutOfBoundsReset(){
        gameOn = false;
        DeactivateGoals();
        OutOfBoundsMessageDisplay.SetActive(true);
        yield return new WaitForSeconds(2);
        OutOfBoundsMessageDisplay.SetActive(false);
        StartCoroutine(CountDownAndDropPuck());
    }
    public void PuckOutOfBounds(){
        if(gameOn){
            StartCoroutine(OutOfBoundsReset());
        }
        // Trigger crowd effects
    }
    private IEnumerator RunClock(){
        clockIsRunning = true;
        while(clockIsRunning){
            yield return new WaitForSeconds(timeManager.gameTime.deltaTime);
            timeRemaining -= timeManager.gameTime.deltaTime*1.4f;
            clockIsRunning = gameOn;
            UpdateScoreBoard();
        }
    }
    private IEnumerator EndOfGamePresentation(){
        // make sure all canvas elements are in the correct state
        endOfGameMenu.SetActive(false);
        endOfGameHomeScoreBox.SetActive(false);
        endOfGameAwayScoreBox.SetActive(false);
        endOfGameHomeWinnerTag.SetActive(false);
        endOfGameAwayWinnerTag.SetActive(false);
        // Begin the show
        endOfGameMenu.SetActive(true);
        yield return new WaitForSeconds(2);
        endOfGameHomeScoreBox.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        endOfGameAwayScoreBox.SetActive(true);
        yield return new WaitForSeconds(2);
        if(homeScore > awayScore){
            endOfGameHomeWinnerTag.SetActive(true);
            StartCoroutine(crowdReactionManager.transform.GetComponent<CrowdReactionManagerScriptComponent>().HandleHomeTeamScoringAGoal());
        } else {
            endOfGameAwayWinnerTag.SetActive(true);
            StartCoroutine(crowdReactionManager.transform.GetComponent<CrowdReactionManagerScriptComponent>().HandleAwayTeamScoringAGoal());
        }
        audioManager.PlayGoalHorn();
        audioManager.PlayCrowdCelebration();
        yield return new WaitForSeconds(4);
    }
    private IEnumerator EndOfGameHandler(){
        audioManager.PlayWoodWhistle();
        yield return new WaitForSeconds(2.5f);
        if(homeScore == awayScore){
            isSuddenDeath = true;
            timerText.text = "sudden death";
            audioManager.PlaySuddenDeath();
            yield return StartCoroutine(FlashingOnScreenMessage(suddenDeathDisplay, 12));
            StartCoroutine(CountDownAndDropPuck());
        } else {
            UpdateScoreBoard();
            timerText.text = "final";
            yield return StartCoroutine(EndOfGamePresentation());
            SetAllActionMapsToUI();
            FindObjectOfType<InGameMenu>().SwitchToEndGameMenu();
        }
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
    private void HandleGameTimer(){
        // Should the clock start running?
        if(gameOn && !clockIsRunning && !isSuddenDeath){StartCoroutine(RunClock());}
        if(timeRemaining <= 0  && gameOn && !isSuddenDeath){
            gameOn = false;
            timeRemaining = 0;
            DeactivateGoals();
            StartCoroutine(EndOfGameHandler());
        }
    }
    public void BeginGame(){
        DeactivateGoals();
        audioManager.PlayBaseCrowdTrack();
        SetPlayersToTeams();
        SetAllActionMapsToPlayer();
        StartCoroutine(CountDownAndDropPuck());
    }
    private void Start(){
        BeginGame();
    }
    void Update(){
        HandleGameTimer();
        HandleCameraPositioning();
        HandleCameraFocus();
    }
}
