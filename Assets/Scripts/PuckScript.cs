using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuckScript : MonoBehaviour
{
    private AudioManager audioManager;
    private void Awake() {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }
    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.name == "Posts") {
            audioManager.PlayPostHitSound();
        }
        if (other.gameObject.name.Contains("Board") || other.gameObject.name.Contains("Corner")) {
            audioManager.PlayPuckOnBoardSound();
        }
    }
}
