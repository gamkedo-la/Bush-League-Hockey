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
    private Vector3 appliedForwardForce;
    private Vector3 appliedSideForce;
    private PlayerInput playerInput;
    private Vector2 movementInput;
    private Vector3 movementPointer;
    [Header("Main Camera")]
    [SerializeField] Camera mainCamera;
    [Header("Team Management")]
    [SerializeField] Skater selectedSkater;
    [SerializeField] Goaltender goaltender;
    public bool isHomeTeam;
    private void Awake(){
        gameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        playerInput = GetComponent<PlayerInput>();
    }
    public void MovementInputHandler(InputAction.CallbackContext context){
        movementInput = context.ReadValue<Vector2>();
        selectedSkater?.SetShotDirection(movementInput);
        appliedForwardForce = movementInput.y * Vector3.Normalize(transform.position - gameSystem.mainCamera.transform.position);
        appliedSideForce = movementInput.x * Vector3.Cross(gameSystem.mainCamera.transform.forward, -gameSystem.mainCamera.transform.up);
        movementPointer = Vector3.Normalize(new Vector3((appliedForwardForce.x + appliedSideForce.x), 0f, (appliedForwardForce.z + appliedSideForce.z)));
        selectedSkater?.MoveSkater(movementPointer);
        goaltender?.MoveGoalie(movementPointer);
    }
}
