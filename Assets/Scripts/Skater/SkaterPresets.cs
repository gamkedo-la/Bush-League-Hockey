using UnityEngine;

[CreateAssetMenu(fileName = "SkaterPresets", menuName = "ScriptableObjects/SkaterPresets", order = 1)]
public class SkaterPresets : ScriptableObject {
    [Header("Skating")]
    public float skaterAcceleration;
    public float skaterMaximumSpeed;
    public float skaterTurnSpeed;
    public float angleAccelerationLimit;
    public float angleTurnLimit;
    [Header("Shooting")]
    [Range(0.5f, 6f)] public float shotPowerWindUpRate; // extraPower / second
    [Range(8f, 20f)] public float shotPowerMax;
    [Range(4f, 16f)] public float shotPower;
    [Header("Passing")]
    [Range(0.5f, 6f)] public float passPowerWindUpRate; // extraPassPower / second
    [Range(6f, 12f)] public float passPowerMax;
    [Range(1f, 8f)] public float passPower;
    [Header("Colliding/Checking")]
    public AnimationCurve checkPowerCurve;
    [Range(1f, 6f)] public float checkPower;
    [Range(4f, 64f)] public float checkPowerMax;
    [Range(1f, 12f)] public float checkPowerWindUpRate;
    [Range(0.2f, 3f)] public float bodycheckCooldownTime;
    [Range(0.2f, 3f)] public float bodyCheckRecoverTime;
}
