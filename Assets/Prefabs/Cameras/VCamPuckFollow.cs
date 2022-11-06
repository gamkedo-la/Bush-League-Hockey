using System;
using UnityEngine;
using Cinemachine;

public class VCamPuckFollow : MonoBehaviour
{
    private GameSystem gameSystem;
    private CinemachineVirtualCamera vCamPuckFollow;
    private void Awake() {
        gameSystem = FindObjectOfType<GameSystem>();
        vCamPuckFollow = GetComponent<CinemachineVirtualCamera>();
    }
    void Start()
    {
        // Find the puck and follow it
        vCamPuckFollow.Follow = gameSystem.puckObject.transform;
        vCamPuckFollow.LookAt = gameSystem.puckObject.transform;
    }
}
