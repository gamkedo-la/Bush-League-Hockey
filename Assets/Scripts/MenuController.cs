using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{
    // [Header("Player Input Management")]
    private GameSystem gameSystem;
    private Vector2 movementInput;
    private PlayerInput playerInput;
    private void Awake() {
        gameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        playerInput = GetComponent<PlayerInput>();
    }
    public void MovementInputHandler(InputAction.CallbackContext context){
        movementInput = context.ReadValue<Vector2>();
        Debug.Log($"{movementInput} Menu Control");
    }
    public void ResetGame(InputAction.CallbackContext context){
        Debug.Log($"{movementInput} Menu Control");
    }
}
