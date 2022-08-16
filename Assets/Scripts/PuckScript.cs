using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuckScript : MonoBehaviour
{
    private AudioManager audioManager;
    private Rigidbody puckRigidbody;
    private void Awake() {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        puckRigidbody = GetComponent<Rigidbody>();
    }
    private void OnCollisionEnter(Collision other) {
        float volumeFactor = puckRigidbody.velocity.magnitude / 10;
        Debug.Log($"volumeFactor: {volumeFactor}");
        if (other.gameObject.name == "Posts") {
            audioManager.PlayPostHitSound();
        }
        if (other.gameObject.name.Contains("Board") || other.gameObject.name.Contains("Corner")) {
            audioManager.PlayPuckOnBoardSound();
        }
    }
}
