using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdReactionManagerScriptComponent : MonoBehaviour
{
    private GameSystem gameSystem;
    [SerializeField] TimeProvider timeProvider;
    private List<GameObject> listOfHomeTeamCrowdGroups = new List<GameObject>();
    public List<GameObject> listOfAwayTeamCrowdGroups = new List<GameObject>();

    public List<GameObject> listOfHomeTeamEggPeople = new List<GameObject>();
    public List<Floater> listOfHomeTeamFloaters = new List<Floater>();
    public List<GameObject> listOfAwayTeamEggPeople = new List<GameObject>();
    public List<Floater> listOfAwayTeamFloaters = new List<Floater>();
    [Header("Design Settings")]
    [SerializeField] [Range(0, 5f)] private float baseFrequency;
    [SerializeField] [Range(5f, 12f)] private float goalExcitementFactor;
    [SerializeField] [Range(0.01f, 0.5f)] private float goalAgainstFactor;
    private void Awake()
    {
        gameSystem = FindObjectOfType<GameSystem>();
        InitializeArrays();
    }
    private void Start() {
        CountGoals.awayGoalScored += AwayGoal;
        CountGoals.homeGoalScored += HomeGoal;
        FaceOffState.onStateEnter += HandleFaceOffEnter;
        BigCelebration.celebrate += HandleEndOfGameCelebration;
    }
    private void AwayGoal(object sender, EventArgs e){
        foreach (Floater floater in listOfAwayTeamFloaters){
            floater.SetNewFrequency(goalExcitementFactor);
        }
        foreach (Floater floater in listOfHomeTeamFloaters){
            floater.SetNewFrequency(goalAgainstFactor);
        }
    }
    private void HomeGoal(object sender, EventArgs e){
        foreach (Floater floater in listOfAwayTeamFloaters){
            floater.SetNewFrequency(goalAgainstFactor);
        }
        foreach (Floater floater in listOfHomeTeamFloaters){
            floater.SetNewFrequency(goalExcitementFactor);
        }
    }
    private void HandleFaceOffEnter(object sender, EventArgs e){
        NormalizeEntireCrowd();
    }
    private void NormalizeEntireCrowd()
    {
        foreach (Floater f in listOfHomeTeamFloaters)
        {
            f.ResetFrequency();
        }
        foreach (Floater f in listOfAwayTeamFloaters)
        {
            f.ResetFrequency();
        }
    }
    private void HandleEndOfGameCelebration(object sender, EventArgs e){
        NormalizeEntireCrowd();
        if(gameSystem.currentGameData.homeScore < gameSystem.currentGameData.awayScore){
            AwayGoal(null, null);
        } else {
            HomeGoal(null, null);
        }
    }
    private void InitializeArrays()
    {
        GameObject[] crowdGroupAArray = GameObject.FindGameObjectsWithTag("CrowdGroupA");
        GameObject[] crowdGroupBArray = GameObject.FindGameObjectsWithTag("CrowdGroupB");

        for (int i = 0; i < crowdGroupAArray.Length; i++)
        {
            listOfHomeTeamCrowdGroups.Add(crowdGroupAArray[i]);
        }
        for (int i = 0; i < crowdGroupBArray.Length; i++)
        {
            listOfAwayTeamCrowdGroups.Add(crowdGroupBArray[i]);
        }
        for (int i = 0; i < listOfHomeTeamCrowdGroups.Count; i++)
        {
            foreach (Transform childEggPerson in listOfHomeTeamCrowdGroups[i].transform)
            {
                listOfHomeTeamEggPeople.Add(childEggPerson.gameObject);
                listOfHomeTeamFloaters.Add(childEggPerson.gameObject.GetComponent<Floater>());
            }
        }
        for (int i = 0; i < listOfAwayTeamCrowdGroups.Count; i++)
        {
            foreach (Transform childEggPerson in listOfAwayTeamCrowdGroups[i].transform)
            {
                listOfAwayTeamEggPeople.Add(childEggPerson.gameObject);
                listOfAwayTeamFloaters.Add(childEggPerson.gameObject.GetComponent<Floater>());
            }
        }
    }
}
