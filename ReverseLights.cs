using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseLights : MonoBehaviour
{
    public CarController script;
    public Material reverseLightsOff;
    public Material reverseLightsOn;
    
    void FixedUpdate(){
        if (script.carVelocity < -0.2 && Input.GetKey(KeyCode.S)){
            GetComponent<Renderer>().material = reverseLightsOn;
        }
        else {
            GetComponent<Renderer>().material = reverseLightsOff;
        }
    }
}