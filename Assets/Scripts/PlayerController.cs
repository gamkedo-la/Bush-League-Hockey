using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    private GameSystem gameSystem;
    [Header("Player Input Management")]
    private Vector3 forwardForce;
    private Vector3 sideForce;
    private PlayerInput playerInput;
    private Vector2 movementInput;
    private Vector3 cameraRelativeMovementPointer;
    [Header("Team Management")]
    private Skater selectedSkater;
    private TeamMember selectedTeamMember;
    private Goaltender goaltender;
    private TeamMember goaltenderTeamMember;
    public bool isHomeTeam;
    public void SetIsHomeTeam(bool homeTeamBool){
        Awake();
        isHomeTeam = homeTeamBool;
        if(isHomeTeam){
            selectedSkater = GameObject.FindWithTag("homeSkater").GetComponent<Skater>();
            selectedTeamMember = GameObject.FindWithTag("homeSkater").GetComponent<TeamMember>();
            goaltender = GameObject.FindWithTag("homeGoaltender").GetComponent<Goaltender>();
            goaltenderTeamMember = GameObject.FindWithTag("homeGoaltender").GetComponent<TeamMember>();
        } else {
            selectedSkater = GameObject.FindWithTag("awaySkater").GetComponent<Skater>();
            selectedTeamMember = GameObject.FindWithTag("awaySkater").GetComponent<TeamMember>();
            goaltender = GameObject.FindWithTag("awayGoaltender").GetComponent<Goaltender>();
            goaltenderTeamMember = GameObject.FindWithTag("awayGoaltender").GetComponent<TeamMember>();
        }
        goaltenderTeamMember.SetIsHomeTeam(isHomeTeam);
        goaltender.FindMyNet();
    }
    private void Awake(){
        gameSystem = GameObject.Find("GameSystem")?.GetComponent<GameSystem>();
        playerInput = GetComponent<PlayerInput>();
    }
    public void Pause(InputAction.CallbackContext context){
        if(context.performed){
            gameSystem.HandlePause();
            playerInput.SwitchCurrentActionMap("UI");
        }
    }
    public void MovementInputHandler(InputAction.CallbackContext context){
        if(selectedSkater && goaltender){
            movementInput = context.ReadValue<Vector2>();
            forwardForce = movementInput.y * gameSystem.mainCamera.transform.forward;
            sideForce = movementInput.x * Vector3.Cross(gameSystem.mainCamera.transform.forward, -gameSystem.mainCamera.transform.up);
            cameraRelativeMovementPointer = Vector3.Normalize(new Vector3((forwardForce.x + sideForce.x), 0f, (forwardForce.z + sideForce.z)));
            selectedSkater.SetPointers(cameraRelativeMovementPointer);
            goaltender.SetPointers(cameraRelativeMovementPointer);
        }
    }
    public void StickControlInputHandler(InputAction.CallbackContext context){
        if(selectedSkater && goaltender){
            movementInput = context.ReadValue<Vector2>();
            forwardForce = movementInput.y * gameSystem.mainCamera.transform.forward;
            sideForce = movementInput.x * Vector3.Cross(gameSystem.mainCamera.transform.forward, -gameSystem.mainCamera.transform.up);
            cameraRelativeMovementPointer = Vector3.Normalize(new Vector3((forwardForce.x + sideForce.x), 0f, (forwardForce.z + sideForce.z)));
            selectedSkater.SetStickControlPointer(cameraRelativeMovementPointer);
        }
    }
    public void ShootButtonInputHandler(InputAction.CallbackContext context){
        if (!selectedSkater) return;
        if(context.performed){
            selectedSkater.ShootPuck();
            goaltender.ShootPuck();
        }
        if(context.started){
            StartCoroutine(selectedSkater.WindUpShot());
            StartCoroutine(goaltender.WindUpShot());
        }
    }
    public void PassButtonInputHandler(InputAction.CallbackContext context){
        if (!selectedSkater) return; // bugfix: selectedSkater can be null here
        if(context.performed){
            selectedSkater.PassPuck();
            goaltender.PassPuck();
        }
        if(context.started){
            StartCoroutine(selectedSkater.WindUpPass());
            StartCoroutine(goaltender.WindUpPass());
        }
    }
    public void BodyCheckInputHandler(InputAction.CallbackContext context){
        if(!selectedSkater) return;
        if(context.performed){
            selectedSkater.DeliverBodyCheck();
        } else if(context.started){
            StartCoroutine(selectedSkater.WindUpBodyCheck());
        }
    }
}
