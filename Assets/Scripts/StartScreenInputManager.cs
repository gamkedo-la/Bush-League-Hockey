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
        Debug.Log($"input type: {playerInput.currentControlScheme}");
        switch (playerInput.currentControlScheme)
        {
            case "Keyboard&Mouse":
            Debug.Log($"case Keyboard and Mouse");
                playerInput.GetComponent<MenuController>().chooseSidesMenuIcon = keyboardIcon;
                break;
            case "PS4":
                playerInput.GetComponent<MenuController>().chooseSidesMenuIcon = ps4ControllerIcon;
                break;
            case "XBox":
                playerInput.GetComponent<MenuController>().chooseSidesMenuIcon = xboxControllerIcon;
                break;
            case "Gamepad":
                playerInput.GetComponent<MenuController>().chooseSidesMenuIcon = genericControllerIcon;
                break;
        }
        playerInput.GetComponent<MenuController>().InitializeController();
        playerInput.GetComponent<MultiplayerEventSystem>().firstSelectedGameObject = GetComponent<GameStartScript>().playButton;
    }
}
