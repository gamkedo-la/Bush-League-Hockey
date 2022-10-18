using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class VCamCoordinator : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera vCamMainMenu;
    [SerializeField] private CinemachineVirtualCamera vCamFaceOff;
    [SerializeField] private CinemachineVirtualCamera vCamGameplay;
    private void Awake() {
        MainMenuState.onEnter += SwitchToMainMenuVCam;
    }
    private void SwitchToMainMenuVCam(object sender, System.EventArgs e){
        vCamMainMenu.gameObject.SetActive(true);
        vCamFaceOff.gameObject.SetActive(false);
        vCamGameplay.gameObject.SetActive(false);
    }
    private void SwitchToFaceOffVCam(object sender, System.EventArgs e){
        vCamMainMenu.gameObject.SetActive(false);
        vCamFaceOff.gameObject.SetActive(true);
        vCamGameplay.gameObject.SetActive(false);
    }
    private void SwitchToGameplayVCam(object sender, System.EventArgs e){
        vCamMainMenu.gameObject.SetActive(false);
        vCamFaceOff.gameObject.SetActive(true);
        vCamGameplay.gameObject.SetActive(false);
    }
}
