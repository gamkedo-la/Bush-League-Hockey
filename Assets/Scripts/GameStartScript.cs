using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
public class GameStartScript : MonoBehaviour
{
    [Header("Main Menu")]
    public GameObject playButton;
    public GameObject creditsButton;
    public GameObject quitButton;
    public GameObject mainDisplay;
    [Header("Choose Sides Menu")]
    List<GameObject> menuIcons;
    public GameObject chooseSidesMenu;
    public GameObject acceptButton;
    [Header("Credits View")]
    public GameObject creditsDisplay;
    public GameObject backButton;
    public string sceneNameToLoad = "error";
    public void LoadSceneByName(){
        if(sceneNameToLoad=="error"){
        } else {
            SceneManager.LoadScene(sceneNameToLoad);
        }
    }
    public void SwitchToChooseSideMenu(){
        mainDisplay.SetActive(false);
        chooseSidesMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(acceptButton);
        // get all the player inputs
        MenuController[] playerInputs = FindObjectsOfType<MenuController>();
    }
    public void SwitchToMainDisplay(){
        mainDisplay.SetActive(true);
        creditsDisplay.SetActive(false);
        chooseSidesMenu.SetActive(false);
        // disable choose sides menu script
        EventSystem.current.SetSelectedGameObject(playButton);
    }
    public void SwitchToCreditsView(){
        creditsDisplay.SetActive(true);
        mainDisplay.SetActive(false);
        EventSystem.current.SetSelectedGameObject(backButton);
    }
    public void SetPlayerInputSides(){
        StartScreenInputManager inputManager = FindObjectOfType<StartScreenInputManager>();
        foreach (MenuController ctrl in FindObjectsOfType<MenuController>()){
            Debug.Log($"status: {ctrl.teamSelectionStatus}");
            ctrl.GetComponent<PlayerController>().isHomeTeam = ctrl.GetComponent<MenuController>().teamSelectionStatus == "home";
            DontDestroyOnLoad(ctrl.gameObject);
        }
        SceneManager.LoadScene("Hat-Trick");        
    }
    // event listener for navigation events
    // when any player uses navigation controls on the menu:
    // they become the input for the UI
    public void QuitGame(){
        Application.Quit();
    }
}
