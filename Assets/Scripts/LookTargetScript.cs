using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookTargetScript : MonoBehaviour
{
    [SerializeField] GameObject lookTarget;
    private GameObject focusObject;
    // Sphere collider detects close objects and overrides look target
    // Set priorities based on context
    // Shooting? Look at net
    // loose puck? Look at puck
    // Opponent is checking? Look at opponent
    // Move looktarget towards object
    // Rythym:  Cycle where by monkey changes look target every few seconds
    private void MoveLookTargetTo(Transform target){
        Debug.Log($"Switching look target to {target.name}");
    }
    
}
