using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManage : MonoBehaviour
{

    //Current state of the game, 0 = main menu, 1 = game, 2 = game over, 3 = credits
    private static int currentState = 1;

    //how long before restarting the scene
    public float gameOverDelay = 5f;

    //gameobject for the enemy prefab so we can spawn more enemies 
    public GameObject enemyPrefab;
    //floats to control enemy respawning
    public float enemySpawnTime = 5.0f;
    public int enemySpawnFactor = 2;
    private float timeSinceLastSpawn = 0.0f;

    //image reference so we can make it look like a fade like black on game over
    public Image blackScreen;
    //boolean to determine in the blackscreen should be fading in
    public static bool isOver = false;

    //causes a gameOver
    public void GameOver()
    {
        if(currentState != 2)
        {
            Debug.Log("GameOver");
            isOver = true;
            Invoke("GameOverDelayed", gameOverDelay);
        }
    }

    public static int GetState()
    {
        return currentState;
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        EnemyBehaviour.EnemyList = new List<EnemyBehaviour>();
        currentState = 1;
    }

    //called once a frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Credits")
            currentState = 3;

        if (currentState == 1 && !isOver)
        {
            //check if it's time to spawn enemies
            if (timeSinceLastSpawn >= enemySpawnTime)
            {
                //if it is, then spawn a number of enemies based on player's score and the spawn factor
                for (int i = 0; i < ((ScoreScript.score * enemySpawnFactor) + 1); i++)
                    Instantiate(enemyPrefab);
                //and reset the timer
                timeSinceLastSpawn = 0.0f;
            }
            //add the change in time to the timer
            timeSinceLastSpawn += Time.deltaTime;
        }
        //if you're still in the game but it has ended have it fade to a black screen
        else if (currentState == 1 && isOver) {
            Color col = blackScreen.color;
            col.a += Time.deltaTime / gameOverDelay;
            blackScreen.color = col;
        }
    }

    //function to start the game when pressed in either the main menu or the game over screen
    public void StartGame()
    {
        Invoke("StartGameDelayed", 1f);
    }

    //start game calls this with invoke so the player has time to see that the button has been pressed and adjust
    private void StartGameDelayed()
    {
        SceneManager.LoadScene("Game");
        currentState = 1;
        isOver = false;
    }

    //function to go to the credits
    public void Credits()
    {
        Invoke("CreditsDelayed", 1f);
    }

    //credits calls this with invoke so the player has time to see that the button has been pressed and adjust
    private void CreditsDelayed()
    {
        Cursor.visible = true;
        SceneManager.LoadScene("Credits");
        currentState = 3;
    }

    //GameOver calls this with invoke so the player has time to see that the button has been pressed and adjust
    private void GameOverDelayed()
    {
        Cursor.visible = true;
        SceneManager.LoadScene("Game over menu");
        currentState = 2;
    }

    //Function to go to the main menu
    public void MainMenu()
    {
        Invoke("MainMenuDelay", 1f);
    }

    //MainMenu calls this with invoke so the player has time to see that the button has been pressed and adjust
    private void MainMenuDelay()
    {
        Cursor.visible = true;
        SceneManager.LoadScene("Main Menu");
        currentState = 0;
    }

    //Function to quit the game
    public void Quit()
    {
        Invoke("QuitDelayed", 1f);
    }

    //Quit calls this with invoke so the player has time to see that button has been pressed 
    private void QuitDelayed()
    {
        Application.Quit();
    }

}
