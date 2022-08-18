using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelCollisionSfx : MonoBehaviour
{
    private AudioManager audioManager;
    public bool collisionSfxEnabled = false;
    private void Awake() {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }
    private void OnCollisionEnter(Collision other) {
        if(collisionSfxEnabled){
            float volumeFactor = GetComponent<Rigidbody>().velocity.magnitude / 3;
            StartCoroutine(audioManager.PlayModelCollisionSound(volumeFactor));
        }
    }
}
