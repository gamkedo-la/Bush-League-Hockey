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
    [SerializeField] public GameObject scoreBoardDisplay;
    [SerializeField] public GameObject goalScoredDisplay;
    [SerializeField] public GameObject faceOffCountdownDisplay;
    [SerializeField] TextMeshProUGUI faceOffCountdownText;
    [SerializeField] public GameObject suddenDeathDisplay;
    [SerializeField] public GameObject endOfGameMenu;
    [SerializeField] public GameObject endOfGameHomeScoreBox;
    [SerializeField] public GameObject endOfGameAwayScoreBox;
    [SerializeField] TextMeshProUGUI endOfGameHomeScoreText;
    [SerializeField] TextMeshProUGUI endOfGameAwayScoreText;
    [SerializeField] public GameObject endOfGameHomeWinnerTag;
    [SerializeField] public GameObject endOfGameAwayWinnerTag;
    private void Awake(){
        MainMenuState.onStateEnter += HandleMainMenuEnter;
        BeginGameState.onStateEnter += HandleGameOn;
        GoalScoredState.onStateEnter += GoalDisplay;
        FaceOffState.onStateEnter += HandleFaceOffEnter;
        FaceOffState.onStateUpdate += HandleFaceOffUpdate;
        FaceOffState.onStateExit += HandleGameOn;
        SuddenDeathMessage.onStateEnter += SuddenDeathDisplay;
        EOGSetup.onStateEnter += SwitchToEndGameMenu;
        ShowScores.onStateEnter += ShowHomeScore;
        ShowScores.onStateExit += ShowAwayScore;
        BigCelebration.celebrate += ShowWinner;
    }
    public void HandleMainMenuEnter(object sender, EventArgs e)
    {
        HideMenus();
    }
    public void HandleGameOn(object sender, EventArgs e)
    {
        HideMenus();
        scoreBoardDisplay.SetActive(true);
    }
    public void HideMenus()
    {
        controlsHelpDisplay.SetActive(false);
        chooseSidesMenu.SetActive(false);
        endOfGameMenu.SetActive(false);
        endOfGameHomeScoreBox.SetActive(false);
        endOfGameAwayScoreBox.SetActive(false);
        endOfGameHomeWinnerTag.SetActive(false);
        endOfGameAwayWinnerTag.SetActive(false);
        faceOffCountdownDisplay.SetActive(false);
        gameMenuButtonPanel.SetActive(false);
        gameStatsDisplay.SetActive(false);
        scoreBoardDisplay.SetActive(false);
    }
    public void SuddenDeathDisplay(object sender, EventArgs e)
    {
        StartCoroutine(FlashingOnScreenMessage(suddenDeathDisplay, 10));
    }
    public void GoalDisplay(object sender, EventArgs e)
    {
        StartCoroutine(FlashingOnScreenMessage(goalScoredDisplay, 6));
    }
    public void HandleFaceOffEnter(object sender, EventArgs e)
    {
        HideMenus();
        faceOffCountdownDisplay.SetActive(true);
    }
    public void HandleFaceOffUpdate(object sender, FaceOffEventArgs e)
    {
        faceOffCountdownText.text = $"{(int)e.countdownTimer}";
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
    private void UpdateTextFields(){

    }
    public void ShowHomeScore(object sender, EventArgs e){
        endOfGameHomeScoreBox.SetActive(true);
        endOfGameHomeScoreText.text = currentGameplayState.homeScore.ToString();
    }
    public void ShowAwayScore(object sender, EventArgs e){
        endOfGameAwayScoreBox.SetActive(true);
        endOfGameAwayScoreText.text = currentGameplayState.awayScore.ToString();
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
        homeHitsText.text = currentGameplayState.homeHits.ToString();
        awayHitsText.text = currentGameplayState.awayHits.ToString();
        homePassesText.text = currentGameplayState.homePasses.ToString();
        awayPassesText.text = currentGameplayState.awayPasses.ToString();
        homeShotsText.text = (currentGameplayState.homeScore + currentGameplayState.awaySaves).ToString();
        awayShotsText.text = (currentGameplayState.awayScore + currentGameplayState.homeSaves).ToString();
        homeSavesText.text = currentGameplayState.homeSaves.ToString();
        awaySavesText.text = currentGameplayState.awaySaves.ToString();
    }
}
