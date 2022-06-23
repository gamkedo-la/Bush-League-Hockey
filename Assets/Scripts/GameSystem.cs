using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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
    [SerializeField] public GameObject homeNet;
    [SerializeField] public GameObject awayNet;
    [HideInInspector] public GameObject puckObject;
    [HideInInspector] public Rigidbody puckRigidBody;
    [Header("Controls Management")]
    [HideInInspector] public List<GameObject> localPlayerControllers;
    private int homeScore = 0;
    private int awayScore = 0;
    private bool gameOn = false;
    private void Awake(){
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        cameraPausePosition = new Vector3 (0,0,0);
        homeGoaltender.transform.position = homeGoalOrigin.position;
        awayGoaltender.transform.position = awayGoalOrigin.position;
    }
    private void DropPuck(){
        gameOn = true;
        homeNet.GetComponent<Goal>().GameOn();
        awayNet.GetComponent<Goal>().GameOn();
        puckObject = Instantiate(puckPrefab, puckDropOrigin.position, Quaternion.Euler(75, 0, 0));
        puckRigidBody = puckObject.GetComponent<Rigidbody>();
        focalObject = puckObject;
    }
    private void Start(){
        // homeGoaltender = Instantiate(goaltenderPrefab, homeGoalOrigin.position, Quaternion.identity);
        // var homeGoalieScript = homeGoaltender.GetComponent<Goaltender>();
        // var homeGoalieTM = homeGoaltender.GetComponent<TeamMember>();
        // homeGoalieTM.SetIsHomeTeam(true);
        // homeGoalieScript.FindMyNet();
        // awayGoaltender = Instantiate(goaltenderPrefab, awayGoalOrigin.position, Quaternion.identity);
        // var awayGoalieTM = homeGoaltender.GetComponent<TeamMember>();
        // var awayGoalieScript = homeGoaltender.GetComponent<Goaltender>();
        // awayGoalieTM.SetIsHomeTeam(false);
        // awayGoalieScript.FindMyNet();
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
    private IEnumerator CelebrateThenReset(){
        Destroy(puckObject, 1.5f);
        //Trigger celebration and other events
        yield return new WaitForSeconds(5);
        DropPuck();
    }
    public void GoalScored(bool scoredOnHomeNet){
        if(scoredOnHomeNet){awayScore++;}
        else{homeScore++;}
        gameOn = false;
        Debug.Log($"Home: {homeScore}  ||  Away: {awayScore}");
        StartCoroutine(CelebrateThenReset());
    }
    void Update(){
        HandleCameraPositioning();
        HandleCameraFocus();
        HandleCameraZoom();
    }
}
