using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
public class StartScreenInputManager : MonoBehaviour
{
    [SerializeField] public GameObject ps4ControllerIcon;
    [SerializeField] public GameObject keyboardIcon;
    [SerializeField] public GameObject genericControllerIcon;
    [SerializeField] public GameObject xboxControllerIcon;
    public void JoinNewPlayer(PlayerInput playerInput){
        switch (playerInput.currentControlScheme){
            case "Keyboard&Mouse":
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
        playerInput.GetComponent<MultiplayerEventSystem>().firstSelectedGameObject = GetComponent<MainMenuScript>().currentItem;
    }
}
