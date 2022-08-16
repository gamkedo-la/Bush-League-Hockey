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
    private void PlayRandomlyFromList(AudioClip[] soundList, AudioSource origin, float volumeFactor){
        int randomSFXIndex = Random.Range(0, soundList.Length - 1);
        origin.PlayOneShot(soundList[randomSFXIndex], volumeFactor);
    }
    public void PlayPassSFX(){
        PlayRandomlyFromList(passSfx, sfxWorldOrigin.GetComponent<AudioSource>(), worldSfxVolume);
    }
    public void PlayShotSFX(){
        PlayRandomlyFromList(shotSfx, sfxWorldOrigin.GetComponent<AudioSource>(), worldSfxVolume);
    }
    public void PlayBodycheckSFX(){
        PlayRandomlyFromList(bodycheckSfx, sfxWorldOrigin.GetComponent<AudioSource>(), worldSfxVolume);
    }
    public void PlayGoalHorn(){
        sfxUniversalOrigin.GetComponent<AudioSource>().PlayOneShot(goalHornSfx, universalSfxVolume);
    }
    public void PlayFaceOffSound(){
        sfxUniversalOrigin.GetComponent<AudioSource>().PlayOneShot(faceOffSfx, universalSfxVolume);
    }
    public void PlayPostHitSound(float volumeFactor){
        PlayRandomlyFromList(puckOnPostSfx, sfxWorldOrigin.GetComponent<AudioSource>(), volumeFactor);
    }
    public void PlayPuckOnBoardSound(float volumeFactor){
        PlayRandomlyFromList(puckOnBoardSfx, sfxWorldOrigin.GetComponent<AudioSource>(), volumeFactor);
    }
}
