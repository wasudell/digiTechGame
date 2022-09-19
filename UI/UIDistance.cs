using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDistance : MonoBehaviour
{
    public PostalCannon script;
    public float roundedDist;
    public Text txt;

    void Update()
    {
        roundedDist = Mathf.Round(script.distance);
        txt.text = roundedDist + "m away";
    }
}