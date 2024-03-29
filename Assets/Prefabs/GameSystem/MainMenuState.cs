using System;
using UnityEngine;
public class MainMenuState : StateMachineBehaviour
{
    private GameSystem gameSystem;
    private MainMenuScript mainMenuScript;
    public static EventHandler<EventArgs> onStateEnter;
    public static EventHandler<EventArgs> onStateExit;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Main menu start");
        gameSystem = FindObjectOfType<GameSystem>();
        mainMenuScript = gameSystem.mainMenu.GetComponent<MainMenuScript>();
        Goal.awayGoalTrigger += GoalScored;
        Goal.homeGoalTrigger += GoalScored;
        onStateEnter.Invoke(this, EventArgs.Empty);
        // Setup the AI to play casually
        gameSystem.mainMenu.SetActive(true); // in game menu manager will handle this
        mainMenuScript.SwitchToMainMenu();
        gameSystem.SetPlayersToTeams();
        gameSystem.SetupPlayersForFaceOff();
        gameSystem.PuckToCenterOrigin();
    }
    public void GoalScored(object sender, EventArgs e)
    {
        gameSystem.CasualGoalScored();
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("Main menu update");
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Main menu exit");
        Goal.awayGoalTrigger -= GoalScored;
        Goal.homeGoalTrigger -= GoalScored;
        gameSystem.mainMenu.SetActive(false);
        onStateExit?.Invoke(this, EventArgs.Empty);
    }
}
