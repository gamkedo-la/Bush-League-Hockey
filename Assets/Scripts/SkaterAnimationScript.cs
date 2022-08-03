using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class SkaterAnimationScript : MonoBehaviour
{
    [SerializeField] Skater thisSkater;
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
        skaterAnimator.SetBool("AnimateBodycheckWindUp", false);
        // EnableRigBuilder();
    }
    public IEnumerator RagdollThenReset(float recoverTime, Vector3 hitForce){
        skaterAnimator.enabled = false;
        thisSkater.isKnockedDown = true;
        bodycheckDeliveryObject.AddForce(hitForce, ForceMode.VelocityChange);
        yield return new WaitForSeconds(recoverTime);
        // set position of skater base to deliveryobject
        skaterAnimator.enabled = true;
        thisSkater.isKnockedDown = false;
    }
    public void DisableRigBuilder(){
        rigBuilder.enabled = false;
    }
    public void EnableRigBuilder(){
        rigBuilder.enabled = true;
    }
}
