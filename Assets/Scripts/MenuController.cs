using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour
{
    // [Header("Player Input Management")]
    private GameSystem gameSystem;
    private Vector2 movementInput;
    private PlayerInput playerInput;
    [Header("Choose Sides")]
    [HideInInspector] public GameObject chooseSidesMenuIcon;
    private string[] teamSelectionChoices = {"home", "neutral", "away"};
    public string teamSelectionStatus;
    private bool movementIsOnCooldown = false;
    private void IncrementChooseSideChoice(){
        for (int i = 0; i < teamSelectionChoices.Length; i++){
            if (teamSelectionChoices[i] == teamSelectionStatus){
                teamSelectionStatus = i < teamSelectionChoices.Length-1 ? teamSelectionChoices[i+1] : teamSelectionChoices[i];
            }
        }
    }
    private void DecrementChooseSideChoice(){
        for (int i = 0; i < teamSelectionChoices.Length; i++){
            if (teamSelectionChoices[i] == teamSelectionStatus){
                teamSelectionStatus = i > 0 ? teamSelectionChoices[i - 1] : teamSelectionChoices[i];
            }
        }
    }
    private bool StatusIsSet(){
        return teamSelectionStatus == "home" || teamSelectionStatus == "away" || teamSelectionStatus == "neutral";
    }
    public void InitializeController(){
        if(!StatusIsSet()){teamSelectionStatus = teamSelectionChoices[1];}
        playerInput = GetComponent<PlayerInput>();
        playerInput.GetComponent<InputSystemUIInputModule>().actionsAsset = playerInput.actions;
        playerInput.uiInputModule = GetComponent<InputSystemUIInputModule>();
    }
    private IEnumerator ChangeTeamStatusAndCooldown(Vector2 input){
        movementIsOnCooldown = true;
        if(input.x > 0){
            IncrementChooseSideChoice();
        } else if (input.x < 0){
            DecrementChooseSideChoice();
        }
        yield return new WaitForSeconds(0.25f);
        movementIsOnCooldown = false;
    }
    public void MovementInputHandler(InputAction.CallbackContext context){
        if(context.performed){
            movementInput = context.ReadValue<Vector2>();
            if(FindObjectOfType<ChooseSidesMenuScript>()?.enabled == true && !movementIsOnCooldown){
                StartCoroutine(ChangeTeamStatusAndCooldown(movementInput));
            }
        }
    }
    public void UnPause(InputAction.CallbackContext context){
        if(context.performed){
            FindObjectOfType<InGameMenu>()?.HandleResume();
        }
    }
}
