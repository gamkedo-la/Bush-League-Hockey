using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamMember : MonoBehaviour
{
    private GameSystem gameSystem;
    public bool memberHasPosession = false;
    private GameObject puckObject;
    public int teamIndex; // 1 home, 2 away
    private void Awake() {
        gameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
    }
    public void SetTeamIndex(int index){
        teamIndex = index;
    }
    public void TakePosession(){
        Debug.Log("Taking Posession");
        gameSystem.
        memberHasPosession = true;
    }
    public void LosePosession(){
        Debug.Log("Lost Posession");
        memberHasPosession = false;
        puckObject.transform.SetParent(gameSystem.transform, true);
    }
}
