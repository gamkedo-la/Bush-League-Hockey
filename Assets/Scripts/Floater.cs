using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour
{
    public float degreesPerSecond = 15.0f;
    public float amplitude = 0.5f;
    public float frequency = 1f;
    private float defaultFrequency = 1f;
    Vector3 posOffset = new Vector3 ();
    Vector3 tempPos = new Vector3 ();
    // Start is called before the first frame update
    void Start()
    {
        degreesPerSecond = Random.Range(-15.0f,15.0f);
        posOffset = transform.position;
    }
    void Update()
    {
        tempPos = posOffset;
        tempPos.y += Mathf.Sin (Time.fixedTime * Mathf.PI * frequency) * amplitude;
        transform.position = tempPos;
    }
    public void ResetFrequency(){
        frequency = defaultFrequency + Random.Range((-0.12f * defaultFrequency), (0.12f * defaultFrequency));
    }
    public void SetNewFrequency(float newFrequency){
        frequency = newFrequency + Random.Range((-0.12f * newFrequency), (0.12f * newFrequency));
    }
}
