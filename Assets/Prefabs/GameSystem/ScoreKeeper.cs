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
        Goal.awayGoalScored += AwayGoalScored;
        Goal.homeGoalScored += HomeGoalScored;
    }
    public void AwayGoalScored(object sender, EventArgs e)
    {
        Debug.Log($"Scorekeeper - AwayGoal");
        currentGameplayState.awayScore++;
        awayScoreText.text = currentGameplayState.awayScore.ToString();
        //StartCoroutine(crowdReactionManager.transform.GetComponent<CrowdReactionManagerScriptComponent>().HandleAwayTeamScoringAGoal());
        //gameSystem.GoalScored(true);
    }
    public void HomeGoalScored(object sender, EventArgs e)
    {
        Debug.Log($"Scorekeeper - AwayGoal");
        currentGameplayState.homeScore++;
        homeScoreText.text = currentGameplayState.homeScore.ToString();
        //gameSystem.GoalScored(false);
    }
    void FixedUpdate()
    {
        minutes = ((int)(currentGameplayState.gameClockTime/60)).ToString();
        if((int)currentGameplayState.gameClockTime % 60 < 10){
            seconds = $"0{(int)currentGameplayState.gameClockTime % 60}";
        } else {
            seconds = $"{(int)currentGameplayState.gameClockTime % 60}";
        }
        gameTimerText.text = $"{minutes}:{seconds}";
    }
}
