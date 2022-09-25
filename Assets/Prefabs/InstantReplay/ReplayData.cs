using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ReplayData", menuName = "ScriptableObjects/ReplayData", order = 1)]
public class ReplayData : ScriptableObject
{
    // Replay config values here
    public float timeSinceFrameWasSwitched;
}
