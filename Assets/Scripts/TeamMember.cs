using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamMember : MonoBehaviour
{
    private GameSystem gameSystem;
    public bool isHomeTeam; // 1 home, 2 away
    private void Awake(){
        gameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        SetIsHomeTeam(true);
    }
    public void SetIsHomeTeam(bool isHome){
        isHomeTeam = isHome;
    }
    
}
