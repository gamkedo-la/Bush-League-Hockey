using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChooseSidesMenuScript : MonoBehaviour
{
    [SerializeField] List<Transform> homeSlots;
    [SerializeField] List<Transform> awaySlots;
    [SerializeField] List<Transform> neutralSlots;
    [SerializeField] Button acceptbutton;
    private bool acceptButtonIsEnabled = false;
    //add in icons for different controllers
    public void PositionInputControllerIcons(){
        foreach(GameObject icon in GameObject.FindGameObjectsWithTag("ControllerMenuIcon")){
            Destroy(icon);
        }
        int numberOfHomePlayers = 0;
        int numberOfAwayPlayers = 0;
        int numberOfNeutralPlayers = 0;
        foreach (MenuController menuController in FindObjectsOfType<MenuController>()){
            GameObject menuIcon = Instantiate(
                menuController.chooseSidesMenuIcon, 
                Vector3.zero,
                Quaternion.identity
            );
            if(menuController.teamSelectionStatus == "neutral"){
                menuIcon.transform.SetParent(neutralSlots[numberOfNeutralPlayers], false);
                numberOfNeutralPlayers ++;
            }else if(menuController.teamSelectionStatus == "home"){
                menuIcon.transform.SetParent(homeSlots[numberOfHomePlayers], false);
                numberOfHomePlayers ++;
            }else if (menuController.teamSelectionStatus == "away"){
                menuIcon.transform.SetParent(awaySlots[numberOfAwayPlayers], false);
                numberOfAwayPlayers ++;
            }
        }
    }
    private void CheckButtonStatus(){
        // AcceptButton - make sure there is at least 1 home or away player
        MenuController[] menuControllers = FindObjectsOfType<MenuController>();
        int playersThatHaveATeam = 0;
        for(int i = 0; i < menuControllers.Length; i++){
            if(menuControllers[i].teamSelectionStatus == "home" || menuControllers[i].teamSelectionStatus == "away"){
                playersThatHaveATeam++;
            }
        }
        // There must be at least 1 player on a team 
        if(playersThatHaveATeam != 0){
            if(acceptbutton.interactable == false){
                acceptbutton.interactable = true;
                EventSystem.current.SetSelectedGameObject(acceptbutton.gameObject);
            }
        } else {
            acceptbutton.interactable = false;
        }
    }
    
    void Update()
    {
        CheckButtonStatus();
        PositionInputControllerIcons();
    }
}
