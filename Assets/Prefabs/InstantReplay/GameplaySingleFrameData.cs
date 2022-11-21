using UnityEngine;
// change to struct
public struct GameplaySingleFrameData
{
    public GameplaySingleFrameData(
        Vector3 p1Pos,
        Vector3 p2Pos,
        Quaternion p1Rot,
        Quaternion p2Rot,
        Vector3 g1Pos,
        Vector3 g2Pos,
        Quaternion g1Rot,
        Quaternion g2Rot,
        Vector3 puckPos,
        Quaternion puckRot
    )
    {
        p1Position = p1Pos;
        p2Position = p2Pos;
        p1Rotation = p1Rot;
        p2Rotation = p2Rot;
        p1Velocity = new Vector3(0, 0, 0);
        p2Velocity = new Vector3(0, 0, 0);
        g1Position = g1Pos;
        g2Position = g2Pos;
        g1Rotation = g1Rot;
        g2Rotation = g2Rot;
        puckPosition = puckPos;
        puckRotation = puckRot;
        puckVelocity = new Vector3(0, 0, 0);
        bones1pos = new Vector3[29];
        bones1rot = new Quaternion[29];
        bones2pos = new Vector3[29];
        bones2rot = new Quaternion[29];
    }
    public Vector3 p1Position {get; set;}
    public Vector3 p2Position {get; set;}
    public Quaternion p1Rotation {get; set;}
    public Quaternion p2Rotation {get; set;}
    public Vector3 p1Velocity {get; set;}
    public Vector3 p2Velocity {get; set;}
    public Vector3 g1Position {get; set;}
    public Vector3 g2Position {get; set;}
    public Quaternion g1Rotation {get; set;}
    public Quaternion g2Rotation {get; set;}
    public Vector3 puckPosition {get; set;}
    public Quaternion puckRotation {get; set;}
    public Vector3 puckVelocity {get; set;}
    public Vector3[] bones1pos {get; set;}
    public Quaternion[] bones1rot {get; set;}
    public Vector3[] bones2pos {get; set;}
    public Quaternion[] bones2rot {get; set;}
}
