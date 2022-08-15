using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKSkateCycle : MonoBehaviour
{
    [SerializeField] GameObject rightFootControl;
    [SerializeField] GameObject rightFootHint;
    [SerializeField] GameObject leftFootControl;
    [SerializeField] GameObject leftFootHint;
    private float skateCycleProgress; // from 0 to 1 how far are we in the skate cycle
    // could connect this with a 'curve' to make the speed variable
    private void Update(){
        // use splines to set foot locations
    }
}
