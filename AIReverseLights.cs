using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIReverseLights : MonoBehaviour
{
    public AIController script;
    public Material reverseLightsOff;
    public Material reverseLightsOn;
    
    void FixedUpdate(){
        if (script.velocity < -0.2 && script.vroomFactor < 0){
            GetComponent<Renderer>().material = reverseLightsOn;
        }
        else {
            GetComponent<Renderer>().material = reverseLightsOff;
        }
    }
}
