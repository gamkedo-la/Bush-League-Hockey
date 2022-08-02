using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodycheckHitZone : MonoBehaviour
{
    [HideInInspector] public Vector3 hitForce;
    private void Start(){hitForce = Vector3.zero;}
    private void OnTriggerEnter(Collider other)
    {
        Skater otherSkater = other.GetComponent<Skater>();
        if (otherSkater){otherSkater.ReceiveBodyCheck(hitForce);}
    }
}
