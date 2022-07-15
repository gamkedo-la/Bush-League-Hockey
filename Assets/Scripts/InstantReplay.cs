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

    public int RecordingLength = 300;

    private Vector3[] p1pos;
    private Vector3[] p2pos;
    private Vector3[] g1pos;
    private Vector3[] g2pos;
    private Vector3[] puckpos;

    private int ticks = 0;

    // Start is called before the first frame update
    void Start()
    {
         p1pos = new Vector3[RecordingLength];
         p2pos = new Vector3[RecordingLength];
         g1pos = new Vector3[RecordingLength];
         g2pos = new Vector3[RecordingLength];
         puckpos = new Vector3[RecordingLength];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // called same # of times per second on all computers
    void FixedUpdate()
    {
        ticks++;

        // the % makes us wrap around to the start of the array once it's full
        int nextOne = ticks % RecordingLength;

        // fixme: copy to existing vector3s for no GC
        if (p1) p1pos[nextOne] = new Vector3(p1.position.x,p1.position.y,p1.position.z);
        if (p2) p2pos[nextOne] = new Vector3(p2.position.x,p1.position.y,p1.position.z);
        if (g1) g1pos[nextOne] = new Vector3(g1.position.x,p1.position.y,p1.position.z);
        if (g2) g2pos[nextOne] = new Vector3(g2.position.x,p1.position.y,p1.position.z);
        if (puck) puckpos[nextOne] = new Vector3(puck.position.x,p1.position.y,p1.position.z);

        if (ticks % 200 == 0) {
            Debug.Log("Instant Replay has recorded "+(ticks*5)+" data points.");
        }

    }
}
