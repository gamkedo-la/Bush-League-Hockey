using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsMenuScript : MonoBehaviour
{
    [SerializeField] GameplayState currentGameplayState;
    [Header("Stats Menu")]
    [SerializeField] TextMeshProUGUI homeHitsText;
    [SerializeField] TextMeshProUGUI awayHitsText;
    [SerializeField] TextMeshProUGUI homePassesText;
    [SerializeField] TextMeshProUGUI awayPassesText;
    [SerializeField] TextMeshProUGUI homeShotsText;
    [SerializeField] TextMeshProUGUI awayShotsText;
    [SerializeField] TextMeshProUGUI homeSavesText;
    [SerializeField] TextMeshProUGUI awaySavesText;
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
    void FixedUpdate()
    {
        SetStatisticsDisplayValues();
    }
}
