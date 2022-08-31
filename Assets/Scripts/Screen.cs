using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screen : MonoBehaviour
{
    private UnityEngine.Video.VideoPlayer videoPlayer;
    private void Awake() {
        videoPlayer = GetComponent<UnityEngine.Video.VideoPlayer>();
    }
    void Start()
    {
        float timelinePosition = Random.Range(0, (float)videoPlayer.length);
        videoPlayer.time = timelinePosition;
    }
}
