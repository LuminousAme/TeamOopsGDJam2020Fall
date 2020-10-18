using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScript : MonoBehaviour
{
    //stores the player's score
    public static int score = 0;
    //reference to the ui text element displaying the score
    Text scoreText;


    // Start is called before the first frame update
    void Start()
    {
        //make sure the text points to the correct object
        scoreText = gameObject.GetComponent<Text>();
        score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //update the text every frame to match the current score
        scoreText.text = "" + score;
    }
}
