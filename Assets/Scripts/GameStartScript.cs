using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartScript : MonoBehaviour
{
    public string sceneNameToLoad = "error";
    public void LoadSceneByName() {
        if(sceneNameToLoad=="error") {
            Debug.LogError("scene name not set in inspector for this function");
        } else {
            SceneManager.LoadScene(sceneNameToLoad);
        }
    }
}
