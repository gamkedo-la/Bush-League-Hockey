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
    private void SwitchToMainMenuVCam(object sender, EventArgs e){
        vCamMainMenu.gameObject.SetActive(true);
        vCamFaceOff.gameObject.SetActive(false);
        vCamGameplay.gameObject.SetActive(false);
    }
    private void SwitchToFaceOffVCam(object sender, EventArgs e){
        vCamMainMenu.gameObject.SetActive(false);
        vCamFaceOff.gameObject.SetActive(true);
        vCamGameplay.gameObject.SetActive(false);
    }
    private void SwitchToGameplayVCam(object sender, EventArgs e){
        vCamMainMenu.gameObject.SetActive(false);
        vCamFaceOff.gameObject.SetActive(true);
        vCamGameplay.gameObject.SetActive(false);
    }
}
