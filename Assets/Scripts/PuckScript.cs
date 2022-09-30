using System;
using UnityEngine;

public class PuckScript : MonoBehaviour
{
    private AudioManager audioManager;
    private Rigidbody puckRigidbody;
    [SerializeField] GameObject puckLocationIndicator;

    private void Awake(){
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        puckRigidbody = GetComponent<Rigidbody>();
    }
    private void OnCollisionEnter(Collision other) {
        float volumeFactor = puckRigidbody.velocity.magnitude / 10;
        if (other.gameObject.name == "Posts") {
            audioManager.PlayPostHitSound(volumeFactor);
        }
        if (other.gameObject.name.Contains("Board") || other.gameObject.name.Contains("Corner")) {
            audioManager.PlayPuckOnBoardSound(volumeFactor);
        }
        if (other.gameObject.name.Contains("DEF") ||  other.gameObject.name.Contains("Goaltender")) {
            StartCoroutine(audioManager.PlayPuckPlayerHitSound(volumeFactor*3));
        }
        if(other.gameObject.name.Contains("Goaltender")){
            FindObjectOfType<GameSystem>().CountSave(other.gameObject.tag.Contains("home"));
        }
    }
    private void Update() {
        // position a VFX object directly underneath the puck
        puckLocationIndicator.transform.position = new Vector3(
            transform.position.x,
            0f,
            transform.position.z
        );
    }
}
