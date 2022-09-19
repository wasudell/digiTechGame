using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMarker : MonoBehaviour
{
    public CarController van;
    public float yRotation;
    private Vector3 eulerRotation;
    private Quaternion rotation;

    // Update is called once per frame
    void Update()
    {
        // Getting Y rotation of the van (direction facing)
        yRotation = van.vanRotation.eulerAngles.y + 90;
        // turning that into a vector for rotation of plane (in Z as rotation of map is on a different plane to van)
        eulerRotation = new Vector3(0, 0, -yRotation);
        // converting euler angles into quaternion
        rotation.eulerAngles = eulerRotation;
        transform.rotation = rotation;
    }
}
