using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManage : MonoBehaviour
{

    //Current state of the game, 0 = main menu, 1 = game, 2 = game over.
    private static int currentState = 1;

    //how long before restarting the scene
    public float restartDelay = 5f;

    //gameobject for the enemy prefab so we can spawn more enemies 
    public GameObject enemyPrefab;
    //floats to control enemy respawning
    public float enemySpawnTime = 5.0f;
    public int enemySpawnFactor = 2;
    private float timeSinceLastSpawn = 0.0f;

    //causes a gameOver
    public void GameOver()
    {
        if(currentState != 2)
        {
            Debug.Log("GameOver");
            currentState = 2;
            Invoke("Restart", restartDelay);
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
        if(currentState == 1)
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

    }

}
