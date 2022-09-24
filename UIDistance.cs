using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDistance : MonoBehaviour
{
    public PostalCannon script; // script with the distance to house
    public float roundedDist; // integer that rounds that distance
    public Text txt; // text to be edited

    void Update()
    {
        roundedDist = Mathf.Round(script.distance); // rounding the distance to the house
        txt.text = roundedDist + "m away"; // displaying that rounded distance in a text box
    }
}