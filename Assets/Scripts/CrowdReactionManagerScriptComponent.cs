using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdReactionManagerScriptComponent : MonoBehaviour
{
    private List<GameObject> listOfHomeTeamCrowdGroups = new List<GameObject>();
    public List<GameObject> listOfAwayTeamCrowdGroups = new List<GameObject>();

    public List<GameObject> listOfHomeTeamEggPeople = new List<GameObject>();
    public List<GameObject> listOfAwayTeamEggPeople = new List<GameObject>();
    [Header("Design Settings")]
    [SerializeField] private float GoalExcitementFactorFloat;
    [SerializeField] private float OtherTeamsGoalSadnessFactorFloat;


    private void Awake()
    {
        InitializeArrays();
    }

    public IEnumerator HandleAwayTeamScoringAGoal()
    {
        AddExcitementToAwayTeam();
        AddSadnessToHomeTeam();

        yield return new WaitForSeconds(5);

        RemoveExcitementFromAwayTeam();
        RemoveSadnessFromHomeTeam();
    }

    public IEnumerator HandleHomeTeamScoringAGoal()
    {
        AddExcitementToHomeTeam();
        AddSadnessToAwayTeam();

        yield return new WaitForSeconds(5);

        RemoveExcitementFromHomeTeam();
        RemoveSadnessFromAwayTeam();
    }

    private void AddExcitementToAwayTeam()
    {
        for (int i = 0; i < listOfAwayTeamEggPeople.Count; i++)
        {
            listOfAwayTeamEggPeople[i].transform.GetComponent<Floater>().frequency *= GoalExcitementFactorFloat;
        }
    }
    private void AddExcitementToHomeTeam()
    {
        for (int i = 0; i < listOfHomeTeamEggPeople.Count; i++)
        {
            listOfHomeTeamEggPeople[i].transform.GetComponent<Floater>().frequency *= GoalExcitementFactorFloat;
        }
    }
    public void RemoveExcitementFromAwayTeam()
    {
        for (int i = 0; i < listOfAwayTeamEggPeople.Count; i++)
        {
            listOfAwayTeamEggPeople[i].transform.GetComponent<Floater>().frequency /= GoalExcitementFactorFloat;
        }
    }
    public void RemoveExcitementFromHomeTeam()
    {
        for (int i = 0; i < listOfHomeTeamEggPeople.Count; i++)
        {
            listOfHomeTeamEggPeople[i].transform.GetComponent<Floater>().frequency /= GoalExcitementFactorFloat;
        }
    }
    private void AddSadnessToHomeTeam()
    {
        for (int i = 0; i < listOfHomeTeamEggPeople.Count; i++)
        {
            listOfHomeTeamEggPeople[i].transform.GetComponent<Floater>().frequency /= OtherTeamsGoalSadnessFactorFloat;
        }
    }
    private void AddSadnessToAwayTeam()
    {
        for (int i = 0; i < listOfAwayTeamEggPeople.Count; i++)
        {
            listOfAwayTeamEggPeople[i].transform.GetComponent<Floater>().frequency /= OtherTeamsGoalSadnessFactorFloat;
        }
    }
    private void RemoveSadnessFromHomeTeam()
    {
        for (int i = 0; i < listOfHomeTeamEggPeople.Count; i++)
        {
            listOfHomeTeamEggPeople[i].transform.GetComponent<Floater>().frequency *= OtherTeamsGoalSadnessFactorFloat;
        }
    }
    private void RemoveSadnessFromAwayTeam()
    {
        for (int i = 0; i < listOfAwayTeamEggPeople.Count; i++)
        {
            listOfAwayTeamEggPeople[i].transform.GetComponent<Floater>().frequency *= OtherTeamsGoalSadnessFactorFloat;
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
            }
        }

        for (int i = 0; i < listOfAwayTeamCrowdGroups.Count; i++)
        {
            foreach (Transform childEggPerson in listOfAwayTeamCrowdGroups[i].transform)
            {
                listOfAwayTeamEggPeople.Add(childEggPerson.gameObject);
            }
        }
    }    
}
