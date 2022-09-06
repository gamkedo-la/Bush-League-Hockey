using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using TMPro;

public class InGameMenu : MonoBehaviour
{
    [Header("In Game Menu")]
    [SerializeField] public GameObject gameMenuButtonPanel;
    [SerializeField] public GameObject endOfGameDisplay;
    [SerializeField] public GameObject controlsHelpDisplay;
    [SerializeField] public GameObject rematchButton;
    [Header("Choose Sides Menu")]
    [SerializeField] public GameObject chooseSidesMenu;
    [SerializeField] public GameObject acceptButton;
    [SerializeField] public GameObject backButton;
    [Header("Stats Menu")]
    [SerializeField] public GameObject gameStatsDisplay;
    [SerializeField] public GameObject backFromStatsButton;
    [SerializeField] TextMeshProUGUI homeHitsText;
    [SerializeField] TextMeshProUGUI awayHitsText;
    [SerializeField] TextMeshProUGUI homePassesText;
    [SerializeField] TextMeshProUGUI awayPassesText;

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
        gameStatsDisplay.SetActive(false);
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
    public void SwitchToGameStats(){
        gameStatsDisplay.SetActive(true);
        chooseSidesMenu.SetActive(false);
        gameMenuButtonPanel.SetActive(false);
        FindObjectOfType<GameSystem>().SetAllActionMapsToUI();
        SetStatisticsDisplayValues();
        SetActiveMenuItemForAllPlayers(backFromStatsButton);
    }
    public void SwitchToGameMenu(){
        gameMenuButtonPanel.SetActive(true);
        chooseSidesMenu.SetActive(false);
        gameStatsDisplay.SetActive(false);
        SetActiveMenuItemForAllPlayers(rematchButton);
    }
    public void SwitchToEndGameMenu(){
        gameMenuButtonPanel.SetActive(true);
        endOfGameDisplay.SetActive(true);
        controlsHelpDisplay.SetActive(false);
        chooseSidesMenu.SetActive(false);
        gameStatsDisplay.SetActive(false);
        SetActiveMenuItemForAllPlayers(rematchButton);
    }
    public void AcceptTeamSelectionChanges(){
        gameMenuButtonPanel.SetActive(false);
        chooseSidesMenu.SetActive(false);
        FindObjectOfType<GameSystem>().SetPlayersToTeams();
        SwitchToGameMenu();
    }
    public void SetStatisticsDisplayValues(){
        homeHitsText.text = FindObjectOfType<GameSystem>().homeHits.ToString();
        awayHitsText.text = FindObjectOfType<GameSystem>().awayHits.ToString();
        homePassesText.text = FindObjectOfType<GameSystem>().homePasses.ToString();
        awayPassesText.text = FindObjectOfType<GameSystem>().awayPasses.ToString();
    }
}
