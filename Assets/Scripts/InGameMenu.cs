using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;

public class InGameMenu : MonoBehaviour
{
    [Header("In Game Menu")]
    [SerializeField] public GameObject gameMenuButtonPanel;
    [SerializeField] public GameObject rematchButton;
    [Header("Choose Sides Menu")]
    [SerializeField] public GameObject chooseSidesMenu;
    [SerializeField] public GameObject acceptButton;
    [SerializeField] public GameObject backButton;
    public void SetActiveMenuItemForAllPlayers(GameObject menuItem){
        foreach (MultiplayerEventSystem eventSystem in FindObjectsOfType<MultiplayerEventSystem>())
        {
            eventSystem.SetSelectedGameObject(menuItem);
        }
    }
    public void HandleResume(){
        gameMenuButtonPanel.SetActive(false);
        chooseSidesMenu.SetActive(false);
        FindObjectOfType<GameSystem>().SetAllActionMapsToPlayer();
    }
    public void HandlePause(){
        gameMenuButtonPanel.SetActive(true);
        SetActiveMenuItemForAllPlayers(rematchButton);
    }
    public void SwitchToChooseSideMenu(){
        gameMenuButtonPanel.SetActive(false);
        chooseSidesMenu.SetActive(true);
        FindObjectOfType<GameSystem>().SetAllActionMapsToUI();
        SetActiveMenuItemForAllPlayers(acceptButton);
    }
    public void SwitchToGameMenu(){
        gameMenuButtonPanel.SetActive(true);
        chooseSidesMenu.SetActive(false);
        SetActiveMenuItemForAllPlayers(rematchButton);
    }
    public void AcceptTeamSelectionChanges(){
        gameMenuButtonPanel.SetActive(false);
        chooseSidesMenu.SetActive(false);
        FindObjectOfType<GameSystem>().SetPlayersToTeams();
        SwitchToGameMenu();
    }
}
