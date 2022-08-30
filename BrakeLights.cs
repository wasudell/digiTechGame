using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrakeLights : MonoBehaviour
{
    public CarController script;
    public Material brakeLightsOff;
    public Material brakeLightsOn;
    
    void FixedUpdate(){
        if (script.isBraking == true){
            GetComponent<Renderer>().material = brakeLightsOn;
        }
        else {
            GetComponent<Renderer>().material = brakeLightsOff;
        }
    }
}
