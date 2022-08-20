using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameStartScript : MonoBehaviour
{
    public GameObject playButton;
    public GameObject creditsButton;
    public GameObject quitButton;
    public GameObject mainDisplay;
    public GameObject creditsDisplay;
    public GameObject chooseSidesMenu;
    public string sceneNameToLoad = "error";
    public void LoadSceneByName(){
        if(sceneNameToLoad=="error"){
            Debug.LogError("scene name not set in inspector for this function");
        } else {
            SceneManager.LoadScene(sceneNameToLoad);
        }
    }
    public void SwitchToChooseSideMenu(){
        mainDisplay.SetActive(true);
        EventSystem.current.SetSelectedGameObject(playButton);
    }
    private void Start() {
        EventSystem.current.SetSelectedGameObject(playButton);
    }
}
