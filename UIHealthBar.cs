using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    public CarController van; // script with health of van
    public Image healthBar; // image representing health of van as a percentage

    void FixedUpdate()
    {
        // setting the scale of the health bar to the player's health divided by 100 (percentage)
        healthBar.transform.localScale = new Vector2((van.health / 100), 1);
        // saying that if the player's health is below 0, the scale shall stay as 0. This is so the bar
        // doesn't have negative scale and go beyond the bounds of it's border
        if (van.health <= 0){
            healthBar.transform.localScale = new Vector2 (0, 1);
        }
    }
}
