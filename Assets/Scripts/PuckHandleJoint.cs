using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class PuckHandleJoint : MonoBehaviour
{
    private FixedJoint puckHandleJoint;
    public void AttachPuckToHandleJoint(){
        puckHandleJoint = ObjectFactory.AddComponent<FixedJoint>(gameObject);
        puckHandleJoint.breakForce = 1000;
    }
}
