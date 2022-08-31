using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;
public class StartScreenInputManager : MonoBehaviour
{
    [SerializeField] public GameObject ps4ControllerIcon;
    [SerializeField] public GameObject keyboardIcon;
    [SerializeField] public GameObject genericControllerIcon;
    [SerializeField] public GameObject xboxControllerIcon;
    public void JoinNewPlayer(PlayerInput playerInput){
        Debug.Log($"input type: {playerInput.devices[0]}");
        // figure out what kind of controller it is
        // apply the correct icon
        playerInput.GetComponent<MenuController>().chooseSidesMenuIcon = ps4ControllerIcon;
        playerInput.GetComponent<MenuController>().InitializeController();
        // set default menu selection
        playerInput.GetComponent<MultiplayerEventSystem>().firstSelectedGameObject = GetComponent<GameStartScript>().playButton;
        foreach (MenuController ctrl in FindObjectsOfType<MenuController>()){
            //Debug.Log($"Controllers:  {ctrl.gameObject}");
        }
    }
    // set players = MultiPlayerEventSystem.count > 0 ? 
    // FindObjectOfType<PlayerInput>
    // if(localPlayerControllers.count > 0 localPlayerController[0].gameObject.GetComponent<MultiplayerEventSystem>().firstSelectedGameObject = GetComponent<GameStartScript>().playButton;}
}
