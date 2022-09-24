using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBrakeLights : MonoBehaviour
{
    public AIController script; // script to find if car is braking
    public Material brakeLightsOff; // material of when brake lights are off
    public Material brakeLightsOn; // material of when brake lights are on
    
    void FixedUpdate(){
        if (script.isBraking == true){
            GetComponent<Renderer>().material = brakeLightsOn; // setting the material to on brake lights if the car is braking
        }
        else {
            GetComponent<Renderer>().material = brakeLightsOff; // otherwise set the material to brake lights off
        }
    }
}
