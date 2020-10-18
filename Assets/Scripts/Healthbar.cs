using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healthbar : MonoBehaviour
{
    //transform of the health bar, used to scale it and make it look like we're losing health
    private Transform bar;

    //variable to control how long the healthbar should flashing white and red when the player takes damage
    private float FlashTime = 0.0f;
    //trigger to control wheter it should currently be white or red
    private float colourChangeTime = 0.0f;
    //bool to check if the bar is currently white
    private bool isWhite = false;
    
    // Start is called before the first frame update
    void Start()
    {
        bar = transform.Find("Bar");
    }

    private void Update()
    {
        if(GameManage.GetState() == 1 && !GameManage.isOver)
        {
            //check if the bar should be flashing
            if (FlashTime > 0.0f)
            {
                //if it should, check if it should be changing colour
                if (colourChangeTime <= 0.0f)
                {
                    //if it should, then check which color it currently it 
                    if (isWhite)
                    {
                        //if it's white, make it red
                        bar.Find("BarSprite").GetComponent<SpriteRenderer>().color = Color.red;
                        isWhite = false;
                    }
                    else
                    {
                        //otherwise it must be red so make it white
                        bar.Find("BarSprite").GetComponent<SpriteRenderer>().color = Color.white;
                        isWhite = true;
                    }
                    //reset the time until next colour change
                    colourChangeTime = 0.05f;
                }

                //update the timers
                FlashTime -= Time.deltaTime;
                colourChangeTime -= Time.deltaTime;
            }
            else
            {
                if (isWhite)
                {
                    //if it's white, make it red
                    bar.Find("BarSprite").GetComponent<SpriteRenderer>().color = Color.red;
                    isWhite = false;
                }
            }
        }
    }

    public void SetSize(float healthPercent)
    {
        bar.localScale = new Vector3(healthPercent, 1.0f);
        FlashTime = 0.5f;
        colourChangeTime = 0.05f;
        //set the colour to white
        bar.Find("BarSprite").GetComponent<SpriteRenderer>().color = Color.white;
        isWhite = true;
    }
}
