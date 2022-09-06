using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
public class Screen : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    [SerializeField] public VideoClip[] videoClips;
    private void Awake() {
        videoPlayer = GetComponent<VideoPlayer>();
    }
    private void PlayRandomGameplayClip(){
        int clipIndex = Random.Range(0, videoClips.Length);
        videoPlayer.clip = videoClips[clipIndex];
        videoPlayer.time = Random.Range(0f, (float)videoPlayer.length/2f);
        videoPlayer.Play();
    }
    void Start()
    {
        PlayRandomGameplayClip();
    }
}
