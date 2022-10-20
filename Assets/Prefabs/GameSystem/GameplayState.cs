using UnityEngine;
[CreateAssetMenu(fileName = "GameplayState", menuName = "ScriptableObjects/GameplayState", order = 1)]
public class GameplayState : ScriptableObject
{
    // contains the data to recreate a moment in the game
    public GameplaySingleFrameData frameData;
    public int currentPeriod = 1;
    public float gameClockTime = 300f;
    public int homeScore = 0;
    public int awayScore = 0;
}
