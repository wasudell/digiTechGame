using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIScore : MonoBehaviour
{
    // declaring text box that needs changing
    public Text txt;
    public int score = 0;
    private bool scoreIncrease = true;
    public int highScore;
    // getting health from van
    public CarController van;
    // getting UI to destroy
    public Text Address;
    public Text nextAddress;
    public Text distance;
    public Image healthBarBorder;
    public Image healthBar;
    public Image mask;
    public RawImage border;
    public Image minimap;
    public RawImage marker;
    public Image background;
    private Color colour;
    public Button playAgainButton;
    public Text highScoreText;
    public Text gameOverText;
    public Button mainMenuButton;

    void Start(){
        // getting the current high score
        highScore = PlayerPrefs.GetInt("highScore");
        // end screen background
        // setting the size of the background to the screen size
        background.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
        // making the background invisible when the game is playing
        colour = background.color;
        colour.a = 0f;
        background.color = colour;
        // play again button
        // setting the button way off the screen so it isn't visible
        playAgainButton.transform.position = new Vector2(9999, 0);
        // allows the button to be clicked
        playAgainButton.onClick.AddListener(PlayAgainClick);
        // main menu button
        // same as play again button
        mainMenuButton.transform.position = new Vector2(9999, 0);
        // allows button to be clicked as well
        mainMenuButton.onClick.AddListener(MainMenuClick);
        // high score text
        // making the text invisible at start
        highScoreText.text = "";
        // game over text
        // making the text invisible
        gameOverText.text = "";
    }
    
    void Update()
    {
        // updating the text box every frame to the correct score
        txt.text = "Score: " + score;
        // destroying the UI
        if (van.endScreen == true){
            Destroy(Address);
            Destroy(nextAddress);
            Destroy(distance);
            Destroy(healthBarBorder);
            Destroy(healthBar);
            Destroy(mask);
            Destroy(border);
            Destroy(minimap);
            Destroy(marker);
            // moving the text down
            transform.position = new Vector2(transform.position.x - (Screen.width / 6), transform.position.y - (0.35f * Screen.height)); // 35% of screen down, and 1/6th to the left
            // background
            // making background visible again
            colour.a = 1f;
            background.color = colour;
            // play again button
            // moving the play again button
            playAgainButton.transform.position = new Vector2(Screen.width / 2, Screen.height / 2); // setting position to centre of screen
            playAgainButton.transform.position = new Vector2(playAgainButton.transform.position.x, playAgainButton.transform.position.y - (Screen.height / 12)); // down 1/12th of screen height
            // main menu button
            mainMenuButton.transform.position = new Vector2(Screen.width / 2, Screen.height / 2); // setting position to centre of screen
            mainMenuButton.transform.position = new Vector2(mainMenuButton.transform.position.x, mainMenuButton.transform.position.y - (Screen.height / 4)); // down 1/4th of screen height
            // high score text
            // bringing up the text once the end screen is called (player dies)
            highScoreText.text = "High Score: " + highScore;
            // moving the text
            highScoreText.transform.position = new Vector2(highScoreText.transform.position.x + (Screen.width / 6), highScoreText.transform.position.y - (0.35f * Screen.height)); // 1/6th to the right and 35% down (of screen)
            // game over text
            // making the text visible again
            gameOverText.text = "Game Over!";
            // moving the text in place
            gameOverText.transform.position = new Vector2(gameOverText.transform.position.x, gameOverText.transform.position.y - (Screen.height / 6)); // 1/6th of the screen down

            // making the code not loop
            van.endScreen = false;
        }
        // changing the highscore if the current score is higher
        if (score > highScore){
            highScore = score;
            PlayerPrefs.SetInt("highScore", highScore);
        }
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

    void PlayAgainClick(){
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
    }

    void MainMenuClick(){
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
