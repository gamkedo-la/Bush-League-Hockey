using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
public class GameStartScript : MonoBehaviour
{
    [Header("Main Menu")]
    public GameObject playButton;
    public GameObject creditsButton;
    public GameObject quitButton;
    public GameObject mainDisplay;
    [Header("Choose Sides Menu")]
    [SerializeField] List<Transform> homeSlots;
    [SerializeField] List<Transform> awaySlots;
    [SerializeField] List<Transform> neutralSlots;
    List<GameObject> menuIcons;
    public GameObject chooseSidesMenu;
    public GameObject acceptButton;
    [Header("Credits View")]
    public GameObject creditsDisplay;
    public GameObject backButton;
    public string sceneNameToLoad = "error";
    public void LoadSceneByName(){
        if(sceneNameToLoad=="error"){
        } else {
            SceneManager.LoadScene(sceneNameToLoad);
        }
    }
    
    public void SwitchToChooseSideMenu(){
        FindObjectOfType<MenuController>().SwitchOnChoosingSides();
        mainDisplay.SetActive(false);
        chooseSidesMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(acceptButton);
        HandleChooseSidePosition();
    }
    public void SwitchToMainDisplay(){
        mainDisplay.SetActive(true);
        creditsDisplay.SetActive(false);
        chooseSidesMenu.SetActive(false);
        FindObjectOfType<MenuController>().SwitchOffChoosingSides();
        EventSystem.current.SetSelectedGameObject(playButton);
    }
    public void SwitchToCreditsView(){
        FindObjectOfType<MenuController>().SwitchOffChoosingSides();
        creditsDisplay.SetActive(true);
        mainDisplay.SetActive(false);
        EventSystem.current.SetSelectedGameObject(backButton);
    }
    public void HandleChooseSidePosition(){
        InputManagerScript inputManager = GameObject.Find("InputManager").GetComponent<InputManagerScript>();
        int numberOfHomePlayers = 0;
        int numberOfAwayPlayers = 0;
        int numberOfNeutralPlayers = 0;
        foreach (GameObject icon in GameObject.FindGameObjectsWithTag("ControllerMenuIcon")){
            Destroy(icon);
        }
        foreach (GameObject ctrl in inputManager.localPlayerControllers){
            GameObject menuIcon = Instantiate(
                ctrl.GetComponent<MenuController>().chooseSidesMenuIcon, 
                Vector3.zero,
                Quaternion.identity
            );
            if (ctrl.GetComponent<MenuController>().teamSelectionStatus == "neutral"){
                menuIcon.transform.SetParent(neutralSlots[numberOfNeutralPlayers], false);
                numberOfNeutralPlayers ++;
            } else if(ctrl.GetComponent<MenuController>().teamSelectionStatus == "home"){
                menuIcon.transform.SetParent(homeSlots[numberOfHomePlayers], false);
                numberOfHomePlayers ++;
            } else {
                menuIcon.transform.SetParent(awaySlots[numberOfAwayPlayers], false);
                numberOfAwayPlayers ++;
            }
        }
    }
    public void SetPlayerInputSides(){
        InputManagerScript inputManager = GameObject.Find("InputManager").GetComponent<InputManagerScript>();
        foreach (GameObject ctrl in inputManager.localPlayerControllers){
            Debug.Log($"status: {ctrl.GetComponent<MenuController>().teamSelectionStatus}");
            ctrl.GetComponent<PlayerController>().isHomeTeam = ctrl.GetComponent<MenuController>().teamSelectionStatus == "home";
            DontDestroyOnLoad(ctrl.gameObject);
        }
        SceneManager.LoadScene("Hat-Trick");        
    }
    public void QuitGame(){
        Application.Quit();
    }
    private void Start() {
        EventSystem.current.SetSelectedGameObject(playButton);
    }
}
