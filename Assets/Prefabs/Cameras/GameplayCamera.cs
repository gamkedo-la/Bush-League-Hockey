using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayCamera : MonoBehaviour
{
    [SerializeField] TimeProvider timeProvider;
    private GameSystem gameSystem;
    private Vector3 cameraDesiredPosition;
    private void Awake() {
        gameSystem = FindObjectOfType<GameSystem>();
    }
    void FixedUpdate()
    {
        cameraDesiredPosition.x = 0.4f*gameSystem.puckObject.transform.position.x;
        cameraDesiredPosition.y = transform.position.y;
        cameraDesiredPosition.z = transform.position.z;
        transform.position = Vector3.Lerp(
            transform.position,
            cameraDesiredPosition,
            timeProvider.fixedDeltaTime
        );        
    }
}
