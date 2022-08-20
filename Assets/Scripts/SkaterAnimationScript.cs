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
    [SerializeField] public GameObject instantReplayController;

    private Rigidbody[] ragdollRigidBodies;
    private Collider[] ragdollColliders;
    public Animator skaterAnimator;
    public RigBuilder rigBuilder;
    public Rig skateCycleRig;
    
    private void Awake(){
        skaterAnimator = GetComponent<Animator>();
        rigBuilder = GetComponent<RigBuilder>();
        ragdollRigidBodies = GetComponentsInChildren<Rigidbody>();
        DisableModelCollisionSfx();
    }
    public void ResetAnimations(){
        skaterAnimator.SetBool("AnimateShotWindUp", false);
        skaterAnimator.SetBool("AnimatePassWindUp", false);
        skaterAnimator.SetBool("AnimateBodychecking", false);
        skaterAnimator.ResetTrigger("AnimateShotFollowThru");
        skaterAnimator.ResetTrigger("AnimatePassFollowThru");
        skaterAnimator.ResetTrigger("AnimateBodycheckFollowThru");
        StartCoroutine(EnableRigConstraints());
    }
    public void ActivateBodycheck(){
        bodyCheckHitZone.SetActive(true);
    }
    public void DeactivateBodycheck(){
        bodyCheckHitZone.SetActive(false);
    }
    private void EnableModelCollisionSfx(){
        foreach (Rigidbody rigidbody in ragdollRigidBodies){
            rigidbody.gameObject.GetComponent<ModelCollisionSfx>().collisionSfxEnabled = true;
        }
    }
    private void DisableModelCollisionSfx(){
        foreach (Rigidbody rigidbody in ragdollRigidBodies){
            rigidbody.gameObject.GetComponent<ModelCollisionSfx>().collisionSfxEnabled = false;
        }
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
        DisableModelCollisionSfx();
    }
    public IEnumerator RagdollThenReset(float hitPower, Vector3 hitDirection, float recoverTime){
        skaterAnimator.enabled = false;
        EnableModelCollisionSfx();
        foreach(Rigidbody rB in ragdollRigidBodies){
            rB.velocity = hitDirection*hitPower;
        }
        yield return new WaitForSeconds(recoverTime);
        ResetRagdoll();
        ResetAnimations();

        Debug.Log("Resetting ragdoll state: triggering an instant replay!");
        instantReplayController?.GetComponent<InstantReplay>()?.startInstantReplay();

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
    public IEnumerator EnableRigConstraints(){
        yield return new WaitForEndOfFrame();
        twistChainConstraint.weight = 1f;
        skateCycleRig.weight = 1f;
    }
}
