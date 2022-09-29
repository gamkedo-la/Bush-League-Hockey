using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class InstantReplay : MonoBehaviour
{
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
    public Vector3 replayCameraOffset;
    public Transform replayTarget2;
    public Camera replayCamera2;
    public Vector3 replayCameraOffset2;
    public Transform replayTarget3;
    public Camera replayCamera3;
    public Vector3 replayCameraOffset3;
    private List<Rigidbody> gamePieceRigidbodies;
    public Transform p1;
    public Transform g1;
    public Transform p2;
    public Transform g2;
    public Transform puck;
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
        p2 = gameSystem.awaySkater.transform;
        g1 = gameSystem.homeGoaltender.transform;
        g2 = gameSystem.awayGoaltender.transform;
        puck = gameSystem.puckObject.transform;
        puckTrail = puck.GetComponent<TrailRenderer>();
    }
    void Start()
    {
        bones1 = bonesRig1.GetComponentsInChildren<Transform>();
        bones2 = bonesRig2.GetComponentsInChildren<Transform>();
        Debug.Log("Replay bones found: " + bones1.Length);
        for (int i=0; i<bones1.Length; i++) {
            Debug.Log("Bone "+i+" is named "+bones1[i].name);
        }
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
    private GameplaySingleFrameData GetCurrentFrameData(){
        currentFrameData = new GameplaySingleFrameData();
        currentFrameData.p1Position = p1.position; currentFrameData.p1Rotation = p1.rotation;
        currentFrameData.p2Position = p2.position; currentFrameData.p2Rotation = p2.rotation;
        currentFrameData.g1Position = g1.position; currentFrameData.g1Rotation = g1.rotation;
        currentFrameData.g2Position = g2.position; currentFrameData.g2Rotation = g2.rotation;
        currentFrameData.puckPosition = puck.position; currentFrameData.puckRotation = puck.rotation;
        // record all 30 or so bone pos+rot for each avatar
        for (int b=0; b<bones1.Length; b++) {
            currentFrameData.bones1pos[b] = bones1[b].position; 
            currentFrameData.bones1rot[b] = bones1[b].rotation;
        }
        for (int b=0; b<bones2.Length; b++) {
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
        float interpolationPercent = 1; // replayTime.deltaTime / Time.fixedDeltaTime;
        // Debug.Log($"interpolate: {interpolationPercent} = {replayTime.deltaTime}/{Time.fixedDeltaTime}");
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
    void LateUpdate()
    {
        // press [R] for replay anytime
        if (playingBack){
            // Fixed update will apply transforms from recorded data
            // LateUpdate will lerp between current frame and next
            // LerpObjectsTowardsNextFrame();
            MapFrameDataToObjects(recordingFrameData[playbackFrame]);
            replayData.timeSinceFrameWasSwitched += replayTime.deltaTime;
        }
    }
    void FixedUpdate()
    {
        // recording in FixedUpdate gives us a recording with a constant time interval between frames
        // this makes our data as true to the original gameplay as possible
        // We also get more replay time out of each frame we store
        if(!playingBack){
            // Not in playback mode, record replay data
            // is our recording at max capacity?
            if(recordingFrameDataQueue.Count >= replayData.recordingFrameCountMax){
                recordingFrameDataQueue.Dequeue(); // removes the oldest frame
            }
            recordingFrameDataQueue.Enqueue(GetCurrentFrameData());
        }
        if(playingBack)
        {
            // FixedUpdate maps frame data, update lerps towards next frame
            if(replayData.timeSinceFrameWasSwitched >= Time.fixedDeltaTime){
                // switch to the next frame index
                playbackFrame++;
                // replay finished?
                if (playbackFrame < recordingFrameData.Length-1)
                {
                    MapFrameDataToObjects(recordingFrameData[playbackFrame]);
                    nextFrameData = recordingFrameData[playbackFrame]; // LateUpdate uses this
                }
                else 
                {
                    StopInstantReplay();
                    return;
                }
                if(playbackFrame == 1){puckTrail.Clear();}
                replayCamera.transform.position = new Vector3(
                    puck.position.x + replayCameraOffset.x,
                    puck.position.y + replayCameraOffset.y,
                    puck.position.z + replayCameraOffset.z);
                replayCamera.transform.LookAt(puck);
                replayCamera2.transform.position = new Vector3(
                    replayTarget2.position.x + replayCameraOffset2.x,
                    replayTarget2.position.y + replayCameraOffset2.y,
                    replayTarget2.position.z + replayCameraOffset2.z);
                replayCamera2.transform.LookAt(replayTarget2);
                replayCamera3.transform.position = new Vector3(
                    replayTarget3.position.x + replayCameraOffset3.x,
                    replayTarget3.position.y + replayCameraOffset3.y,
                    replayTarget3.position.z + replayCameraOffset3.z);
                replayCamera3.transform.LookAt(replayTarget3);
                replayData.timeSinceFrameWasSwitched = 0;
            }
        }
    }
    public void StopInstantReplay()
    {
        gameSystem.UnFreeze();
        gameSystem.SetAllActionMapsToPlayer();
        gameSystem.SetAIActiveState(true);
        MapFrameDataToObjects(beforeReplayState.frame); // return to before replay state
        EnableAnimators();
        playingBack = false;
        playbackFrame = 0;
        instantReplayGUI.SetActive(false);
        replayCamera.gameObject.SetActive(false);
        replayCamera2.gameObject.SetActive(false);
        replayCamera3.gameObject.SetActive(false);
        puckTrail.time = 1.4f;
    }
    public IEnumerator startInstantReplay() // called by the game manager when someone scores or does something cool
    {
        playbackFrame = 0;
        //copy the contents of the queue into an array
        recordingFrameData = new GameplaySingleFrameData[recordingFrameDataQueue.Count];
        recordingFrameDataQueue.CopyTo(recordingFrameData, 0);
        nextFrameData = recordingFrameData[1];
        beforeReplayState.frame = GetCurrentFrameData();
        instantReplayGUI.SetActive(true);
        replayCamera.gameObject.SetActive(true);
        replayCamera2.gameObject.SetActive(true);
        replayCamera3.gameObject.SetActive(true);
        playingBack = true;
        gameSystem.FreezeGame();
        gameSystem.SetAllActionMapsToReplay();
        gameSystem.SetAIActiveState(false);
        DisableAnimators();
        puckTrail.time = 4f;
        while(playingBack) {
            yield return new WaitForFixedUpdate();
        }
    }
}