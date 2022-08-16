using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class InstantReplay : MonoBehaviour
{

    public Camera replayCamera;
    public Vector3 replayCameraOffset;
    public GameObject instantReplayGUI; // an icon 
    
    public Transform p1;
    public Transform g1;
    public Transform p2;
    public Transform g2;
    public Transform puck;
    public Transform bonesRig1;
    private Vector3[,] bones1pos;
    private Quaternion[,] bones1rot;
    public Transform bonesRig2;
    private Vector3[,] bones2pos;
    private Quaternion[,] bones2rot;
    private Transform[] bones1;
    private Transform[] bones2;
    public bool playingBack = false;
    public float playbackSpeed = 0.5f;
    public int RecordingLength = 300;

    private Vector3[] p1pos;
    private Vector3[] p2pos;
    private Vector3[] g1pos;
    private Vector3[] g2pos;
    private Vector3[] puckpos;

    private Quaternion[] p1rot;
    private Quaternion[] p2rot;
    private Quaternion[] g1rot;
    private Quaternion[] g2rot;
    private Quaternion[] puckrot;

    // these should probably be private
    public float recordingTimespan = 0;
    public float playbackTime = 0f;
    public int playbackFrame = 0;
    public int ticks = 0;
    public int newestIndex = 0;
    public int playbackStartFrame = -999;
    public int playbackEndFrame = -999;
    private bool animatorSwitch = true;

    // Start is called before the first frame update
    void Start()
    {

        if (instantReplayGUI) instantReplayGUI.SetActive(false);
        animatorSwitch = true;
        
        // create an empty array
        p1pos = new Vector3[RecordingLength];
        p2pos = new Vector3[RecordingLength];
        g1pos = new Vector3[RecordingLength];
        g2pos = new Vector3[RecordingLength];
        puckpos = new Vector3[RecordingLength];
        p1rot = new Quaternion[RecordingLength];
        p2rot = new Quaternion[RecordingLength];
        g1rot = new Quaternion[RecordingLength];
        g2rot = new Quaternion[RecordingLength];
        puckrot = new Quaternion[RecordingLength];

        bones1 = bonesRig1.GetComponentsInChildren<Transform>();
        bones2 = bonesRig2.GetComponentsInChildren<Transform>();
        //Debug.Log("Replay bones found: " + bones1.Length);
        for (int i=0; i<bones1.Length; i++) {
            //Debug.Log("Bone "+i+" is named "+bones2[i].name);
        }

        bones1pos = new Vector3[32,RecordingLength];
        bones1rot = new Quaternion[32,RecordingLength];
        bones2pos = new Vector3[32,RecordingLength];
        bones2rot = new Quaternion[32,RecordingLength];

        // fill with 300 vec3s that we reuse over and over
        for (int nextOne=0; nextOne<RecordingLength; nextOne++) {

            p1pos[nextOne] = new Vector3();
            p2pos[nextOne] = new Vector3();
            g1pos[nextOne] = new Vector3();
            g2pos[nextOne] = new Vector3();
            puckpos[nextOne] = new Vector3();
            p1rot[nextOne] = new Quaternion();
            p2rot[nextOne] = new Quaternion();
            g1rot[nextOne] = new Quaternion();
            g2rot[nextOne] = new Quaternion();
            puckrot[nextOne] = new Quaternion();

            for (int b=0; b<32; b++) {
                bones1pos[b,nextOne] = new Vector3();
                bones1rot[b,nextOne] = new Quaternion();
                bones2pos[b,nextOne] = new Vector3();
                bones2rot[b,nextOne] = new Quaternion();
            }

        }

        recordingTimespan = RecordingLength*Time.fixedDeltaTime; /// calculate max length of replay

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
    // Update is called once per frame
    void Update()
    {
        // we need to find the puck AFTER init since it is not there at first
        if (!puck) puck = GameObject.FindWithTag("puck")?.transform; // NOTE: can vanish mid game!!!!!!!
        
        //Debug.Log("stick bone pos: "+bones2[8].position);

        if (playingBack) {
            
            //Debug.Log("REPLAY!!!");

            Time.timeScale = 0; // PAUSE THE GAME!
            
            // FIXME: we need to pause the game simulation so playback doesn't argue with it
            // TODO: lerp from one recorded frame to another for smooth slow-mo not slideshow
            // ie = Vector3.Lerp(p1rot[(playbackFrame-1)%RecordingLength],p1rot[playbackFrame],lerpPercent);
            if (p1) { p1.position = p1pos[playbackFrame]; p1.rotation = p1rot[playbackFrame]; }
            if (p2) { p2.position = p2pos[playbackFrame]; p2.rotation = p2rot[playbackFrame]; }
            if (g1) { g1.position = g1pos[playbackFrame]; g1.rotation = g1rot[playbackFrame]; }
            if (g2) { g2.position = g2pos[playbackFrame]; g2.rotation = g2rot[playbackFrame]; }
            if (puck) { puck.position = puckpos[playbackFrame]; puck.rotation = puckrot[playbackFrame]; }
            if (replayCamera!=null && puck!=null) {
                //Debug.Log("replay cam is following the puck!");
                replayCamera.enabled = true;
                // follow the puck
                replayCamera.transform.position = new Vector3(
                    puck.position.x + replayCameraOffset.x,
                    puck.position.y + replayCameraOffset.y,
                    puck.position.z + replayCameraOffset.z);
            } else {
                //Debug.Log("replay cam is messed up!"); // this should never run
            }
            if(animatorSwitch){
                // This code executes once at the beginning of the replay
                TurnOffAnimators();
                puck.gameObject.GetComponent<TrailRenderer>().Clear();
                animatorSwitch = false;
            }
            // playback all 30 or so bones for each avatar
            for (int b=0; b<bones1.Length; b++) {
                bones1[b].position = bones1pos[b,playbackFrame];
                bones1[b].rotation = bones1rot[b,playbackFrame]; 
            }
            for (int b=0; b<bones2.Length; b++) {
                bones2[b].position = bones2pos[b,playbackFrame]; 
                bones2[b].rotation = bones2rot[b,playbackFrame]; 
            }

            // crazy hack to test whether changing bone pos affects the mesh as intended
            // bones2[8].position = new Vector3(10,10,10); // insta mega move!!!!

            // notice when we first started playback so we know when to finish
            if (playbackStartFrame==-999) {
                playbackEndFrame = newestIndex;
                playbackStartFrame = newestIndex+1;
                playbackStartFrame = playbackStartFrame % RecordingLength; // wrap around
                //Debug.Log("Playback starting on recorded frame "+playbackStartFrame);
                if (instantReplayGUI) instantReplayGUI.SetActive(true);

                // spammy debug of recorded data just to ensure we have real values
                //Debug.Log("==== ALL RECORDED DATA FOR PLR1 BONE 8 (stick) ====");
                // for (int i=0; i<RecordingLength; i++) {
                //     Debug.Log(bones2pos[8,i]); 
                // }
                
            }
            
            // maybe advance to next frame in the playback
            playbackTime += Time.unscaledDeltaTime * playbackSpeed; // must be unscaled dt since when paused it is 0
            playbackFrame = playbackStartFrame + (int)(Mathf.Floor(RecordingLength * (playbackTime/recordingTimespan)));
            playbackFrame = playbackFrame % RecordingLength; //wrap around

            // finished a full replay?
            if (playbackFrame == playbackEndFrame) {
                //Debug.Log("Playback completed on recorded frame "+playbackFrame);
                playingBack = false;
                animatorSwitch = true;
                TurnOnAnimators();
                playbackTime = 0f;
                playbackFrame = 0;
                playbackStartFrame = -999;
                playbackEndFrame = -999;
                if (instantReplayGUI) instantReplayGUI.SetActive(false);
            }

        }

        if (!playingBack) {
            // make sure the game is no longer paused
            // fixme: wasteful to call this every frame
            // but it lets us toggle the checkbox during debugging
            Time.timeScale = 1f;
            playbackFrame = 0;
            playbackTime = 0f;
            playbackStartFrame = -999;
            if (replayCamera) replayCamera.enabled = false;
        }
        
    }

    // called same # of times per second on all computers
    void FixedUpdate()
    {
        ticks++;

        // the % makes us wrap around to the start of the array once it's full
        newestIndex = ticks % RecordingLength;

        // copy to existing vector3s so we don't fill up ram, avoiding any GC
        // instead of this: = new Vector3(p1.position.x,p1.position.y,p1.position.z);
        if (p1) { p1pos[newestIndex] = p1.position; p1rot[newestIndex] = p1.rotation; }
        if (p2) { p2pos[newestIndex] = p2.position; p2rot[newestIndex] = p2.rotation; }
        if (g1) { g1pos[newestIndex] = g1.position; g1rot[newestIndex] = g1.rotation; }
        if (g2) { g2pos[newestIndex] = g2.position; g2rot[newestIndex] = g2.rotation; }
        if (puck) { puckpos[newestIndex] = puck.position; puckrot[newestIndex] = puck.rotation; }

        // record all 30 or so bone pos+rot for each avatar
        for (int b=0; b<bones1.Length; b++) {
            bones1pos[b,newestIndex] = bones1[b].position; 
            bones1rot[b,newestIndex] = bones1[b].rotation;
        }
        for (int b=0; b<bones2.Length; b++) {
            bones2pos[b,newestIndex] = bones2[b].position; 
            bones2rot[b,newestIndex] = bones2[b].rotation;
        }


    }

    public void startInstantReplay() // called by the game manager when someone scores or does something cool
    {
        playingBack = true;
    }
}
