using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    void Start()
    {
        // unparenting the explosion from the van so it doesn't fling around
        transform.SetParent(null);
        // moving the explosion down
        transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
    }
}
