using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    public Button playButton; // button that is the play button

    void Start()
    {
        playButton.onClick.AddListener(PlayGameClick); // getting the button ready to be clicked
    }

    void PlayGameClick()
    {
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single); // loading the game on button press
    }
}
