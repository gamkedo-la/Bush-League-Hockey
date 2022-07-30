using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdReactionManagerScriptComponent : MonoBehaviour
{
    private List<GameObject> listOfCrowdGroupsA = new List<GameObject>();
    public List<GameObject> listOfCrowdGroupsB = new List<GameObject>();
    public List<GameObject> listOfEggPeopleToExciteB = new List<GameObject>();
    [Header("Design Settings")]
    [SerializeField] private float GoalExcitementFactorFloat;


    private void Awake()
    {
        GameObject[] crowdGroupAArray = GameObject.FindGameObjectsWithTag("CrowdGroupA");
        GameObject[] crowdGroupBArray = GameObject.FindGameObjectsWithTag("CrowdGroupB");

        for (int i = 0; i < crowdGroupAArray.Length; i++)
        {
            listOfCrowdGroupsA.Add(crowdGroupAArray[i]);
        }    

        for (int i = 0; i < crowdGroupBArray.Length; i++)
        {
            listOfCrowdGroupsB.Add(crowdGroupBArray[i]);
        }

        for (int i = 0; i < listOfCrowdGroupsB.Count; i++)
        {
            foreach (Transform childEggPerson in listOfCrowdGroupsB[i].transform)
            {
                listOfEggPeopleToExciteB.Add(childEggPerson.gameObject);
            }
        }    
        

        //for (int i = 0; i < listOfCrowdGroupsB.Count; i++)
        //{
        //    Debug.Log(listOfCrowdGroupsB[i]);
        //}    
    }

    public void AddGoalExcitement()
    {
       
        for (int i = 0; i < listOfEggPeopleToExciteB.Count; i++)
        {

            listOfEggPeopleToExciteB[i].transform.GetComponent<Floater>().frequency *= GoalExcitementFactorFloat;        
        }
    }    

    public void TakeAwayGoalExcitementFactor()
    {
        for (int i = 0; i < listOfEggPeopleToExciteB.Count; i++)
        {

            listOfEggPeopleToExciteB[i].transform.GetComponent<Floater>().frequency /= GoalExcitementFactorFloat;
        }
    }    
}
