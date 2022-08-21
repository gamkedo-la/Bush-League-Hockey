using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class InputManagerScript : MonoBehaviour
{
    [SerializeField] public GameObject ps4ControllerIcon;
    [SerializeField] public GameObject keyboardIcon;
    [SerializeField] public GameObject genericControllerIcon;
    [SerializeField] public GameObject xboxControllerIcon;
    [HideInInspector] public List<GameObject> localPlayerControllers;
    public void JoinNewPlayer(PlayerInput playerInput){
        Debug.Log($"new input:  {playerInput}");
        playerInput.GetComponent<MenuController>().chooseSidesMenuIcon = ps4ControllerIcon;
        playerInput.GetComponent<MenuController>().InitializeController();
        if(!localPlayerControllers.Contains(playerInput.gameObject)){localPlayerControllers.Add(playerInput.gameObject);}
        foreach (GameObject ctrl in localPlayerControllers){
            Debug.Log($"Controllers:  {ctrl}");
        }
    }
}
