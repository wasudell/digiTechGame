using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScore : MonoBehaviour
{
    // declaring text box that needs changing
    public Text txt;
    public int score = 0;
    private bool scoreIncrease = true;

    void Update()
    {
        // updating the text box every frame to the correct score
        txt.text = "Score: " + score;
    }

    // function that is declared in the package's to increase the score by one
    public void IncreaseScore(){
        // checks that it can increase score
        if (scoreIncrease == true){
            score = score + 1;
            StartCoroutine(increaseAllowed());
        }
    }

    // keeps the score from updating for 8 seconds in case the package hits the house's collider again
    IEnumerator increaseAllowed(){
        scoreIncrease = false;
        yield return new WaitForSeconds(8);
        scoreIncrease = true;
    }
}
