using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class InstantReplay : MonoBehaviour
{

    public Transform p1;
    public Transform g1;
    public Transform p2;
    public Transform g2;
    public Transform puck;

    public bool playingBack = false;
    public float playbackSpeed = 0.5f;
    public int RecordingLength = 300;

    // these should probably be private
    public float recordingTimespan = 0;
    public float playbackTime = 0f;
    public int playbackFrame = 0;

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

    private int ticks = 0;

    // Start is called before the first frame update
    void Start()
    {
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
        }

        recordingTimespan = RecordingLength*Time.fixedDeltaTime; /// calculate max length of replay

    }

    // Update is called once per frame
    void Update()
    {
        // we need to find the puck AFTER init since it is not there at first
        if (!puck) puck = GameObject.FindWithTag("puck").transform;
        
        if (playingBack) {
            
            Time.timeScale = 0; // PAUSE THE GAME!

            // FIXME: we need to pause the game simulation so playback doesn't argue with it
            // TODO: lerp from one recorded frame to another for smooth slow-mo not slideshow
            if (p1) { p1.position = p1pos[playbackFrame]; p1.rotation = p1rot[playbackFrame]; }
            if (p2) { p2.position = p2pos[playbackFrame]; p2.rotation = p2rot[playbackFrame]; }
            if (g1) { g1.position = g1pos[playbackFrame]; g1.rotation = g1rot[playbackFrame]; }
            if (g2) { g2.position = g2pos[playbackFrame]; g2.rotation = g2rot[playbackFrame]; }
            if (puck) { puck.position = puckpos[playbackFrame]; puck.rotation = puckrot[playbackFrame]; }

            // maybe advance to next frame in the playback
            playbackTime += Time.unscaledDeltaTime * playbackSpeed; // must be unscaled dt since when paused it is 0
            playbackFrame = (int)(Mathf.Floor(RecordingLength * (playbackTime/recordingTimespan)));

        }

        if (playbackFrame >= RecordingLength) { // done
            playingBack = false;
            playbackTime = 0f;
            playbackFrame = 0;
        }

        if (!playingBack) { // hmm spammy?
            Time.timeScale = 1f;
            playbackFrame = 0;
            playbackTime = 0f;
        }
        
    }

    // called same # of times per second on all computers
    void FixedUpdate()
    {
        ticks++;

        // the % makes us wrap around to the start of the array once it's full
        int nextOne = ticks % RecordingLength;

        // copy to existing vector3s so we don't fill up ram, avoiding any GC
        // instead of this: = new Vector3(p1.position.x,p1.position.y,p1.position.z);
        if (p1) { p1pos[nextOne] = p1.position; p1rot[nextOne] = p1.rotation; }
        if (p2) { p2pos[nextOne] = p2.position; p2rot[nextOne] = p2.rotation; }
        if (g1) { g1pos[nextOne] = g1.position; g1rot[nextOne] = g1.rotation; }
        if (g2) { g2pos[nextOne] = g2.position; g2rot[nextOne] = g2.rotation; }
        if (puck) { puckpos[nextOne] = puck.position; puckrot[nextOne] = puck.rotation; }

    }
}
