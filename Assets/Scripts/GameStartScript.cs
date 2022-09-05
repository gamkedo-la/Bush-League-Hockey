using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;
public class GameStartScript : MonoBehaviour
{
    [Header("Main Menu")]
    [SerializeField] public GameObject playButton;
    public GameObject creditsButton;
    public GameObject quitButton;
    public GameObject mainDisplay;
    [Header("Choose Sides Menu")]
    List<GameObject> menuIcons;
    public GameObject chooseSidesMenu;
    public GameObject acceptButton;
    public GameObject helpDisplay;
    [Header("Credits View")]
    public GameObject creditsDisplay;
    public GameObject backButton;
    [HideInInspector] public GameObject currentItem;
    public void SetActiveMenuItemForAllPlayers(GameObject menuItem){
        currentItem = menuItem;
        foreach (MultiplayerEventSystem eventSystem in FindObjectsOfType<MultiplayerEventSystem>())
        {
            eventSystem.SetSelectedGameObject(menuItem);
        }
    }
    public void SwitchToChooseSideMenu(){
        mainDisplay.SetActive(false);
        chooseSidesMenu.SetActive(true);
        SetActiveMenuItemForAllPlayers(acceptButton);
    }
    public void SwitchToMainDisplay(){
        mainDisplay.SetActive(true);
        creditsDisplay.SetActive(false);
        chooseSidesMenu.SetActive(false);
        helpDisplay.SetActive(true);
        SetActiveMenuItemForAllPlayers(playButton);
    }
    public void SwitchToCreditsView(){
        creditsDisplay.SetActive(true);
        mainDisplay.SetActive(false);
        helpDisplay.SetActive(false);
        SetActiveMenuItemForAllPlayers(backButton);
    }
    public void SetPlayerInputSides(){
        StartScreenInputManager inputManager = FindObjectOfType<StartScreenInputManager>();
        foreach (MenuController ctrl in FindObjectsOfType<MenuController>()){
            Debug.Log($"status: {ctrl.teamSelectionStatus}");
            DontDestroyOnLoad(ctrl.gameObject);
        }
        SceneManager.LoadScene("Hat-Trick");        
    }
    private void Start() {
        SetActiveMenuItemForAllPlayers(playButton);
    }
    public void QuitGame(){
        Application.Quit();
    }
}
