using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIReverseLights : MonoBehaviour
{
    public AIController script; // script to find out if car is reversing
    public Material reverseLightsOff; // material of when the reverse lights are off
    public Material reverseLightsOn; // material of when the reverse lights are on
    
    void FixedUpdate(){
        if (script.velocity < -0.2 && script.vroomFactor < 0){
            GetComponent<Renderer>().material = reverseLightsOn; // changing the material to on reverse lights if the car is moving backwards and the car is being told to reverse
        }
        else {
            GetComponent<Renderer>().material = reverseLightsOff; // otherwise set material to reverse lights off
        }
    }
}
