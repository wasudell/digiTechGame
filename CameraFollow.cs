using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public CarController van;
    public Vector3 offset;
    public Transform target;
    public float translateSpeed;
    public float rotationSpeed;
    
    void FixedUpdate()
    {
        // only move if van still exits (van gets removed when falling below 0 health)
        if (van.health > 0){
            HandleTranslation();
            HandleRotation();
        }
    }

    void HandleTranslation()
    {
        var targetPosition = target.TransformPoint(offset);
        transform.position = Vector3.Lerp(transform.position, targetPosition, translateSpeed * Time.deltaTime);
    }

    void HandleRotation()
    {
        var direction = target.position - transform.position;
        var rotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }
}
