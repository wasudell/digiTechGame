using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrakeLights : MonoBehaviour
{
    public CarController script; // script to find if van is braking
    public Material brakeLightsOff; // material of when brake lights are off
    public Material brakeLightsOn; // material of when brake lights are on
    
    void FixedUpdate(){
        if (script.isBraking == true){
            GetComponent<Renderer>().material = brakeLightsOn; // setting the material to on brake lights if the van is braking
        }
        else {
            GetComponent<Renderer>().material = brakeLightsOff; // otherwise set the material to brake lights off
        }
    }
}
