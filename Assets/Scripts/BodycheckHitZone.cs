using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodycheckHitZone : MonoBehaviour
{
    [SerializeField] Transform thisSkaterTransform;
    [HideInInspector] public float hitPower;
    private void OnTriggerEnter(Collider other){
        Skater otherSkater = other.GetComponent<Skater>();
        if(otherSkater && !otherSkater.isKnockedDown){ otherSkater?.ReceiveBodyCheck(hitPower, thisSkaterTransform.forward); }
    }
}
