using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class SkaterAnimationScript : MonoBehaviour
{
    [SerializeField] Rigidbody bodycheckDeliveryObject;
    public Animator skaterAnimator;
    public RigBuilder rigBuilder;
    private void Awake(){
        skaterAnimator = GetComponent<Animator>();
        rigBuilder = GetComponent<RigBuilder>();
    }
    public void StopWindUpAnimation(){
        skaterAnimator.SetBool("AnimateShotWindUp", false);
        skaterAnimator.SetBool("AnimatePassWindUp", false);
        // EnableRigBuilder();
    }
    public IEnumerator RagdollThenReset(float recoverTime, Vector3 hitForce){
        skaterAnimator.enabled = false;
        bodycheckDeliveryObject.AddForce(hitForce, ForceMode.VelocityChange);
        yield return new WaitForSeconds(recoverTime);
        skaterAnimator.enabled = true;
    }
    public void DisableRigBuilder(){
        rigBuilder.enabled = false;
    }
    public void EnableRigBuilder(){
        rigBuilder.enabled = true;
    }
}
