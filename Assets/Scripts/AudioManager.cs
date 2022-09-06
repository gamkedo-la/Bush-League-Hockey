using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] [Range(0.0f, 2f)] public float universalSfxVolume;
    [SerializeField] [Range(0.0f, 4f)] public float worldSfxVolume;
    [SerializeField] [Range(0.0f, 1f)] public float crowdSfxVolume;
    [SerializeField] GameObject sfxUniversalOrigin;
     [SerializeField] GameObject sfxWorldOrigin;
    [SerializeField] GameObject sfxCrowdOrigin;
    [Header("SFX Files")]
    [SerializeField] public AudioClip[] crowdNormalTracks;
    [SerializeField] public AudioClip[] crowdCelebrationTracks;
    [SerializeField] public AudioClip[] goalHornSfx;
    [SerializeField] public AudioClip[] takePosessionSfx;
    [SerializeField] public AudioClip[] passSfx;
    [SerializeField] public AudioClip[] passAnnounceSfx;
    [SerializeField] public AudioClip[] deliverCheckSfx;
    [SerializeField] public AudioClip[] bodycheckSfx;
    [SerializeField] public AudioClip[] deliveryCheckGruntSfx;
    [SerializeField] public AudioClip[] puckOnPostSfx;
    [SerializeField] public AudioClip[] puckOnBoardSfx;
    [SerializeField] public AudioClip[] puckOnPlayerSfx;
    [SerializeField] public AudioClip[] OuchSfx;
    [SerializeField] public AudioClip[] modelCollisionSfx;
    private bool modelCollisionSoundReady = true;
    [SerializeField] public AudioClip[] shotSfx;
    [SerializeField] public AudioClip faceOffSfx;

    [SerializeField] public AudioClip suddenDeathSfx;

    [SerializeField] public AudioClip[] ReadySfx;

    [SerializeField] public AudioClip[] GoSfx;
    [Header("SFX Files")]
    [SerializeField] public AudioClip[] songs;
    private void PlayRandomlyFromList(AudioClip[] soundList, GameObject origin, float volumeFactor){
        int randomSFXIndex = Random.Range(0, soundList.Length);
        origin.GetComponent<AudioSource>().PlayOneShot(soundList[randomSFXIndex], volumeFactor);
    }
    public IEnumerator PlayPassSFX(){
        PlayRandomlyFromList(passSfx, sfxWorldOrigin, worldSfxVolume);
        yield return new WaitForSeconds(0.2f);
        PlayRandomlyFromList(passAnnounceSfx, sfxWorldOrigin, worldSfxVolume);
    }
    public void PlayTakePossessionSFX(){
        PlayRandomlyFromList(takePosessionSfx, sfxWorldOrigin, worldSfxVolume);
    }
    public void PlayShotSFX(){
        PlayRandomlyFromList(shotSfx, sfxWorldOrigin, worldSfxVolume);
    }
     public void PlayBodyCheckGrunt(){
        PlayRandomlyFromList(deliveryCheckGruntSfx, sfxWorldOrigin, worldSfxVolume);
    }
    public IEnumerator PlayBodycheckHitAndReaction(){
        PlayRandomlyFromList(deliverCheckSfx, sfxWorldOrigin, worldSfxVolume*4);
        yield return new WaitForSeconds(0.2f);
        PlayRandomlyFromList(bodycheckSfx, sfxWorldOrigin, worldSfxVolume);
    }
    public void PlayBaseCrowdTrack(){
        PlayRandomlyFromList(crowdNormalTracks, sfxCrowdOrigin, crowdSfxVolume);
    }
    public void PlaySong(){
        PlayRandomlyFromList(songs, sfxUniversalOrigin, universalSfxVolume*0.5f);
    }
    public void PlayGoalHorn(){
        PlayRandomlyFromList(goalHornSfx, sfxUniversalOrigin, universalSfxVolume*0.5f);
    }
    public void PlayCrowdCelebration(){
        PlayRandomlyFromList(crowdCelebrationTracks, sfxCrowdOrigin, crowdSfxVolume*5);
    }
    public void PlayFaceOffSound(){
        PlayRandomlyFromList(GoSfx, sfxUniversalOrigin, universalSfxVolume*9);
    }

     public void PlayReadySound(){
        PlayRandomlyFromList(ReadySfx, sfxUniversalOrigin, universalSfxVolume*6);
    }
    public void PlaySuddenDeath (){
        sfxUniversalOrigin.GetComponent<AudioSource>().PlayOneShot(suddenDeathSfx, universalSfxVolume*10);
    }
    public IEnumerator PlayPuckPlayerHitSound(float volumeFactor){
        PlayRandomlyFromList(puckOnPlayerSfx, sfxWorldOrigin, volumeFactor);
        yield return new WaitForSeconds(0.2f);
        PlayRandomlyFromList(OuchSfx, sfxWorldOrigin, volumeFactor);
    }
    public void PlayPostHitSound(float volumeFactor){
        PlayRandomlyFromList(puckOnPostSfx, sfxWorldOrigin, volumeFactor);
    }
    public IEnumerator PlayModelCollisionSound(float volumeFactor){
        if(modelCollisionSoundReady){
            modelCollisionSoundReady = false;
            PlayRandomlyFromList(modelCollisionSfx, sfxWorldOrigin, volumeFactor);
            yield return new WaitForSeconds(0.1f);
            modelCollisionSoundReady = true;
        }
    }
    public void PlayPuckOnBoardSound(float volumeFactor){
        PlayRandomlyFromList(puckOnBoardSfx, sfxWorldOrigin, volumeFactor);
    }
}
