using System;
using UnityEngine;
using Cinemachine;

public class VCamAwaySkaterFollow : MonoBehaviour
{
    private GameSystem gameSystem;
    private CinemachineVirtualCamera vCamAwaySkaterFollow;
    private void Awake() {
        gameSystem = FindObjectOfType<GameSystem>();
        vCamAwaySkaterFollow = GetComponent<CinemachineVirtualCamera>();
    }
    void Start()
    {
        // Find the puck and follow it
        vCamAwaySkaterFollow.Follow = gameSystem.awaySkater.transform;
        vCamAwaySkaterFollow.LookAt = gameSystem.awaySkater.transform;
    }
}
