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
    [SerializeField] GameObject skaterPrefab;
    [SerializeField] GameObject goaltenderPrefab;
    [SerializeField] GameObject puckPrefab;
    [SerializeField] public Transform homeGoalOrigin;
    [SerializeField] public Transform awayGoalOrigin;
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
    [Header("SFX")]
    [SerializeField] public AudioClip goalHornhSFX;
    [SerializeField] public float goalHornVolume;
    [SerializeField] public AudioClip puckDropSFX;
    [SerializeField] public float puckDropVolume;
    [SerializeField] public AudioClip shotSFX;
    [SerializeField] public float shotVolume;
    [SerializeField] public AudioClip[] passSFX;
    [SerializeField] public float passVolume;
    private void Awake(){
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        cameraPausePosition = new Vector3 (0,0,0);
        homeGoaltender.transform.position = homeGoalOrigin.position;
        awayGoaltender.transform.position = awayGoalOrigin.position;
    }
    private IEnumerator TemporaryFaceOffMessage(){
        FaceOffMessageDisplay.SetActive(true);
        yield return new WaitForSeconds(2);
        FaceOffMessageDisplay.SetActive(false);
    }
    public void DropPuck(){
        GoalScoredDisplay.SetActive(false);
        StartCoroutine(TemporaryFaceOffMessage());
        if(puckObject){Destroy(puckObject);}
        gameOn = true;
        homeNet.GetComponent<Goal>().GameOn();
        awayNet.GetComponent<Goal>().GameOn();
        puckObject = Instantiate(puckPrefab, puckDropOrigin.position, Quaternion.Euler(75, 0, 0));
        AudioSource.PlayClipAtPoint(puckDropSFX, Camera.main.transform.position, puckDropVolume);
        puckRigidBody = puckObject.GetComponent<Rigidbody>();
        focalObject = puckObject;
        // deactivate HUD messages
    }
    private void Start(){
        DropPuck();
    }
    public void JoinNewPlayer(PlayerInput playerInput){
        var newPlayerInput = playerInput.gameObject.GetComponent<PlayerController>();
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
        AudioSource.PlayClipAtPoint(goalHornhSFX, Camera.main.transform.position, goalHornVolume);
        yield return new WaitForSeconds(2);
        GoalScoredDisplay.SetActive(false);
    }
    private IEnumerator CelebrateThenReset(){
        StartCoroutine(TemporaryGoalMessage());
        Destroy(puckObject, 1.5f);
        // Trigger celebration / sad animations
        // point a spotlight on the player who scored
        // Goal horn sound
        // Turn on goal lights
        yield return new WaitForSeconds(5);
        // yield return Startcoroutine(Celebrations())
        DropPuck();
    }
    public void GoalScored(bool scoredOnHomeNet){
        if(scoredOnHomeNet){awayScore++;}
        else{homeScore++;}
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
