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
        Debug.Log($"{movementInput} Menu Control");
    }
    public void MenuSelectInputHandler(InputAction.CallbackContext context){
        if(context.performed){
            Debug.Log($"MenuSelect");
        }
    }
    public void UnPause(InputAction.CallbackContext context){
        if(context.performed){
            gameSystem.HandleResume();
            // Cursor.lockState = CursorLockMode.None;
        }
    }
    public void ResetGame(InputAction.CallbackContext context){
        if(context.performed){
            gameSystem.BeginGame();
        }
    }
    public void UIClick(InputAction.CallbackContext context){
        if(context.performed){
            clickData.position = Mouse.current.position.ReadValue();
            clickResults.Clear();
            foreach (RaycastResult result in clickResults){
                Debug.Log($"Menu Click: {result.gameObject.name}");
            }
        }
    }
}
