using System;
using UnityEngine;
public class FaceOffState : StateMachineBehaviour
{
    private GameSystem gameSystem;
    private AudioManager audioManager;
    private TimeProvider gameTimeProvider;
    public static EventHandler<EventArgs> onStateEnter;
    public static EventHandler<EventArgs> onStateExit;
    float countdownTimer = 4f;
    bool faceOffCountDown = false;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       Debug.Log("FaceOffState Enter");
       gameSystem = FindObjectOfType<GameSystem>();
       gameTimeProvider = gameSystem.timeManager.gameTime;
       countdownTimer = 4;
       faceOffCountDown = true;
       gameSystem.countdownDisplayPanel.SetActive(true);
       gameSystem.audioManager.PlayReadySound();
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        countdownTimer -= gameTimeProvider.fixedDeltaTime;// Animator update mode -> Physics, makes this function run during FixedUpdate
        gameSystem.countdownCountText.text = ((int)countdownTimer).ToString();
        if(countdownTimer <= 0 && faceOffCountDown){
            faceOffCountDown = false;
            countdownTimer = 4;
            gameSystem.countdownDisplayPanel.SetActive(false);
            gameSystem.audioManager.PlayFaceOffSound();
            gameSystem.ActivateGoals();
            gameSystem.DropPuck();
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
