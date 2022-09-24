using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMarker : MonoBehaviour
{
    public CarController van; // script with rotation of van
    public float yRotation; // y rotation of the van
    private Vector3 eulerRotation; // converting that rotation to a value in degrees
    private Quaternion rotation; // rotation to set the rotation of the marker to

    // Update is called once per frame
    void Update()
    {
        // Getting Y rotation of the van (direction facing)
        yRotation = van.vanRotation.eulerAngles.y + 90;
        // turning that into a vector for rotation of plane (in Z as rotation of map is on a different plane to van)
        eulerRotation = new Vector3(0, 0, -yRotation);
        // converting euler angles into quaternion
        rotation.eulerAngles = eulerRotation;
        // setting the rotation to that quaternion
        transform.rotation = rotation;
    }
}
