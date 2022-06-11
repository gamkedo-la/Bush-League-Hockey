using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpToPos : MonoBehaviour
{
    private Vector3 endPosition = Vector3(0,0,0);
    private Vector3 startPosition;
    private float desiredDuration = 3f;
    private float elapsedTime;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        float percentageComplete = elapsedTime / desiredDuration;

        transform.position = new Vector3.Lerp(startPosition, endPosition, percentageComplete);        
    }
}
