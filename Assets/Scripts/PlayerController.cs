using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Player controls team
    // goaltender, skater
    private GameSystem gameSystem;
    [Header("Player Input Management")]
    private PlayerInput playerInput;
    [SerializeField] Skater selectedSkater;
    private Vector2 movementInput;
    private Vector3 movementVector;
    [Header("Main Camera")]
    [SerializeField] Camera mainCamera;
    [Header("Team Management")]
    public bool isHomeTeam;
    private void Awake(){
        gameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        playerInput = GetComponent<PlayerInput>();
    }
    public void OnMove(InputAction.CallbackContext context){
        selectedSkater.MoveInput(context.ReadValue<Vector2>());
        Debug.Log("Move Input Received: " + context.ReadValue<Vector2>().ToString());
    }
}
