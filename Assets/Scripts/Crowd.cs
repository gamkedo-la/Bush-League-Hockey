using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crowd : MonoBehaviour
{
    //adjust this to change speed
    public float speed = 10f;
    //adjust this to change how high it goes
    public float height = 0.1f;

    private float phase = 0f; // start at random time

    void Start() {
        phase = Random.value * 5f;
    }

    void Update() {
        //get the objects current position and put it in a variable so we can access it later with less code
        Vector3 pos = transform.position;
        //calculate what the new Y position will be
        float newY = pos.y + (Mathf.Sin((Time.time+phase) * speed) * height);
        //set the object's Y to the new calculated Y
        transform.position = new Vector3(pos.x, newY, pos.z);
    }
}
