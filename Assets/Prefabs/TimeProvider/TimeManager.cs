using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public TimeProvider gameTime;
    public TimeProvider gameClock;
    public TimeProvider replayTime;
    public TimeProvider menuTime;
    private void FixedUpdate()
    {
        gameTime.fixedDeltaTime = Time.fixedDeltaTime * gameTime.timeScale;
        replayTime.fixedDeltaTime = Time.fixedDeltaTime * replayTime.timeScale;
    }
    void Update()
    {
        gameTime.deltaTime = Time.deltaTime * gameTime.timeScale;
        gameTime.time += Time.deltaTime * gameTime.timeScale;
        replayTime.deltaTime = Time.deltaTime * replayTime.timeScale;
        replayTime.time += Time.deltaTime * replayTime.timeScale;
    }
}
