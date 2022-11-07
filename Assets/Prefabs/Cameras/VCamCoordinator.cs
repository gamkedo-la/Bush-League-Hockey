using System;
using UnityEngine;
using Cinemachine;
public class VCamCoordinator : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera vCamMainMenu;
    [SerializeField] private CinemachineVirtualCamera vCamFaceOff;
    [SerializeField] private CinemachineVirtualCamera vCamGameplay;
    [SerializeField] private CinemachineVirtualCamera vCamCrowdPan;

    private void Awake(){
        MainMenuState.onStateEnter += SwitchToMainMenuVCam;
        FaceOffState.onStateEnter += SwitchToFaceOffVCam;
        GameOnState.onStateEnter += SwitchToGameplayVCam;
        GoalScoredState.onStateEnter += SwitchToCrowdPanVCam;
        EOGSetup.onStateEnter += SwitchToCrowdPanVCam;
    }
    private void AllCamsOff(){
        vCamMainMenu.gameObject.SetActive(false);
        vCamFaceOff.gameObject.SetActive(false);
        vCamGameplay.gameObject.SetActive(false);
        vCamCrowdPan.gameObject.SetActive(false);
    }
    private void SwitchToMainMenuVCam(object sender, EventArgs e){
        AllCamsOff();
        vCamMainMenu.gameObject.SetActive(true);
    }
    private void SwitchToFaceOffVCam(object sender, EventArgs e){
        AllCamsOff();
        vCamFaceOff.gameObject.SetActive(true);
    }
    private void SwitchToGameplayVCam(object sender, EventArgs e){
        AllCamsOff();
        vCamGameplay.gameObject.SetActive(true);
    }
    private void SwitchToCrowdPanVCam(object sender, EventArgs e){
        AllCamsOff();
        vCamCrowdPan.gameObject.SetActive(true);
    }
}
