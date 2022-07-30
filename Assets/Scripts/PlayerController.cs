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
    private Vector3 forwardForce;
    private Vector3 sideForce;
    private PlayerInput playerInput;
    private Vector2 movementInput;
    private Vector3 movementPointer;
    [Header("Team Management")]
    private Skater selectedSkater;
    private TeamMember selectedTeamMember;
    private Goaltender goaltender;
    private TeamMember goaltenderTeamMember;
    public bool isHomeTeam;
    public void SetIsHomeTeam(bool homeTeamBool){
        isHomeTeam = homeTeamBool;
        if(isHomeTeam){
            selectedSkater = GameObject.FindWithTag("homeSkater").GetComponent<Skater>();
            selectedTeamMember = GameObject.FindWithTag("homeSkater").GetComponent<TeamMember>();
            goaltender = GameObject.FindWithTag("homeGoaltender").GetComponent<Goaltender>();
            goaltenderTeamMember = GameObject.FindWithTag("homeGoaltender").GetComponent<TeamMember>();
        } else{
            selectedSkater = GameObject.FindWithTag("awaySkater").GetComponent<Skater>();
            selectedTeamMember = GameObject.FindWithTag("awaySkater").GetComponent<TeamMember>();
            goaltender = GameObject.FindWithTag("awayGoaltender").GetComponent<Goaltender>();
            goaltenderTeamMember = GameObject.FindWithTag("awayGoaltender").GetComponent<TeamMember>();
        }
        goaltenderTeamMember.SetIsHomeTeam(isHomeTeam);
        goaltender.FindMyNet();
    }
    private void Awake(){
        gameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        playerInput = GetComponent<PlayerInput>();
    }
    public void MovementInputHandler(InputAction.CallbackContext context){
        if(selectedSkater && goaltender){
            movementInput = context.ReadValue<Vector2>();
            selectedSkater.SetShotDirection(movementInput);
            selectedTeamMember.SetPassDirection(movementInput);
            goaltender.SetShotDirection(movementInput);
            goaltenderTeamMember.SetPassDirection(movementInput);
            forwardForce = movementInput.y * Vector3.Normalize(transform.position - gameSystem.mainCamera.transform.position);
            sideForce = movementInput.x * Vector3.Cross(gameSystem.mainCamera.transform.forward, -gameSystem.mainCamera.transform.up);
            movementPointer = Vector3.Normalize(new Vector3((forwardForce.x + sideForce.x), 0f, (forwardForce.z + sideForce.z)));
            selectedSkater.MoveSkater(movementPointer);
            goaltender.MoveGoalie(movementPointer);
        }
    }
    public void ShootButtonInputHandler(InputAction.CallbackContext context){
        if (!selectedSkater) return; // bugfix: selectedSkater can be null here
        if(context.performed){
            selectedSkater.ShootPuck();
            goaltender.ShootPuck();
        }
        if(context.started && !selectedTeamMember.windingUp){
            selectedTeamMember.windingUp = true;
            goaltenderTeamMember.windingUp = true;
            StartCoroutine(selectedSkater.WindUpShot());
            StartCoroutine(goaltender.WindUpShot());
        }
    }
    public void PassButtonInputHandler(InputAction.CallbackContext context){
        if (!selectedSkater) return; // bugfix: selectedSkater can be null here
        if(context.performed){
            selectedTeamMember.PassPuck();
            goaltenderTeamMember.PassPuck();
        }
        if(context.started && !selectedTeamMember.windingUp){
            selectedTeamMember.windingUp = true;
            goaltenderTeamMember.windingUp = true;
            StartCoroutine(selectedTeamMember.WindUpPass());
            StartCoroutine(goaltenderTeamMember.WindUpPass());
        }
    }
    public void BodyCheckInputHandler(InputAction.CallbackContext context)
    {
        if (!selectedSkater) return;
        if (selectedTeamMember.HasPuck()) return;
        if (context.performed) {
            selectedSkater.BodyCheck();
        }
        else if (context.started && !selectedTeamMember.windingUp){
            selectedTeamMember.windingUp = true;
            StartCoroutine(selectedSkater.WindUpBodyCheck());
        }
    }
}
