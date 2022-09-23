using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    public Button playButton;

    void Start()
    {
        playButton.onClick.AddListener(PlayGameClick);       
    }

    void PlayGameClick()
    {
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
    }
}
