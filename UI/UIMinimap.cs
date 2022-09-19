using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMinimap : MonoBehaviour
{
    public CarController van;

    private float xOffset;
    private float yOffset;
    private float yMovement;
    private float xMovement;
    public Vector2 Movement;

    void Start()
    {
        // getting offset which changes with screen size
        xOffset = transform.position.x;
        yOffset = transform.position.y;
        
    }

    void FixedUpdate()
    {   
        // converting the position of the van on the real map to movement of the image, movement is determined by the scale
        // of the minimap to the actual terrain (0.52...), while x and y offset change with screen size so must be declared and added
        yMovement= (van.transform.position.x * 0.5257525f) + yOffset;
        xMovement = (-van.transform.position.z * 0.5247864f) + xOffset;
        Movement = new Vector2(xMovement, yMovement);
        transform.position = Movement;
    }
}
