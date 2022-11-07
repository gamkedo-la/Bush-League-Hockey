using System;
using TMPro;
using UnityEngine;
public class ScoreKeeper : MonoBehaviour
{
    [SerializeField] public GameplayState currentGameplayState;
    [SerializeField] TextMeshProUGUI gameTimerText;
    [SerializeField] TextMeshProUGUI homeScoreText;
    [SerializeField] TextMeshProUGUI awayScoreText;
    private string minutes;
    private string seconds;
    private void Awake(){
        GameOnState.onStateEnter += HandleGameOn;
        CountGoals.awayGoalScored += AwayGoalScored;
        CountGoals.homeGoalScored += HomeGoalScored;
        RunClockState.timerDone += EndOfGame; 
        SuddenDeathMessage.onStateEnter += SuddenDeath;
    }
    public void HandleGameOn(object sender, EventArgs e)
    {
        ScoreUpdate();
        RunClockState.onStateUpdate += ClockUpdate;
    }
    public void AwayGoalScored(object sender, EventArgs e)
    {
        currentGameplayState.awayScore++;
        ScoreUpdate();
    }
    public void HomeGoalScored(object sender, EventArgs e)
    {
        currentGameplayState.homeScore++;
        ScoreUpdate();
    }
    public void EndOfGame(object sender, EventArgs e)
    {
        RunClockState.onStateUpdate -= ClockUpdate;
        gameTimerText.text = "final";
    }
    public void SuddenDeath(object sender, EventArgs e)
    {
        RunClockState.onStateUpdate -= ClockUpdate;
        gameTimerText.text = "sudden death";
    }
    public void ScoreUpdate()
    {
        awayScoreText.text = currentGameplayState.awayScore.ToString();
        homeScoreText.text = currentGameplayState.homeScore.ToString();
    }
    private void ClockUpdate(object sender, EventArgs e) {
        minutes = ((int)(currentGameplayState.gameClockTime/60)).ToString();
        seconds = currentGameplayState.gameClockTime % 60 < 10 ?
            $"0{(int)currentGameplayState.gameClockTime % 60}" :
            $"{(int)currentGameplayState.gameClockTime % 60}";
        gameTimerText.text = $"{minutes}:{seconds}";
    }
}
