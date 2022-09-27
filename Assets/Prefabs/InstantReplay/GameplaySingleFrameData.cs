using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplaySingleFrameData : MonoBehaviour
{
    // This class represents the information required to recreate one frame of gameplay
    public Vector3 p1Position;
    public Vector3 p2Position;
    public Quaternion p1Rotation;
    public Quaternion p2Rotation;
    public Vector3 p1Velocity = new Vector3(0, 0, 0);
    public Vector3 p2Velocity = new Vector3(0, 0, 0);
    public Vector3 g1Position;
    public Vector3 g2Position;
    public Quaternion g1Rotation;
    public Quaternion g2Rotation;
    public Vector3 puckPosition;
    public Quaternion puckRotation;
    public Vector3 puckVelocity = new Vector3(0, 0, 0);
    public Vector3[] bones1pos = new Vector3[29];
    public Quaternion[] bones1rot = new Quaternion[29];
    public Vector3[] bones2pos = new Vector3[29];
    public Quaternion[] bones2rot = new Quaternion[29];
}
