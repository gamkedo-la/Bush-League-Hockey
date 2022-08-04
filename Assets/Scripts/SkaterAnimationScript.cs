using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
public class SkaterAnimationScript : MonoBehaviour
{
    [SerializeField] Skater thisSkater;
    [SerializeField] TeamMember thisTeamMember;
    [SerializeField] Rigidbody bodycheckDeliveryObject;
    [SerializeField] GameObject bodyCheckHitZone;
    public Animator skaterAnimator;
    public RigBuilder rigBuilder;
    private void Awake(){
        skaterAnimator = GetComponent<Animator>();
        rigBuilder = GetComponent<RigBuilder>();
    }
    public void ResetAnimations(){
        skaterAnimator.SetBool("AnimateShotWindUp", false);
        skaterAnimator.SetBool("AnimatePassWindUp", false);
        skaterAnimator.SetBool("AnimateBodycheckWindUp", false);
        skaterAnimator.ResetTrigger("AnimateShotFollowThru");
        skaterAnimator.ResetTrigger("AnimatePassFollowThru");
        skaterAnimator.ResetTrigger("AnimateBodycheckFollowThru");
        // EnableRigBuilder();
    }
    public void ActivateBodycheck(){
        bodyCheckHitZone.SetActive(true);
    }
    public void DeactivateBodycheck(){
        bodyCheckHitZone.SetActive(false);
        thisTeamMember.windingUp = false;
        ResetAnimations();
    }
    public IEnumerator RagdollThenReset(float recoverTime, Vector3 hitForce){
        // DisableRigBuilder();
        skaterAnimator.enabled = false;
        bodycheckDeliveryObject.AddForce(hitForce, ForceMode.VelocityChange);
        yield return new WaitForSeconds(recoverTime);
        thisSkater.transform.position = new Vector3(
            bodycheckDeliveryObject.gameObject.transform.position.x, 
            thisSkater.transform.position.y, 
            bodycheckDeliveryObject.gameObject.transform.position.z
        );
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
