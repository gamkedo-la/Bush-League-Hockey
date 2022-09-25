using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TimeProvider", menuName = "ScriptableObjects/TimeProvider", order = 1)]
public class TimeProvider : ScriptableObject
{
    public float time;
    public float deltaTime;
    public float fixedDeltaTime;
    [Range(0.05f, 1.5f)] public float timeScale;
}
