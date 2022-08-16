using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
public class SkaterAnimationScript : MonoBehaviour
{
    [SerializeField] Skater thisSkater;
    [SerializeField] TeamMember thisTeamMember;
    [SerializeField] Rigidbody modelHips;
    [SerializeField] GameObject bodyCheckHitZone;
    [SerializeField] TwistChainConstraint twistChainConstraint;
    private Rigidbody[] ragdollRigidBodies;
    private Collider[] ragdollColliders;
    public Animator skaterAnimator;
    public RigBuilder rigBuilder;
    public Rig skateCycleRig;
    
    private void Awake(){
        skaterAnimator = GetComponent<Animator>();
        rigBuilder = GetComponent<RigBuilder>();
        ragdollRigidBodies = GetComponentsInChildren<Rigidbody>();
    }
    public void ResetAnimations(){
        skaterAnimator.SetBool("AnimateShotWindUp", false);
        skaterAnimator.SetBool("AnimatePassWindUp", false);
        skaterAnimator.SetBool("AnimateBodychecking", false);
        skaterAnimator.ResetTrigger("AnimateShotFollowThru");
        skaterAnimator.ResetTrigger("AnimatePassFollowThru");
        skaterAnimator.ResetTrigger("AnimateBodycheckFollowThru");
        // EnableRigBuilder();
        EnableRigAll();
    }
    public void ActivateBodycheck(){
        bodyCheckHitZone.SetActive(true);
    }
    public void DeactivateBodycheck(){
        bodyCheckHitZone.SetActive(false);
        ResetAnimations();
    }
    public void ResetRagdoll(){
        thisSkater.transform.position = new Vector3(
            modelHips.gameObject.transform.position.x,
            thisSkater.transform.position.y,
            modelHips.gameObject.transform.position.z
        );
        thisTeamMember.EnableInteractions();
        thisSkater.GetComponent<Collider>().enabled = true;
        thisSkater.isKnockedDown = false;
        skaterAnimator.enabled = true;
    }
    public IEnumerator RagdollThenReset(float hitPower, Vector3 hitDirection, float recoverTime){
        // DisableRigBuilder();
        skaterAnimator.enabled = false;
        foreach(Rigidbody rB in ragdollRigidBodies){
            rB.velocity = hitDirection*hitPower;
        }
        yield return new WaitForSeconds(recoverTime);
        ResetRagdoll();
    }
    public void DisableRigBuilder(){
        rigBuilder.enabled = false;
    }
    public void EnableRigBuilder(){
        rigBuilder.enabled = true;
    }
    public void DisableRigExceptHead(){
        twistChainConstraint.weight = 0;
        skateCycleRig.weight = 0;
    }
    public void EnableRigAll(){
        twistChainConstraint.weight = 1;
        skateCycleRig.weight = 1;
    }
}
