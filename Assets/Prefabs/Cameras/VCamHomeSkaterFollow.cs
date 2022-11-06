using System;
using UnityEngine;
using Cinemachine;

public class VCamHomeSkaterFollow : MonoBehaviour
{
    private GameSystem gameSystem;
    private CinemachineVirtualCamera vCamHomeSkaterFollow;
    private void Awake() {
        gameSystem = FindObjectOfType<GameSystem>();
        vCamHomeSkaterFollow = GetComponent<CinemachineVirtualCamera>();
    }
    void Start()
    {
        // Find the puck and follow it
        vCamHomeSkaterFollow.Follow = gameSystem.homeSkater.transform;
        vCamHomeSkaterFollow.LookAt = gameSystem.homeSkater.transform;
    }
}
