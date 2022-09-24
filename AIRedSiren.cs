using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRedSiren : MonoBehaviour
{
    public Material redLightOff; // material for when red light is off
    public Material redLightOn; // material for when red light is on

    void Start()
    {
        StartCoroutine(redSiren());
    }

    IEnumerator redSiren(){ // timer that sets material to red light on, waits a second, turns material to red light off, wait a second, then repeat
        GetComponent<Renderer>().material = redLightOff;
        yield return new WaitForSeconds(1);
        GetComponent<Renderer>().material = redLightOn;
        yield return new WaitForSeconds(1);
        StartCoroutine(redSiren());
    }
}
