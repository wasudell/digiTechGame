using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBlueSiren : MonoBehaviour
{
    public Material blueLightOff;
    public Material blueLightOn;

    void Start()
    {
        StartCoroutine(blueSiren());
    }

    IEnumerator blueSiren(){
        GetComponent<Renderer>().material = blueLightOn;
        yield return new WaitForSeconds(1);
        GetComponent<Renderer>().material = blueLightOff;
        yield return new WaitForSeconds(1);
        StartCoroutine(blueSiren());
    }
}
