using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class GameSystem : MonoBehaviour
{
    [Header("Main Camera")]
    [HideInInspector] public Camera mainCamera;
    private Vector3 cameraPausePosition;
    private Vector3 cameraDesiredPosition;
    private Quaternion cameraPauseRotation;
    [SerializeField] [Range(15f, .5f)] float zoom;
    private GameObject focalObject;
    [SerializeField] [Range(0.0f, 0.2f)] float cameraRotationSpeed;
    private Vector3 lineToDesiredTarget;
    private Quaternion desiredCameraRotation;
    [Header("Game Management")]
    [SerializeField] private GameObject mainMenu;
    public static EventHandler<EventArgs> dropPuck;
    public TimeManager timeManager;
    public TimeProvider activeTimeProvider;
    private AudioManager audioManager;
    [SerializeField] private GameObject crowdReactionManager;
    [SerializeField] public GameObject homeSkater;
    [SerializeField] public GameObject awaySkater;
    [SerializeField] public GameObject homeGoaltender;
    [SerializeField] public GameObject awayGoaltender;
    [SerializeField] public GameObject puckObject;
    [HideInInspector] public Rigidbody puckRigidBody;
    [HideInInspector] public TrailRenderer puckTrail;
    [SerializeField] public Transform puckDropOrigin;
    [SerializeField] GameObject AIControllerHome;
    [SerializeField] GameObject AIControllerAway;
    [SerializeField] public Transform homeGoalOrigin;
    [SerializeField] public Transform homeFaceOffOrigin;
    [SerializeField] public Transform awayGoalOrigin;
    [SerializeField] public Transform awayFaceOffOrigin;
    [HideInInspector] public TeamMember[] allTeamMemberScripts;
    [HideInInspector] public Rigidbody homeSkaterRigidBody;
    [HideInInspector] public Rigidbody awaySkaterRigidBody;
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
        puckTrail = puckObject.GetComponent<TrailRenderer>();
        homeSkaterRigidBody = homeSkater.GetComponent<Rigidbody>();
        awaySkaterRigidBody = awaySkater.GetComponent<Rigidbody>();
        allTeamMemberScripts = FindObjectsOfType<TeamMember>();
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
        playerInput.GetComponent<MultiplayerEventSystem>().firstSelectedGameObject = mainMenu.GetComponent<MainMenuScript>().currentItem;
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
        AIControllerAway.SetActive(true);
        AIControllerHome.SetActive(true);
        foreach(MenuController ctrl in FindObjectsOfType<MenuController>()){
            Debug.Log($"Setting player {ctrl} to {ctrl.teamSelectionStatus}");
            switch (ctrl.teamSelectionStatus){
                case "home":
                    AIControllerHome.SetActive(false);
                    ctrl.GetComponent<PlayerController>().SetToHomeTeam();
                    homeTeamMemberCount++;
                    break;
                case "away":
                    AIControllerAway.SetActive(false);
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
    public void ApplyGameplayFrameData(GameplaySingleFrameData frame){
        homeSkater.transform.position = frame.p1Position; homeSkater.transform.rotation = frame.p1Rotation;
        homeSkaterRigidBody.velocity = frame.p1Velocity;
        awaySkater.transform.position = frame.p2Position; awaySkater.transform.rotation = frame.p2Rotation;
        awaySkaterRigidBody.velocity = frame.p1Velocity;
        homeGoaltender.transform.position = frame.g1Position; homeGoaltender.transform.rotation = frame.g1Rotation;
        awayGoaltender.transform.position = frame.g2Position; awayGoaltender.transform.rotation = frame.g2Rotation;
        puckObject.transform.position = frame.puckPosition; puckObject.transform.rotation = frame.puckRotation;
        puckRigidBody.velocity = frame.puckVelocity;
    }
    public void SetupPlayersForFaceOff(){
        timeManager.gameTime.timeScale = 1;
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
    public void PuckToCenterOrigin(){
        puckObject.transform.position = puckDropOrigin.position;
        puckObject.transform.rotation = puckDropOrigin.rotation;
        puckRigidBody.velocity = Vector3.zero;
        puckTrail.Clear();
        dropPuck?.Invoke(this, EventArgs.Empty);
    }
    public void DropPuck(){
        GoalScoredDisplay.SetActive(false);
        SetupPlayersForFaceOff();
        StartCoroutine(TemporaryFaceOffMessage());
        PuckToCenterOrigin();
        gameOn = true;
        audioManager.PlayFaceOffSound();
        focalObject = puckObject;
        ActivateGoals();
    }
    public void CasualGoalScored(){
        StartCoroutine(CasualDropPuck());
    }
    public IEnumerator CasualDropPuck(){
        DeactivateGoals();
        float elapsedTime = 0;
        while(elapsedTime < 3){
            yield return new WaitForFixedUpdate();
            elapsedTime += Time.fixedDeltaTime;
        }
        PuckToCenterOrigin();
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
        cameraDesiredPosition.x = 0.4f*puckObject.transform.position.x;
        cameraDesiredPosition.y = mainCamera.transform.position.y;
        cameraDesiredPosition.z = mainCamera.transform.position.z;
        mainCamera.transform.position = Vector3.Lerp(
            mainCamera.transform.position,
            cameraDesiredPosition,
            Time.fixedDeltaTime
        );
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
        //BeginGame();
    }
    void Update(){
        // current state -> OnStateUpdate
        HandleGameTimer();
        HandleCameraPositioning();
    }
}
