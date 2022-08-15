using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("SFX")]
    [SerializeField] [Range(0.0f, 2f)] public float universalSfxVolume;
    [SerializeField] [Range(0.0f, 4f)] public float worldSfxVolume;
    [SerializeField] [Range(0.0f, 1f)] public float crowdSfxVolume;
    [SerializeField] GameObject sfxUniversalOrigin;
    [SerializeField] GameObject sfxWorldOrigin;
    [SerializeField] GameObject sfxCrowdOrigin;
    [Header("SFX Files")]
    [SerializeField] public AudioClip[] passSfx;
    [SerializeField] public AudioClip[] bodycheckSfx;
    [SerializeField] public AudioClip[] puckOnPostSfx;
    [SerializeField] public AudioClip[] puckOnBoardSfx;
    [SerializeField] public AudioClip[] shotSfx;
    [SerializeField] public AudioClip goalHornSfx;
    [SerializeField] public AudioClip faceOffSfx;
    private void Start(){
        // start the crowd sfx
        // coroutine to play random crowd sfx
    }
    public void PlayPassSFX(){
        int randomSFXIndex = Random.Range(0, passSfx.Length - 1);
        sfxWorldOrigin.GetComponent<AudioSource>().PlayOneShot(passSfx[randomSFXIndex], worldSfxVolume);
    }
    public void PlayShotSFX(){
        int randomSFXIndex = Random.Range(0, shotSfx.Length - 1);
        sfxWorldOrigin.GetComponent<AudioSource>().PlayOneShot(shotSfx[randomSFXIndex], worldSfxVolume);
    }
    public void PlayBodycheckSFX(){
        int randomSFXIndex = Random.Range(0, bodycheckSfx.Length - 1);
        sfxWorldOrigin.GetComponent<AudioSource>().PlayOneShot(bodycheckSfx[randomSFXIndex], worldSfxVolume);
    }
    public void PlayGoalHorn(){
        sfxUniversalOrigin.GetComponent<AudioSource>().PlayOneShot(goalHornSfx, universalSfxVolume);
    }
    public void PlayFaceOffSound(){
        sfxUniversalOrigin.GetComponent<AudioSource>().PlayOneShot(faceOffSfx, universalSfxVolume);
    }
    public void PlayPostHitSound(){
        int randomSFXIndex = Random.Range(0, puckOnPostSfx.Length - 1);
        sfxWorldOrigin.GetComponent<AudioSource>().PlayOneShot(puckOnPostSfx[randomSFXIndex], worldSfxVolume);
    }
    public void PlayPuckOnBoardSound(){
        int randomSFXIndex = Random.Range(0, puckOnBoardSfx.Length - 1);
        sfxWorldOrigin.GetComponent<AudioSource>().PlayOneShot(puckOnBoardSfx[randomSFXIndex], worldSfxVolume);
    }
}
