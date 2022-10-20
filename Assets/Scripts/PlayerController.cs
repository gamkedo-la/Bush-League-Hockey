using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    private GameSystem gameSystem;
    private Camera mainCamera;
    [Header("Player Input Management")]
    private Vector3 forwardForce;
    private Vector3 sideForce;
    private PlayerInput playerInput;
    private Vector2 movementInput;
    private Vector3 cameraRelativeMovementPointer;
    private bool canSpeedBoost = true;
    [Header("Team Management")]
    private Skater selectedSkater;
    private TeamMember selectedTeamMember;
    private Goaltender goaltender;
    private TeamMember goaltenderTeamMember;
    private void Awake(){
        gameSystem = FindObjectOfType<GameSystem>();
        mainCamera = Camera.main;
        playerInput = GetComponent<PlayerInput>();
    }
    public void SetToHomeTeam(){
        selectedSkater = GameObject.FindWithTag("homeSkater").GetComponent<Skater>();
        goaltender = GameObject.FindWithTag("homeGoaltender").GetComponent<Goaltender>();
        InitializeTeamObjects();
    }
    public void SetToAwayTeam(){
        selectedSkater = GameObject.FindWithTag("awaySkater").GetComponent<Skater>();
        goaltender = GameObject.FindWithTag("awayGoaltender").GetComponent<Goaltender>();
        InitializeTeamObjects();
    }
    public void SetToNeutralTeam(){
        selectedSkater = null;
        selectedTeamMember = null;
        goaltender = null;
    }
    private void InitializeTeamObjects(){
        Awake();
        selectedTeamMember = selectedSkater.gameObject.GetComponent<TeamMember>();
        goaltender.FindMyNet();
    }
    public void Pause(InputAction.CallbackContext context){
        if(context.performed){
            FindObjectOfType<InGameMenu>()?.HandlePause();
            GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
        }
    }
    public void StartInstantReplay(InputAction.CallbackContext context){
        if(context.performed){
            FindObjectOfType<InstantReplay>()?.startInstantReplay();
        }
    }
    public void CancelInstantReplay(InputAction.CallbackContext context){
        if(context.performed){
            FindObjectOfType<InstantReplay>()?.StopInstantReplay();
        }
    }
    public void TrashTalk(InputAction.CallbackContext context){
        if(context.performed){
            FindObjectOfType<AudioManager>()?.PlayTrashTalk();
        }
    }
    public void MovementInputHandler(InputAction.CallbackContext context){
        if(selectedSkater && goaltender){
            movementInput = context.ReadValue<Vector2>();
            forwardForce = movementInput.y * mainCamera.transform.forward;
            sideForce = movementInput.x * Vector3.Cross(mainCamera.transform.forward, -mainCamera.transform.up);
            cameraRelativeMovementPointer = Vector3.Normalize(new Vector3((forwardForce.x + sideForce.x), 0f, (forwardForce.z + sideForce.z)));
            selectedSkater.SetPointers(cameraRelativeMovementPointer);
            goaltender.SetPointers(cameraRelativeMovementPointer);
        }
    }
    public void StickControlInputHandler(InputAction.CallbackContext context){
        if(selectedSkater && goaltender){
            movementInput = context.ReadValue<Vector2>();
            forwardForce = movementInput.y * mainCamera.transform.forward;
            sideForce = movementInput.x * Vector3.Cross(mainCamera.transform.forward, -mainCamera.transform.up);
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
    private IEnumerator SpeedBoostAndCooldown(){
        canSpeedBoost = false;
        StartCoroutine(goaltender.Slide());
        yield return new WaitForSeconds(3f);
        canSpeedBoost = true;
    }
    public void SpeedBoost(InputAction.CallbackContext context){
        if(context.performed && canSpeedBoost){
            StartCoroutine(SpeedBoostAndCooldown());
        }
    }
}
