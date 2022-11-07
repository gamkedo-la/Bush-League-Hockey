using System;
using UnityEngine;
public class FaceOffState : StateMachineBehaviour
{
    private GameSystem gameSystem;
    private AudioManager audioManager;
    private TimeProvider gameTimeProvider;
    public static EventHandler<EventArgs> onStateEnter;
    public static EventHandler<FaceOffEventArgs> onStateUpdate;
    public static EventHandler<EventArgs> onStateExit;
    public float countdownTimer = 4f;
    bool faceOffCountDown = false;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       Debug.Log("FaceOffState Enter");
       gameSystem = FindObjectOfType<GameSystem>();
       gameTimeProvider = gameSystem.timeManager.gameTime;
       countdownTimer = 4;
       faceOffCountDown = true;
       onStateEnter?.Invoke(this, EventArgs.Empty);
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        countdownTimer -= gameTimeProvider.fixedDeltaTime;
        // gameSystem.puckObject.transform.position = gameSystem.puckDropOrigin.position;
        // gameSystem.puckObject.transform.rotation = gameSystem.puckDropOrigin.rotation;
        if(countdownTimer <= 0 && faceOffCountDown){
            faceOffCountDown = false;
            countdownTimer = 4;
            gameSystem.SetupPlayersForFaceOff();
            gameSystem.PuckToCenterOrigin();
            gameSystem.masterStateMachine.SetBool("GameOn", true);
        }
        onStateUpdate?.Invoke(this, new FaceOffEventArgs(countdownTimer));
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        onStateExit?.Invoke(this, EventArgs.Empty);
    }
}
public class FaceOffEventArgs : EventArgs
{
    public float countdownTimer = 4f;
    public FaceOffEventArgs(float s){
        countdownTimer = s;
    }
}
