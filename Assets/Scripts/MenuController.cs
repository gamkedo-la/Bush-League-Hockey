using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour
{
    // [Header("Player Input Management")]
    private GameSystem gameSystem;
    private Vector2 movementInput;
    private PlayerInput playerInput;
    private PointerEventData clickData;
    [Header("Choose Sides")]
    public GameObject chooseSidesMenuIcon;
    private string[] teamSelectionChoices = {"home", "neutral", "away"};
    [HideInInspector] public string teamSelectionStatus = "neutral";
    public bool isChoosingSides = false;
    public void Awake() {
        gameSystem = GameObject.Find("GameSystem")?.GetComponent<GameSystem>();
        playerInput = gameObject.GetComponent<PlayerInput>();
    }
    public void SwitchActionMapToChooseSides(){
        Awake();
        playerInput.SwitchCurrentActionMap("ChooseSide");
    }
    private void IncrementChooseSideChoice() {
        for (int i = 0; i < teamSelectionChoices.Length; i++) {
            if (teamSelectionChoices[i] == teamSelectionStatus) {
                teamSelectionStatus = i < teamSelectionChoices.Length-1 ? teamSelectionChoices[i + 1] : teamSelectionChoices[i];
            }
        }
    }
    private void DecrementChooseSideChoice() {
        for (int i = 0; i < teamSelectionChoices.Length; i++) {
            if (teamSelectionChoices[i] == teamSelectionStatus) {
                teamSelectionStatus = i > 0 ? teamSelectionChoices[i - 1] : teamSelectionChoices[i];
            }
        }
    }
    public void InitializeController(){
        teamSelectionStatus = teamSelectionChoices[1];
    }
    public void ChooseSidesNavigation(InputAction.CallbackContext context){
        if(context.performed){
            movementInput = context.ReadValue<Vector2>();
            if(movementInput.x > 0){
                IncrementChooseSideChoice();
            } else if(movementInput.x < 0){
                DecrementChooseSideChoice();
            }
            FindObjectOfType<GameStartScript>()?.HandleChooseSidePosition();
        }
    }
    public void MovementInputHandler(InputAction.CallbackContext context){
        Debug.Log($"movementInput: {movementInput}");
        movementInput = context.ReadValue<Vector2>();
        // if(context.performed && isChoosingSides){
        //     Debug.Log($"choosing side logic");
        //     movementInput = context.ReadValue<Vector2>();
        //     if(movementInput.x > 0){
        //         IncrementChooseSideChoice();
        //     } else if(movementInput.x < 0){
        //         DecrementChooseSideChoice();
        //     }
        //     FindObjectOfType<GameStartScript>().HandleChooseSidePosition();
        // }
        // gamestartscript HandleChooseSidePosition
    }
    public void UnPause(InputAction.CallbackContext context){
        if(context.performed){
            gameSystem.HandleResume();
        }
    }
}
