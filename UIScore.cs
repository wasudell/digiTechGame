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
    public Button mainMenuButton;
    public Button resumeButton;
    public Text highScoreText;
    public Text gameOverText;
    // pause menu checker
    public bool pauseMenu = false;

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

        // resume button
        // setting the button way off the screen so it isn't visible
        resumeButton.transform.position = new Vector2(9999, 0);
        // allows the button to be clicked
        resumeButton.onClick.AddListener(ResumeClick);

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
        // moving the text in place
        gameOverText.transform.position = new Vector2(gameOverText.transform.position.x, gameOverText.transform.position.y - (Screen.height / 6)); // 1/6th of the screen down

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

            // making the code not loop
            van.endScreen = false;
        }

        // changing the highscore if the current score is higher
        if (score > highScore){
            highScore = score;
            PlayerPrefs.SetInt("highScore", highScore);
        }

        // Pause Menu
        if (Input.GetKeyDown("escape") && pauseMenu == false){ // get button down so the code doesn't get run over multiple frames
            // setting so that things don't change when escape is pressed again, timed as will open and close menu in same frame if not
            StartCoroutine(wait());

            // setting the time to 0 so that the game pauses in the background
            Time.timeScale = 0.000001f;

            // game over text (using game over text as it is basically the same thing)
            // making the text visible again
            gameOverText.text = "Game Paused";

            // background
            // making background visible again
            colour.a = 1f;
            background.color = colour;

            // main menu button
            mainMenuButton.transform.position = new Vector2(Screen.width / 2, Screen.height / 2); // setting position to centre of screen
            mainMenuButton.transform.position = new Vector2(mainMenuButton.transform.position.x, mainMenuButton.transform.position.y - (Screen.height / 4)); // down 1/4th of screen height

            // resume button
            // moving the resume button
            resumeButton.transform.position = new Vector2(Screen.width / 2, Screen.height / 2); // setting position to centre of screen
            resumeButton.transform.position = new Vector2(resumeButton.transform.position.x, resumeButton.transform.position.y - (Screen.height / 12)); // down 1/12th of screen height

            // turning off the game's audio
            AudioListener.pause = true;
        }

        // setting so that if escape is pressed when the pause menu is active, the game resumes
        if (Input.GetKeyDown("escape") && pauseMenu == true){
            // same function as resume button
            ResumeClick(); // doesn't need waiter as it is placed after pause menu opener so wont run until next frame
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
        // reloading the game if play again is pressed
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
    }

    void MainMenuClick(){
        // loading the main menu if main menu is pressed
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    void ResumeClick(){
        // resuming the game if the resume button is clicked

        // setting the time back to normal
        Time.timeScale = 1;

        // setting everything back to as it was as the game is playing
        // making the background invisible when the game is playing
        colour = background.color;
        colour.a = 0f;
        background.color = colour;

        // resume button
        // setting the button way off the screen so it isn't visible
        resumeButton.transform.position = new Vector2(9999, 0);

        // main menu button
        // same as play again button
        mainMenuButton.transform.position = new Vector2(9999, 0);

        // game over text
        // making the text invisible
        gameOverText.text = "";

        // allowing pause menu to be reactivated
        pauseMenu = false;

        // resuming the game's audio
        AudioListener.pause = false;
    }

    IEnumerator wait(){ // see line 134
        yield return new WaitForSeconds(0.0000001f);
        pauseMenu = true;
    }
}
