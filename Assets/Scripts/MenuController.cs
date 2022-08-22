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
    private void IncrementChooseSideChoice(){
        for (int i = 0; i < teamSelectionChoices.Length; i++){
            if (teamSelectionChoices[i] == teamSelectionStatus){
                teamSelectionStatus = i < teamSelectionChoices.Length-1 ? teamSelectionChoices[i+1] : teamSelectionChoices[teamSelectionChoices.Length-1];
            }
        }
        Debug.Log($"current: {teamSelectionStatus}");
    }
    private void DecrementChooseSideChoice(){
        for (int i = 0; i < teamSelectionChoices.Length; i++){
            if (teamSelectionChoices[i] == teamSelectionStatus) {
                teamSelectionStatus = i > 0 ? teamSelectionChoices[i - 1] : teamSelectionChoices[i];
            }
        }
    }
    public void InitializeController(){
        teamSelectionStatus = teamSelectionChoices[1];
    }
    public void SwitchOffChoosingSides(){
        isChoosingSides = false;
        foreach (MenuController ctrl in FindObjectsOfType<MenuController>()){
            ctrl.isChoosingSides = false;
        }
    }
    public void SwitchOnChoosingSides(){
        isChoosingSides = true;
        foreach (MenuController ctrl in FindObjectsOfType<MenuController>()){
            ctrl.isChoosingSides = true;
        }
    }
    public void ChooseSidesNavigation(InputAction.CallbackContext context){
        if(context.performed){
            movementInput = context.ReadValue<Vector2>();
            if(isChoosingSides){
                if(movementInput.x > 0){
                    IncrementChooseSideChoice();
                } else if(movementInput.x < 0){
                    DecrementChooseSideChoice();
                }
                FindObjectOfType<GameStartScript>()?.HandleChooseSidePosition();
            }
        }
    }
    public void MovementInputHandler(InputAction.CallbackContext context){
        movementInput = context.ReadValue<Vector2>();
    }
    public void UnPause(InputAction.CallbackContext context){
        if(context.performed){
            gameSystem.HandleResume();
        }
    }
}
