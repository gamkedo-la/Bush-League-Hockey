using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayerController : MonoBehaviour
{
    private Vector3 movementPointer;
    private Skater selectedSkater;
    private TeamMember selectedTeamMember;
    private Goaltender goaltender;
    private bool isDoingSomething = false;
    // If one team doesn't have a player, this object is assigned to that team.
    // Whereas the 'PlayerController' prefab uses player input to trigger functions resulting in behaviour
    // This object will use it's own logic to emulate those same input values.
    // Look at the 'PlayerController' script to see how the pieces in the game are controlled.
    // 'PlayerController' calls 'Skater', 'Goaltender', 'TeamMember' scripts to trigger behavior.
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
    private void InitializeTeamObjects(){
        selectedTeamMember = selectedSkater.gameObject.GetComponent<TeamMember>();
        goaltender.FindMyNet();
    }
    private void HandleMovement(){ // based on 'Skater' -> MovementInputHandler()
        // move in the given direction
        // selectedSkater.SetPointers(movementPointer);
    }
    private IEnumerator ChaseDownPuck(){
        isDoingSomething = true;
        Debug.Log($"AI pursuing puck");
        while(!selectedTeamMember.hasPosession){
            // where is the puck?
            // draw a vector from me to the puck
            // zero the y axis (only moving in x, z)
            // normalize the vector
            // Set movementPointer to the normalized vector
            yield return new WaitForSeconds(3f); // commit to doing this for a some time
            Debug.Log($"AI keep chasing?");
        }
        movementPointer = Vector3.zero;
        isDoingSomething = false;
    }
    void Start()
    {
        
    }
    void Update()
    {
        // read the situation, decide what to do
        // call coroutines StartCoroutine(DoSomething())
        // am I already doing something?
        if(!isDoingSomething){
            // do I have posession?
            StartCoroutine(ChaseDownPuck());
            // or try to score
        }
        // Is there something more important I should do?
        // cancel the current action and respond to it
    }
}
