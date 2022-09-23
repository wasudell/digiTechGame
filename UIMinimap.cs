using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMinimap : MonoBehaviour
{
    public CarController van;
    public Image minimap;

    private float xOffset;
    private float yOffset;
    public float scale;
    private float yMovement;
    private float xMovement;
    public float testX;
    public float testY;

    void Start()
    {
        // getting offset which changes with screen size
        xOffset = transform.position.x;
        yOffset = transform.position.y;
    }

    void FixedUpdate()
    {  
        scale = (Screen.width / 513f) * 0.17f;
        // converting the position of the van on the real map to movement of the image, movement is determined by the scale
        // of the minimap to the actual terrain, while x and y offset change with screen size so must be declared and added
        yMovement= (van.transform.position.x * scale) + yOffset;
        xMovement = (-van.transform.position.z * scale) + xOffset;
        minimap.transform.position = new Vector2(xMovement, yMovement);
    }
}
