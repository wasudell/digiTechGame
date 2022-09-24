using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public CarController van; // script with van health
    public Vector3 offset; // offset of camera between van position and desired camera position
    public Transform target; // target camera has to follow
    public float translateSpeed; // speed at which camera moves
    public float rotationSpeed; // speed at which camera rotates
    
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
        var targetPosition = target.TransformPoint(offset); // decaring the position at which the camera needs to go, accounting for offset
        transform.position = Vector3.Lerp(transform.position, targetPosition, translateSpeed * Time.deltaTime); // creating a path between the camera's current position and desired position, then moving along that path
    }

    void HandleRotation()
    {
        var direction = target.position - transform.position; // direction camera needs to face
        var rotation = Quaternion.LookRotation(direction, Vector3.up); // rotation that camera needs to achieve
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime); // finding a way to get from the camera's current rotation to the desired rotation, then executing that way
    }
}
