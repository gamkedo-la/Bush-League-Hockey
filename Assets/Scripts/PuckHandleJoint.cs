using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class PuckHandleJoint : MonoBehaviour
{
    private FixedJoint puckHandleJoint;
    public void AttachPuckToHandleJoint(Rigidbody puckRigidBody){
        puckHandleJoint = gameObject.AddComponent(typeof(FixedJoint)) as FixedJoint;
        puckHandleJoint.connectedBody = puckRigidBody;
        puckHandleJoint.breakForce = 2500f;
    }
    public void BreakFixedJoint(){
        Destroy(puckHandleJoint);
    }
}
