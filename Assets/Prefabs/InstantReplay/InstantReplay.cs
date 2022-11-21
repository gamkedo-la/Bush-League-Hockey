using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class InstantReplay : MonoBehaviour
{
    public static EventHandler<EventArgs> replayEnd;
    [SerializeField] GameObject instantReplayGUI;
    [SerializeField] TimeProvider replayTime;
    [SerializeField] ReplayData replayData;
    [SerializeField] GameplayState beforeReplayState;
    private GameSystem gameSystem;
    private Queue<GameplaySingleFrameData> recordingFrameDataQueue = new Queue<GameplaySingleFrameData>();
    private GameplaySingleFrameData[] recordingFrameData;
    private GameplaySingleFrameData currentFrameData;
    private GameplaySingleFrameData nextFrameData;
    public Camera replayCamera;
    public Camera replayCamera2;
    public Camera replayCamera3;
    private List<Rigidbody> gamePieceRigidbodies;
    public Transform p1;
    public Rigidbody p1Rigidbody;
    public Transform g1;
    public Transform p2;
    public Rigidbody p2Rigidbody;
    public Transform g2;
    public Transform puck;
    public Rigidbody puckRigidbody;
    private TrailRenderer puckTrail;
    public Transform bonesRig1;
    public Transform bonesRig2;
    private Transform[] bones1;
    private Transform[] bones2;
    public bool playingBack = false;
    public int playbackFrame = 0;
    private void Awake() {
        gameSystem = FindObjectOfType<GameSystem>();
        p1 = gameSystem.homeSkater.transform;
        p1Rigidbody = p1.GetComponent<Rigidbody>();
        p2 = gameSystem.awaySkater.transform;
        p2Rigidbody = p2.GetComponent<Rigidbody>();
        g1 = gameSystem.homeGoaltender.transform;
        g2 = gameSystem.awayGoaltender.transform;
        puck = gameSystem.puckObject.transform;
        puckRigidbody = puck.GetComponent<Rigidbody>();
        puckTrail = puck.GetComponent<TrailRenderer>();
    }
    void Start()
    {
        GameOnState.onStateEnter += StartRecording;
        StateRecordsReplayData.onStateUpdate += RecordCurrentFrameData;
        InstantReplayState.onStateEnter += startInstantReplay;
        InstantReplayState.onStateUpdate += PlaybackUpdate;
        InstantReplayState.onStateExit += CancelReplay;
        PlayerController.replayTrigger += startInstantReplay;
        PlayerController.replayCancelTrigger += CancelReplay;
        bones1 = bonesRig1.GetComponentsInChildren<Transform>();
        bones2 = bonesRig2.GetComponentsInChildren<Transform>();
        //Debug.Log("Replay bones found: " + bones1.Length);
        // for (int i=0; i<bones1.Length; i++) {
        //     Debug.Log("Bone "+i+" is named "+bones1[i].name);
        // }
        gamePieceRigidbodies = GetGamePieceRigidbodies();
    }
    private void DisableAnimators()
    {
        p1.gameObject.GetComponentInChildren<Animator>().enabled = false;
        p2.gameObject.GetComponentInChildren<Animator>().enabled = false;
        //g1.gameObject.GetComponentInChildren<Animator>().enabled = false;
        //g2.gameObject.GetComponentInChildren<Animator>().enabled = false;
    }
    private void EnableAnimators()
    {
        p1.gameObject.GetComponentInChildren<Animator>().enabled = true;
        p2.gameObject.GetComponentInChildren<Animator>().enabled = true;
        //g1.gameObject.GetComponentInChildren<Animator>().enabled = true;
        //g2.gameObject.GetComponentInChildren<Animator>().enabled = true;
    }
    private List<Rigidbody> GetGamePieceRigidbodies()
    {
        List<Rigidbody> rigidbodies = new List<Rigidbody>();
        rigidbodies.Add(p1.gameObject.GetComponent<Rigidbody>());
        rigidbodies.Add(p2.gameObject.GetComponent<Rigidbody>());
        rigidbodies.Add(g1.gameObject.GetComponent<Rigidbody>());
        rigidbodies.Add(g2.gameObject.GetComponent<Rigidbody>());
        rigidbodies.Add(puck.gameObject.GetComponent<Rigidbody>());
        foreach (Rigidbody rb in bonesRig1.GetComponentsInChildren<Rigidbody>())
        {
            rigidbodies.Add(rb);
        }
        foreach (Rigidbody rb in bonesRig2.GetComponentsInChildren<Rigidbody>())
        {
            rigidbodies.Add(rb);
        }
        return rigidbodies;
    }
    private void ZeroRigidbodyVelocities()
    {
        foreach (Rigidbody rb in gamePieceRigidbodies)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
    public GameplaySingleFrameData GetCurrentFrameData(){
        currentFrameData = new GameplaySingleFrameData(
            p1.position, p2.position,
            p1.rotation, p2.rotation,
            g1.position, g2.position,
            g1.rotation, g2.rotation,
            puck.position, puck.rotation
        );
        // record all 30 or so bone pos+rot for each avatar
        for (int b=0; b<bones1.Length; b++){
            currentFrameData.bones1pos[b] = bones1[b].position; 
            currentFrameData.bones1rot[b] = bones1[b].rotation;
        }
        for (int b=0; b<bones2.Length; b++){
            currentFrameData.bones2pos[b] = bones2[b].position; 
            currentFrameData.bones2rot[b] = bones2[b].rotation;
        }
        return currentFrameData;
    }
    private void MapFrameDataToObjects(GameplaySingleFrameData frame){
        p1.position = frame.p1Position; p1.rotation = frame.p1Rotation;
        p2.position = frame.p2Position; p2.rotation = frame.p2Rotation;
        g1.position = frame.g1Position; g1.rotation = frame.g1Rotation;
        g2.position = frame.g2Position; g2.rotation = frame.g2Rotation;
        puck.position = frame.puckPosition; puck.rotation = frame.puckRotation;
        // Apply bone transforms for each Rig
        for (int b=0; b<bones1.Length; b++) {
            bones1[b].position = frame.bones1pos[b];
            bones1[b].rotation = frame.bones1rot[b];
        }
        for (int b=0; b<bones2.Length; b++) {
            bones2[b].position = frame.bones2pos[b]; 
            bones2[b].rotation = frame.bones2rot[b];
        }
        ZeroRigidbodyVelocities();
    }
    private void LerpObjectsTowardsNextFrame(){
        // interpolation amount = what fraction of fixedDeltaTime has passed?
        float interpolationPercent = replayTime.deltaTime / Time.fixedDeltaTime;
        Debug.Log($"interpolate: {interpolationPercent} = {replayTime.deltaTime}/{Time.fixedDeltaTime}");
        p1.position = Vector3.Lerp(p1.position, nextFrameData.p1Position, interpolationPercent);
        p1.rotation = Quaternion.Lerp(p1.rotation, nextFrameData.p1Rotation, interpolationPercent);
        p2.position = Vector3.Lerp(p2.position, nextFrameData.p2Position, interpolationPercent);
        p2.rotation = Quaternion.Lerp(p2.rotation, nextFrameData.p2Rotation, interpolationPercent);
        g1.position = Vector3.Lerp(g1.position, nextFrameData.g1Position, interpolationPercent);
        g1.rotation = Quaternion.Lerp(g1.rotation, nextFrameData.g1Rotation, interpolationPercent);
        g2.position = Vector3.Lerp(g2.position, nextFrameData.g2Position, interpolationPercent);
        g2.rotation = Quaternion.Lerp(g2.rotation, nextFrameData.g2Rotation, interpolationPercent);
        puck.position = Vector3.Lerp(puck.position, nextFrameData.puckPosition, interpolationPercent);
        puck.rotation = Quaternion.Lerp(puck.rotation, nextFrameData.puckRotation, interpolationPercent);
        for (int b=0; b<bones1.Length; b++) {
            bones1[b].position = Vector3.Lerp(bones1[b].position, nextFrameData.bones1pos[b], interpolationPercent);
            bones1[b].rotation = Quaternion.Lerp(bones1[b].rotation, nextFrameData.bones1rot[b], interpolationPercent);
        }
        for (int b=0; b<bones2.Length; b++) {
            bones2[b].position = Vector3.Lerp(bones2[b].position, nextFrameData.bones2pos[b], interpolationPercent);
            bones2[b].rotation = Quaternion.Lerp(bones2[b].rotation, nextFrameData.bones2rot[b], interpolationPercent);
        }
        ZeroRigidbodyVelocities();
    }
    void Update()
    {
        // press [R] for replay anytime
        if (playingBack){
            // Fixed update will apply transforms from recorded data
            // LateUpdate will lerp between current frame and next
            // theory not actually working in practice
            //LerpObjectsTowardsNextFrame();
            MapFrameDataToObjects(recordingFrameData[playbackFrame]);
            replayData.timeSinceFrameWasSwitched += replayTime.deltaTime;
        }
    }
    public void RecordCurrentFrameData(object sender, EventArgs e)
    {
        // is our recording at max capacity?
        if(recordingFrameDataQueue.Count >= replayData.recordingFrameCountMax){
            recordingFrameDataQueue.Dequeue(); // removes the oldest frame
        }
        recordingFrameDataQueue.Enqueue(GetCurrentFrameData());
    }
    public void StartRecording(object sender, EventArgs e)
    {
        recordingFrameDataQueue = new Queue<GameplaySingleFrameData>();
    }
    public void PlaybackUpdate(object sender, EventArgs e)
    {
        // FixedUpdate maps frame data, update lerps towards next frame
        if(replayData.timeSinceFrameWasSwitched >= Time.fixedDeltaTime){
            // switch to the next frame index
            playbackFrame++;
            // replay finished?
            if (playbackFrame < recordingFrameData.Length-1)
            {
                MapFrameDataToObjects(recordingFrameData[playbackFrame]);
                nextFrameData = recordingFrameData[playbackFrame+1]; // LateUpdate uses this
            }
            else 
            {
                StopInstantReplay();
                return;
            }
            if(playbackFrame == 1){puckTrail.Clear();}
            replayData.timeSinceFrameWasSwitched = 0;
        }
    }
    private void CancelReplay(object sender, EventArgs e)
    {
        StopInstantReplay();
    }
    public void StopInstantReplay()
    {
        gameSystem.UnFreeze();
        gameSystem.SetAllActionMapsToPlayer();
        gameSystem.SetPlayersToTeams();
        gameSystem.ApplyGameplayFrameData(beforeReplayState.frameData); // return to before replay state
        EnableAnimators();
        playingBack = false;
        playbackFrame = 0;
        instantReplayGUI.SetActive(false);
        replayCamera.gameObject.SetActive(false);
        replayCamera2.gameObject.SetActive(false);
        replayCamera3.gameObject.SetActive(false);
        puckTrail.time = 1.4f;
        replayEnd?.Invoke(this, EventArgs.Empty);
    }
    public void startInstantReplay(object sender, EventArgs e) // called by the game manager when someone scores or does something cool
    {
        playingBack = true;
        playbackFrame = 0;
        //copy the contents of the queue into an array
        recordingFrameData = new GameplaySingleFrameData[recordingFrameDataQueue.Count];
        recordingFrameDataQueue.CopyTo(recordingFrameData, 0);
        nextFrameData = recordingFrameData[1];
        beforeReplayState.frameData = GetCurrentFrameData();
        instantReplayGUI.SetActive(true);
        replayCamera.gameObject.SetActive(true);
        replayCamera2.gameObject.SetActive(true);
        replayCamera3.gameObject.SetActive(true);
        gameSystem.FreezeGame();
        gameSystem.SetAllActionMapsToReplay();
        gameSystem.SetAIActiveState(false);
        DisableAnimators();
        puckTrail.time = 4f;
    }
}