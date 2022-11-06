using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using TMPro;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] GameplayState currentGameplayState;
    [Header("In Game Menu")]
    [SerializeField] public GameObject gameMenuButtonPanel;
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
    [SerializeField] TextMeshProUGUI homeShotsText;
    [SerializeField] TextMeshProUGUI awayShotsText;
    [SerializeField] TextMeshProUGUI homeSavesText;
    [SerializeField] TextMeshProUGUI awaySavesText;
    [Header("Onscreen Messages")]
    [SerializeField] public GameObject suddenDeathDisplay;
    [SerializeField] public GameObject endOfGameMenu;
    [SerializeField] public GameObject endOfGameHomeScoreBox;
    [SerializeField] public GameObject endOfGameAwayScoreBox;
    [SerializeField] TextMeshProUGUI endOfGameHomeScoreText;
    [SerializeField] TextMeshProUGUI endOfGameAwayScoreText;
    [SerializeField] public GameObject endOfGameHomeWinnerTag;
    [SerializeField] public GameObject endOfGameAwayWinnerTag;
    private void Awake() {
        BeginGameState.onStateEnter += HideMenus;
        SuddenDeathMessage.onStateEnter += SuddenDeathDisplay;
        EOGSetup.onStateEnter += SwitchToEndGameMenu;
        ShowScores.onStateEnter += ShowHomeScore;
        ShowScores.onStateExit += ShowAwayScore;
        BigCelebration.celebrate += ShowWinner;
    }
    public void HideMenus(object sender, EventArgs e)
    {
        endOfGameMenu.SetActive(false);
        endOfGameHomeScoreBox.SetActive(false);
        endOfGameAwayScoreBox.SetActive(false);
        endOfGameHomeWinnerTag.SetActive(false);
        endOfGameAwayWinnerTag.SetActive(false);
        gameMenuButtonPanel.SetActive(true);
        controlsHelpDisplay.SetActive(false);
        chooseSidesMenu.SetActive(false);
        gameStatsDisplay.SetActive(false); 
    }
    public void SuddenDeathDisplay(object sender, EventArgs e)
    {
        StartCoroutine(FlashingOnScreenMessage(suddenDeathDisplay, 10));
    }
    public IEnumerator FlashingOnScreenMessage(GameObject messageDisplay, int cycles){
        for (int i = 0; i < cycles; i++){
            messageDisplay.SetActive(true);
            yield return new WaitForSeconds(0.15f);
            messageDisplay.SetActive(false);
            yield return new WaitForSeconds(0.075f);
        }
    }
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
    public void SwitchToEndGameMenu(object sender, EventArgs e){
        endOfGameMenu.SetActive(true);
        endOfGameHomeScoreBox.SetActive(false);
        endOfGameAwayScoreBox.SetActive(false);
        endOfGameHomeWinnerTag.SetActive(false);
        endOfGameAwayWinnerTag.SetActive(false);
        gameMenuButtonPanel.SetActive(true);
        controlsHelpDisplay.SetActive(false);
        chooseSidesMenu.SetActive(false);
        gameStatsDisplay.SetActive(false);
        SetActiveMenuItemForAllPlayers(rematchButton);
    }
    public void ShowHomeScore(object sender, EventArgs e){
        endOfGameHomeScoreBox.SetActive(true);
    }
    public void ShowAwayScore(object sender, EventArgs e){
        endOfGameAwayScoreBox.SetActive(true);
    }
    public void ShowWinner(object sender, EventArgs e){
        if(currentGameplayState.homeScore > currentGameplayState.awayScore){
            endOfGameHomeWinnerTag.SetActive(true);
        } else {
            endOfGameAwayWinnerTag.SetActive(true);
        }
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
        homeShotsText.text = (FindObjectOfType<GameSystem>().homeScore + FindObjectOfType<GameSystem>().awaySaves).ToString();
        awayShotsText.text = (FindObjectOfType<GameSystem>().awayScore + FindObjectOfType<GameSystem>().homeSaves).ToString();
        homeSavesText.text = FindObjectOfType<GameSystem>().homeSaves.ToString();
        awaySavesText.text = FindObjectOfType<GameSystem>().awaySaves.ToString();
    }
}
