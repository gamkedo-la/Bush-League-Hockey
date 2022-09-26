using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class InstantReplay : MonoBehaviour
{
    [SerializeField] GameObject instantReplayGUI;
    [SerializeField] TimeProvider replayTime;
    [SerializeField] ReplayData replayData;
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
    public Transform p1;
    public Transform g1;
    public Transform p2;
    public Transform g2;
    public Transform puck;
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
    }
    void Start()
    {
        bones1 = bonesRig1.GetComponentsInChildren<Transform>();
        bones2 = bonesRig2.GetComponentsInChildren<Transform>();
        //Debug.Log("Replay bones found: " + bones1.Length);
        // for (int i=0; i<bones1.Length; i++) {
        //     Debug.Log("Bone "+i+" is named "+bones2[i].name);
        // }
    }
    private void TurnOffAnimators(){
        p1.gameObject.GetComponentInChildren<Animator>().enabled = false;
        p2.gameObject.GetComponentInChildren<Animator>().enabled = false;
        //g1.gameObject.GetComponentInChildren<Animator>().enabled = false;
        //g2.gameObject.GetComponentInChildren<Animator>().enabled = false;
    }
    private void TurnOnAnimators(){
        p1.gameObject.GetComponentInChildren<Animator>().enabled = true;
        p2.gameObject.GetComponentInChildren<Animator>().enabled = true;
        //g1.gameObject.GetComponentInChildren<Animator>().enabled = true;
        //g2.gameObject.GetComponentInChildren<Animator>().enabled = true;
    }
    void Update()
    {
        // press [R] for replay anytime
        if (playingBack){
            // lerp towards the next frame transforms
            // current frame, next frame
            // fixed deltatime, deltatime, lerpt % = deltatime / fixed deltatime
            float interpolationPercent = Time.deltaTime / Time.fixedDeltaTime;
            Debug.Log($"interpolate: {interpolationPercent} = {Time.deltaTime}/{Time.fixedDeltaTime}");
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
        }
    }
    void FixedUpdate()
    {
        // recording in FixedUpdate gives us a recording with a constant time interval between frames
        // this means playback with timescale will be as smooth as possible
        // We also get more replay time out of each frame we store; fixedDeltaTime is longer than deltaTIme
        if(!playingBack){
            // Not in playback mode, record replay data
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
            // is our recording at max capacity?
            if(recordingFrameDataQueue.Count >= replayData.recordingFrameCountMax){
                // remove the oldest frame(s)
                // is it possible to store more than 1 frame over limit?
                for (int i = 0; i <= recordingFrameDataQueue.Count - replayData.recordingFrameCountMax; i++)
                {
                    recordingFrameDataQueue.Dequeue();
                }
            }
            recordingFrameDataQueue.Enqueue(currentFrameData);
        }

        if(playingBack)
        {
            // playback mode
            // increment the current frame based on replay timescale
            // Update() will Lerp() towards next transform set
            //if(replayData.timeSinceFrameWasSwitched >= Time.fixedDeltaTime){
                // switch to the next frame index
                // reset the frame timer
                //replayData.timeSinceFrameWasSwitched = 0f;
                playbackFrame++;
                 // replay finished?
                if (playbackFrame < recordingFrameData.Length -1)
                {
                    nextFrameData = recordingFrameData[playbackFrame+1];
                } else 
                {
                    StopInstantReplay();
                    return;
                }
                Debug.Log($"i: {playbackFrame}/{recordingFrameData.Length-1}");
                p1.position = recordingFrameData[playbackFrame].p1Position; p1.rotation = recordingFrameData[playbackFrame].p1Rotation;
                p2.position = recordingFrameData[playbackFrame].p2Position; p2.rotation = recordingFrameData[playbackFrame].p2Rotation;
                g1.position = recordingFrameData[playbackFrame].g1Position; g1.rotation = recordingFrameData[playbackFrame].g1Rotation;
                g2.position = recordingFrameData[playbackFrame].g2Position; g2.rotation = recordingFrameData[playbackFrame].g2Rotation;
                puck.position = recordingFrameData[playbackFrame].puckPosition; puck.rotation = recordingFrameData[playbackFrame].puckRotation;
                
                if (replayCamera!=null && puck!=null) {
                    replayCamera.enabled = true;
                    replayCamera.transform.position = new Vector3(
                        puck.position.x + replayCameraOffset.x,
                        puck.position.y + replayCameraOffset.y,
                        puck.position.z + replayCameraOffset.z);
                    replayCamera.transform.LookAt(puck);
                } 
                if (replayCamera2!=null && replayTarget2!=null) {
                    replayCamera2.enabled = true;
                    replayCamera2.transform.position = new Vector3(
                        replayTarget2.position.x + replayCameraOffset2.x,
                        replayTarget2.position.y + replayCameraOffset2.y,
                        replayTarget2.position.z + replayCameraOffset2.z);
                    replayCamera2.transform.LookAt(replayTarget2);
                } 
                if (replayCamera3!=null && replayTarget3!=null) {
                    replayCamera3.enabled = true;
                    replayCamera3.transform.position = new Vector3(
                        replayTarget3.position.x + replayCameraOffset3.x,
                        replayTarget3.position.y + replayCameraOffset3.y,
                        replayTarget3.position.z + replayCameraOffset3.z);
                    replayCamera3.transform.LookAt(replayTarget3);
                }
                // playback all 30 or so bones for each avatar
                for (int b=0; b<bones1.Length; b++) {
                    bones1[b].position = recordingFrameData[playbackFrame].bones1pos[b];
                    bones1[b].rotation = recordingFrameData[playbackFrame].bones1rot[b]; 
                }
                for (int b=0; b<bones2.Length; b++) {
                    bones2[b].position = recordingFrameData[playbackFrame].bones2pos[b]; 
                    bones2[b].rotation = recordingFrameData[playbackFrame].bones2rot[b];
                }
            //}
            //replayData.timeSinceFrameWasSwitched += Time.fixedDeltaTime;
        }
    }
    public void StopInstantReplay(){
        gameSystem.UnFreeze();
        TurnOnAnimators();
        playingBack = false;
        playbackFrame = 0;
        instantReplayGUI.SetActive(false);
        replayCamera.enabled = false;
        replayCamera2.enabled = false;
        replayCamera3.enabled = false;
        puck.gameObject.GetComponent<TrailRenderer>().time = 1.4f;
        // return the players to the beforeReplayGameState
    }
    public IEnumerator startInstantReplay() // called by the game manager when someone scores or does something cool
    {
        playbackFrame = 0;
        gameSystem = FindObjectOfType<GameSystem>();
        puck = gameSystem.puckObject.transform;
        //copy the contents of the queue into an array
        recordingFrameData = new GameplaySingleFrameData[recordingFrameDataQueue.Count];
        recordingFrameDataQueue.CopyTo(recordingFrameData, 0);
        instantReplayGUI.SetActive(true);
        replayCamera.enabled = true;
        replayCamera2.enabled = true;
        replayCamera3.enabled = true;
        playingBack = true;
        gameSystem.FreezeGame();
        TurnOffAnimators();
        puck.gameObject.GetComponent<TrailRenderer>().Clear();
        puck.gameObject.GetComponent<TrailRenderer>().time = 5f;
        while(playingBack) {
            yield return new WaitForFixedUpdate();
        }
    }
}
