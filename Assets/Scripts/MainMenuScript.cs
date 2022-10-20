using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;
public class MainMenuScript : MonoBehaviour
{
    List<GameObject> menuIcons;
    GameSystem gameSystem;
    Animator masterStateMachine;
    [Header("Title Screen")]
    [SerializeField] public GameObject playButton;
    public GameObject creditsButton;
    public GameObject quitButton;
    public GameObject mainDisplay;
    [Header("Choose Sides Menu")]
    public GameObject chooseSidesMenu;
    public GameObject acceptButton;
    public GameObject helpDisplay;
    [Header("Credits View")]
    public GameObject creditsDisplay;
    public GameObject backButton;
    [HideInInspector] public GameObject currentItem;
    private void Awake() {
        gameSystem = FindObjectOfType<GameSystem>();
        masterStateMachine = gameSystem.GetComponent<Animator>();
        MainMenuState.onStateEnter += MainMenuEnter;
    }
    private void MainMenuEnter(object sender, EventArgs e){
        SwitchToMainMenu();
    }
    public void SetActiveMenuItemForAllPlayers(GameObject menuItem){
        currentItem = menuItem;
        foreach(MultiplayerEventSystem eventSystem in FindObjectsOfType<MultiplayerEventSystem>())
        {
            eventSystem.SetSelectedGameObject(menuItem);
        }
    }
    public void CloseMenus(){
        mainDisplay.SetActive(false);
        creditsDisplay.SetActive(false);
        chooseSidesMenu.SetActive(false);
        helpDisplay.SetActive(false);
    }
    public void SwitchToChooseSideMenu(){
        CloseMenus();
        chooseSidesMenu.SetActive(true);
        SetActiveMenuItemForAllPlayers(acceptButton);
    }
    public void SwitchToMainMenu(){
        CloseMenus();
        mainDisplay.SetActive(true);
        helpDisplay.SetActive(true);
        SetActiveMenuItemForAllPlayers(playButton);
    }
    public void SwitchToCreditsView(){
        CloseMenus();
        creditsDisplay.SetActive(true);
        SetActiveMenuItemForAllPlayers(backButton);
    }
    public void AcceptSideSelection(){
        masterStateMachine.SetTrigger("BeginGame");
        CloseMenus();
    }
    private void Start() {
        SetActiveMenuItemForAllPlayers(playButton);
    }
    public void QuitGame(){
        Application.Quit();
    }
}
