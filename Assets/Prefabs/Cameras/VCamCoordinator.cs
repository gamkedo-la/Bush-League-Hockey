using System;
using UnityEngine;
using Cinemachine;
public class VCamCoordinator : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera vCamMainMenu;
    [SerializeField] private CinemachineVirtualCamera vCamFaceOff;
    [SerializeField] private CinemachineVirtualCamera vCamGameplay;

    private void Awake(){
        MainMenuState.onStateEnter += SwitchToMainMenuVCam;
        GameOnState.onStateEnter += SwitchToGameplayVCam;
        BeginGameState.onStateEnter += SwitchToFaceOffVCam;
    }
    private void AllCamsOff(){
        vCamMainMenu.gameObject.SetActive(false);
        vCamFaceOff.gameObject.SetActive(false);
        vCamGameplay.gameObject.SetActive(false);
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
        vCamFaceOff.gameObject.SetActive(true);
    }
}
