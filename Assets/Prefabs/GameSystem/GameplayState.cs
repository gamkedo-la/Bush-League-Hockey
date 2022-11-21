using UnityEngine;
[CreateAssetMenu(fileName = "GameplayState", menuName = "ScriptableObjects/GameplayState", order = 1)]
public class GameplayState : ScriptableObject
{
    // contains the data to recreate a moment in the game
    public GameplaySingleFrameData frameData;
    public int currentPeriod = 1;
    public float gameClockTime = 360f;
    public int homeScore = 0;
    public int awayScore = 0;
    public int homeHits = 0;
    public int awayHits = 0;
    public int homeSaves = 0;
    public int awaySaves = 0;
    public int homePasses = 0;
    public int awayPasses = 0;
    public void ResetStats(){
        currentPeriod = 1;
        homeScore = 0;
        awayScore = 0;
        homeHits = 0;
        awayHits = 0;
        homeSaves = 0;
        awaySaves = 0;
        homePasses = 0;
        awayPasses = 0;
    }
}
