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
    List<RaycastResult> clickResults;
    public void Awake() {
        gameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        playerInput = GetComponent<PlayerInput>();
    }
    public void MovementInputHandler(InputAction.CallbackContext context){
        movementInput = context.ReadValue<Vector2>();
    }
    public void UnPause(InputAction.CallbackContext context){
        if(context.performed){
            gameSystem.HandleResume();
        }
    }
    public void ResetGame(InputAction.CallbackContext context){
        if(context.performed){
            gameSystem.BeginGame();
        }
    }
}
