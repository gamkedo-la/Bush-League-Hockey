using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    [Header("Main Camera")]
    private Camera mainCamera;
    [SerializeField] float zoom;
    [SerializeField] GameObject focalObject;
    [SerializeField] [Range(0.0f, 1f)] float cameraRotationSpeed;
    private Vector3 lineToDesiredTarget;
    private Quaternion desiredCameraRotation;
    [Header("Game Management")]
    private int homeScore = 0;
    private int awayScore = 0;
    private int posessionIndex = 0; //  0 free puck, 1 home posession, 2 away posession
    private List<TeamMember> membersCompetingForPosession;
    private TeamMember memberWithPosession;
    private void Awake(){
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }
    public bool CompeteForPosession(TeamMember memberCompetingForPosession){
        // player can take posession
        // does player already have posession
        // add player to membersCompetingForPosession if they aren't there already
        return true;
    }
    public bool MemberLostPosession(){
        memberWithPosession = null;
        return true;
    }
    public bool TeamHasPosession(int TeamIndex){
        return TeamIndex == posessionIndex;
    }
    private void HandlePosession(){
        // how many players competing for posession
        // if there is only one they win
        // otherwise 
    }
    private void HandleCameraFocus(){
        lineToDesiredTarget = Vector3.Normalize(focalObject.transform.position - mainCamera.transform.position);
        desiredCameraRotation = Quaternion.LookRotation(lineToDesiredTarget, Vector3.up);
        mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, desiredCameraRotation, cameraRotationSpeed);
    }
    private void HandleCameraZoom(){
        // total horizontal distance between the players and the puck 
        // (max: rink width, min: everone at center ice)
    }
    void Update(){
        HandlePosession();
        HandleCameraFocus();
        HandleCameraZoom();
    }
}
