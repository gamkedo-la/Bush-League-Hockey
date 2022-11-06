using System.Collections;
using System;
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
    [SerializeField] public AudioClip[] trashTalkSfx;
    [SerializeField] public AudioClip[] hitBySmallObjectSfx;
    [SerializeField] public AudioClip[] modelCollisionSfx;
    private bool modelCollisionSoundReady = true;
    private bool trashTalkReady = true;
    private float woodWhistleStartTime = 0f;
    [SerializeField] public AudioClip[] shotSfx;
    [SerializeField] public AudioClip woodWhistleSfx;

    [SerializeField] public AudioClip suddenDeathSfx;

    [SerializeField] public AudioClip[] ReadySfx;

    [SerializeField] public AudioClip[] GoSfx;
    [Header("SFX Files")]
    [SerializeField] public AudioClip[] songs;
    private void Awake() {
        CountGoals.awayGoalScored += GoalScored;
        CountGoals.homeGoalScored += GoalScored;
        FaceOffState.onStateEnter += PlayReadySound;
        FaceOffState.onStateExit += PlayFaceOffSound;
        EOGSetup.onStateEnter += HandleEndOfGame;
        RunClockState.timerDone += PlayWoodWhistle;
        SuddenDeathMessage.onStateEnter += PlaySuddenDeath;
    }
    public void EndOfGameCelebration(object sender, EventArgs e){
        PlayGoalHorn();
        PlayCrowdCelebration();
        BigCelebration.celebrate -= EndOfGameCelebration;
    }
    public void GoalScored(object sender, EventArgs e){
        PlayGoalHorn();
        PlayCrowdCelebration();
    }
    public void HandleEndOfGame(object sender, EventArgs e)
    {
        BigCelebration.celebrate += EndOfGameCelebration;
    }
    private void PlayRandomlyFromList(AudioClip[] soundList, GameObject origin, float volumeFactor){
        int randomSFXIndex = UnityEngine.Random.Range(0, soundList.Length);
        origin.GetComponent<AudioSource>().PlayOneShot(soundList[randomSFXIndex], volumeFactor);
    }
    private void SetRandomTrackToOrigin(AudioClip[] soundList, GameObject origin, float volumeFactor){
        int randomSFXIndex = UnityEngine.Random.Range(0, soundList.Length);
        origin.GetComponent<AudioSource>().clip = soundList[randomSFXIndex];
        origin.GetComponent<AudioSource>().Play(0);
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
        PlayRandomlyFromList(crowdCelebrationTracks, sfxCrowdOrigin, crowdSfxVolume*3);
    }
    public void PlayFaceOffSound(object sender, EventArgs e){
        PlayRandomlyFromList(GoSfx, sfxUniversalOrigin, universalSfxVolume*9);
    }
    public void PlayWoodWhistle(object sender, EventArgs e){
        if(Time.time - woodWhistleStartTime >= 5f){
            woodWhistleStartTime = Time.time;
            sfxWorldOrigin.GetComponent<AudioSource>().PlayOneShot(woodWhistleSfx, worldSfxVolume);
        }
    }
     public void PlayReadySound(object sender, EventArgs e){
        PlayRandomlyFromList(ReadySfx, sfxUniversalOrigin, universalSfxVolume*5);
    }
    public void PlaySuddenDeath(object sender, EventArgs e){
        sfxUniversalOrigin.GetComponent<AudioSource>().PlayOneShot(suddenDeathSfx, universalSfxVolume*10);
    }
    private IEnumerator CooldownTrashTalk(){
        trashTalkReady = false;
        yield return new WaitForSeconds(5);
        trashTalkReady = true;
    }
    public void PlayTrashTalk(){
        if(trashTalkReady){
            PlayRandomlyFromList(trashTalkSfx, sfxWorldOrigin, worldSfxVolume*5);
            StartCoroutine(CooldownTrashTalk());
        }
    }
    public IEnumerator PlayPuckPlayerHitSound(float volumeFactor){
        PlayRandomlyFromList(puckOnPlayerSfx, sfxWorldOrigin, volumeFactor);
        yield return new WaitForSeconds(0.2f);
        PlayRandomlyFromList(hitBySmallObjectSfx, sfxWorldOrigin, volumeFactor);
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
