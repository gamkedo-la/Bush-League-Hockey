using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TeamMember : MonoBehaviour
{
    private GameSystem gameSystem;
    [SerializeField] public bool isHomeTeam;
    private void Awake(){
        gameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
    }
    public void SetIsHomeTeam(bool isHome){
        isHomeTeam = isHome;
    }
}
