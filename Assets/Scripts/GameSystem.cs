using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameSystem : MonoBehaviour
{
    [Header("Main Camera")]
    public Camera mainCamera;
    [SerializeField] [Range(15f, .5f)] float zoom;
    private GameObject focalObject;
    [SerializeField] [Range(0.0f, 0.2f)] float cameraRotationSpeed;
    private Vector3 lineToDesiredTarget;
    private Quaternion desiredCameraRotation;
    [Header("Game Management")]
    public GameObject puckObject;
    public Rigidbody puckRigidBody;
    private int homeScore = 0;
    private int awayScore = 0;
    private void Awake(){
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        focalObject = puckObject;
        puckRigidBody = puckObject.GetComponent<Rigidbody>();
    }
    public void JoinNewPlayer(){
        Debug.Log("New player joining");
        // Pause game
        // Instantiate the player controller
        // open choose sides menu
        // all players accept
    }
    private void HandleCameraPositioning(){
        mainCamera.transform.position = new Vector3((puckObject.transform.position.x / 2.5f), mainCamera.transform.position.y, mainCamera.transform.position.z);
    }
    private void HandleCameraFocus(){
        lineToDesiredTarget = Vector3.Normalize(focalObject.transform.position - mainCamera.transform.position);
        desiredCameraRotation = Quaternion.LookRotation(lineToDesiredTarget, Vector3.up);
        mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, desiredCameraRotation, cameraRotationSpeed);
    }
    private void HandleCameraZoom(){
        // total horizontal distance between the players and the puck
        // 
        // (max: rink width, min: everone at center ice)
    }
    void Update(){
        HandleCameraPositioning();
        HandleCameraFocus();
        HandleCameraZoom();
    }
}
