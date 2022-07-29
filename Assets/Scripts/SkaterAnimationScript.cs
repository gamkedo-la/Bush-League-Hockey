using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkaterAnimationScript : MonoBehaviour
{
    [SerializeField] Animator skaterAnimator;
    public void StopWindUpAnimation(){
        Debug.Log("StopWindUpAnimation");
        skaterAnimator.SetBool("AnimateShotWindUp", false);
    }
}
