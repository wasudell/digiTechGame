using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseLights : MonoBehaviour
{
    public CarController script; // script to find out if van is reversing
    public Material reverseLightsOff; // material of when the reverse lights are off
    public Material reverseLightsOn; // material of when the reverse lights are on
    
    void FixedUpdate(){
        if (script.carVelocity < -0.2 && Input.GetKey(KeyCode.S)){ // changing the material to on reverse lights if the van is moving backwards and the reverse key is being pressed
            GetComponent<Renderer>().material = reverseLightsOn;
        }
        else {
            GetComponent<Renderer>().material = reverseLightsOff; // otherwise set material to reverse lights off
        }
    }
}