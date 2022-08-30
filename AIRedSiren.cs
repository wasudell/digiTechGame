using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRedSiren : MonoBehaviour
{
    // Start is called before the first frame update
    public Material redLightOff;
    public Material redLightOn;

    void Start()
    {
        StartCoroutine(redSiren());
    }

    IEnumerator redSiren(){
        GetComponent<Renderer>().material = redLightOff;
        yield return new WaitForSeconds(1);
        GetComponent<Renderer>().material = redLightOn;
        yield return new WaitForSeconds(1);
        StartCoroutine(redSiren());
    }
}
