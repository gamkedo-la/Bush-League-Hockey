using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManagerScript : MonoBehaviour
{
    [HideInInspector] public List<GameObject> localPlayerControllers;
    public void JoinNewPlayer(PlayerInput playerInput){
        PlayerController newPlayerInput = playerInput.gameObject.GetComponent<PlayerController>();
        Debug.Log($"new input:  {newPlayerInput}");
        if(localPlayerControllers.Count % 2 == 0){
            newPlayerInput.SetIsHomeTeam(false);
        } else{
            newPlayerInput.SetIsHomeTeam(true);
        }
        if(!localPlayerControllers.Contains(playerInput.gameObject)){localPlayerControllers.Add(playerInput.gameObject);}
        Debug.Log($"Controllers:  {localPlayerControllers}");
    }
}
