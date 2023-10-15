using UnityEngine;
public class PuckHandleJoint : MonoBehaviour
{
    private FixedJoint puckHandleJoint;
    public void AttachPuckToHandleJoint(Rigidbody puckRigidBody){
        puckHandleJoint = gameObject.AddComponent(typeof(FixedJoint)) as FixedJoint;
        puckHandleJoint.connectedBody = puckRigidBody;
        puckHandleJoint.breakForce = 150000f;
    }
    public void BreakFixedJoint(){
        Destroy(puckHandleJoint);
    }
}
