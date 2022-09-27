using UnityEngine;
[CreateAssetMenu(fileName = "GameplayState", menuName = "ScriptableObjects/GameplayState", order = 1)]
public class GameplayState : ScriptableObject
{
    // contains the data to recreate a moment in the game
    public GameplaySingleFrameData frame;
    public float gameClockTime;
}
