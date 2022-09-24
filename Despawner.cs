using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Despawner : MonoBehaviour
{
    // getting 2 scripts, both the cannon script so it can know the house that is
    // getting delivered to and the score text so it can update the score
    public PostalCannon script;
    public UIScore Score;
    // setting the frames that the package is allowed to exist
    public int framesAllowed;
    public int frames;

    void Start()
    {
        // getting the scripts from the postal cannon and score text
        script = FindObjectOfType<PostalCannon>();
        Score = FindObjectOfType<UIScore>();
    }

    void FixedUpdate()
    {
        frames = frames + 1;
        // destroying the package once the number of frames allowed is past
        if (frames > framesAllowed){
            Destroy(gameObject);
        }
    }

    // tells the score script to increase score by 1 if the correct house is hit
    void OnCollisionEnter(Collision collision){
        if (collision.gameObject == script.previousHouse){
            Score.IncreaseScore();
        }
    }
}
