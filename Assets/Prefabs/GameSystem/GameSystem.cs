using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class GameSystem : MonoBehaviour
{
    [Header("Game Management")]
    public Animator masterStateMachine;
    public static EventHandler<EventArgs> dropPuck;
    public static EventHandler<EventArgs> outOfBounds;
    public TimeManager timeManager;
    public AudioManager audioManager;
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
    [SerializeField] public GameplayState currentGameData;
    [SerializeField] public GameObject homeNet;
    [SerializeField] public GameObject awayNet;
    [SerializeField] TextMeshProUGUI homeScoreText;
    [SerializeField] TextMeshProUGUI awayScoreText;
    [SerializeField] TextMeshProUGUI timerText;
    public int homeHits = 0;
    public int awayHits = 0;
    public int homeSaves = 0;
    public int awaySaves = 0;
    public int homePasses = 0;
    public int awayPasses = 0;
    private bool saveCooldownDone = true;
    [Header("Onscreen Messages / Menus")]
    [SerializeField] public GameObject inGameHUD;
    [SerializeField] public GameObject mainMenu;
    [SerializeField] public GameObject FaceOffMessageDisplay;
    [SerializeField] public GameObject OutOfBoundsMessageDisplay;
    [SerializeField] public GameObject countdownDisplayPanel;
    [SerializeField] public TextMeshProUGUI countdownCountText;
    private void Awake(){
        masterStateMachine = GetComponent<Animator>();
        audioManager = FindObjectOfType<AudioManager>();
        timeManager = FindObjectOfType<TimeManager>();
        puckRigidBody = puckObject.GetComponent<Rigidbody>();
        puckTrail = puckObject.GetComponent<TrailRenderer>();
        homeSkaterRigidBody = homeSkater.GetComponent<Rigidbody>();
        awaySkaterRigidBody = awaySkater.GetComponent<Rigidbody>();
        allTeamMemberScripts = FindObjectsOfType<TeamMember>();
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
        SetAIActiveState(true);
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
    public void SetAllActionMapsToDisabled(){
        foreach (PlayerInput ctrl in FindObjectsOfType<PlayerInput>()){
            ctrl.SwitchCurrentActionMap("Disabled");
        }
    }
    public void SetAIActiveState(bool activeState){
        AIControllerAway.SetActive(activeState);
        AIControllerHome.SetActive(activeState);
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
    public void PuckToCenterOrigin(){
        puckObject.transform.position = puckDropOrigin.position;
        puckObject.transform.rotation = puckDropOrigin.rotation;
        puckRigidBody.velocity = Vector3.zero;
        puckTrail.Clear();
        dropPuck?.Invoke(this, EventArgs.Empty);
    }
    public void CasualGoalScored(){
        StartCoroutine(CasualDropPuck());
    }
    public IEnumerator CasualDropPuck(){
        float elapsedTime = 0;
        while(elapsedTime < 3){
            yield return new WaitForFixedUpdate();
            elapsedTime += Time.fixedDeltaTime;
        }
        PuckToCenterOrigin();
    }
    private IEnumerator OutOfBoundsReset(){
        OutOfBoundsMessageDisplay.SetActive(true);
        yield return new WaitForSeconds(2);
        OutOfBoundsMessageDisplay.SetActive(false);
    }
    public void PuckOutOfBounds(){
        StartCoroutine(OutOfBoundsReset());
    }
}
