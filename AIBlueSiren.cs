using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBlueSiren : MonoBehaviour
{
    public Material blueLightOff; // material for when blue light is off
    public Material blueLightOn; // material for when blue light is on

    void Start()
    {
        StartCoroutine(blueSiren());
    }

    IEnumerator blueSiren(){ // timer that sets material to blue light on, waits a second, turns material to blue light off, wait a second, then repeat
        GetComponent<Renderer>().material = blueLightOn;
        yield return new WaitForSeconds(1);
        GetComponent<Renderer>().material = blueLightOff;
        yield return new WaitForSeconds(1);
        StartCoroutine(blueSiren());
    }
}
