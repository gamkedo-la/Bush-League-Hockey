using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplaySingleFrameData : MonoBehaviour
{
    // This class represents the information required to recreate one frame of gameplay
    Vector3 p1Position;
    Vector3 p2Position;
    Quaternion p1Rotation;
    Quaternion p2Rotation;
    Vector3 p1Velocity = new Vector3(0, 0, 0);
    Vector3 p2Velocity = new Vector3(0, 0, 0);
    Vector3 g1Position;
    Vector3 g2Position;
    Quaternion g1Rotation;
    Quaternion g2Rotation;
    Vector3 puckPosition;
    Quaternion puckRotation;
    Vector3 puckVelocity = new Vector3(0, 0, 0);
    Transform bonesRig1;
    Vector3[,] bones1pos;
    Quaternion[,] bones1rot;
    Transform bonesRig2;
    Vector3[,] bones2pos;
    Quaternion[,] bones2rot;
    Transform[] bones1;
    Transform[] bones2;
}
