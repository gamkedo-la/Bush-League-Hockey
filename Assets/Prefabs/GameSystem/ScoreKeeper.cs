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
    private void Awake() {
        // Subscribe to stats monitoring events
        CountGoals.awayGoalScored += AwayGoalScored;
        CountGoals.homeGoalScored += HomeGoalScored;
        RunClockState.onStateUpdate += ClockUpdate;
        RunClockState.timerDone += EndOfGame;
        SuddenDeathMessage.onStateEnter += SuddenDeath;
    }
    public void AwayGoalScored(object sender, EventArgs e)
    {
        Debug.Log($"AwayGoal");
        currentGameplayState.awayScore++;
        awayScoreText.text = currentGameplayState.awayScore.ToString();
        //StartCoroutine(crowdReactionManager.transform.GetComponent<CrowdReactionManagerScriptComponent>().HandleAwayTeamScoringAGoal());
        //gameSystem.GoalScored(true);
    }
    public void HomeGoalScored(object sender, EventArgs e)
    {
        Debug.Log($"HomeGoal");
        currentGameplayState.homeScore++;
        homeScoreText.text = currentGameplayState.homeScore.ToString();
        //gameSystem.GoalScored(false);
    }
    public void EndOfGame(object sender, EventArgs e)
    {
        gameTimerText.text = "final";
    }
    public void SuddenDeath(object sender, EventArgs e)
    {
        gameTimerText.text = "sudden death";
    }
    private void ClockUpdate(object sender, EventArgs e) {
        minutes = ((int)(currentGameplayState.gameClockTime/60)).ToString();
        seconds = currentGameplayState.gameClockTime % 60 < 10 ?
            $"0{(int)currentGameplayState.gameClockTime % 60}" :
            $"{(int)currentGameplayState.gameClockTime % 60}";
        gameTimerText.text = $"{minutes}:{seconds}";
    }
}
