using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMinimap : MonoBehaviour
{
    public CarController van; // script with position of van
    public Image minimap; // image that has minimap on it to move around

    private float xOffset; // offset from global centre on the x axis
    private float yOffset; // same but on the y axis
    public float scale; // scale between minimap image and actual terrain
    private float yMovement; // movement that the image has to move on the y axis
    private float xMovement; // same but on the x axis

    void Start()
    {
        // getting offset; which changes with screen size
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
