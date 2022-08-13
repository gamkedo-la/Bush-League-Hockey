using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LookTargetScript : MonoBehaviour
{
    [SerializeField] GameObject lookTarget;
    [SerializeField] GameObject defaultFocusPoint;
    [SerializeField] float lookTargetSpeed;
    private GameObject focusObject;
    // Sphere collider detects close objects and overrides look target
    // Set priorities based on context
    // Shooting? Look at net
    // loose puck? Look at puck
    // Opponent is checking? Look at opponent
    // Move looktarget towards object
    // Rythym:  Cycle where by monkey changes look target every few seconds
    private void Awake(){
        focusObject = defaultFocusPoint;
    }
    private void MoveLookTarget(){
        if(focusObject == null)return;
        lookTarget.transform.position = Vector3.MoveTowards(
            lookTarget.transform.position, 
            focusObject.transform.position, 
            lookTargetSpeed * Time.deltaTime
        );
    }
    private bool ObjectIsLookTarget(GameObject objectToCheck){
        return (
            objectToCheck.tag == "puck"
            || objectToCheck.tag.Contains("Net")
            || objectToCheck.tag.Contains("Skater")
            || objectToCheck.tag.Contains("Goaltender")
        );
    }
    private void OnTriggerEnter(Collider other){
        if(ObjectIsLookTarget(other.gameObject)){
            focusObject = other.gameObject;
        }
    }
    private void Update(){
        MoveLookTarget();
    }
}
