using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    //a static list of all the enemies 
    private static List<EnemyBehaviour> EnemyList = new List<EnemyBehaviour>();
    //this enemy's index in the list 
    public int index;

    //enemy hp
    public int hp = 1;
    //the time after an enemy dies where it slowly fades out 
    private float fadeTime = 0.5f;
    //the parimeter telling how far along the fade it currently is, serves as the t while lerping the alpha.
    private float fadeProgress = 0.0f;

    //The attached enemy sprite 
    public SpriteRenderer EnemySprite;

    //runs first frame the object exists
    void Start()
    {
        //add the enemy to the list 
        EnemyList.Add(this);
        //save the index it is in the list (this is used to cleanly destroy it later) 
        index = EnemyList.Count - 1;
    }

    //runs every frame
    void Update()
    {
        //check if the enemy is dead
        if(hp <= 0)
        {
            //if it, have the enemy fade out
            //update the t for the alpha lerp 
            fadeProgress = Mathf.Min(fadeProgress + Time.deltaTime / fadeTime, 1.0f);
            //grab the color 
            Color EnemyColor = EnemySprite.color;
            //and lerp it's alpha
            EnemyColor.a = Mathf.Lerp(1.0f, 0.0f, fadeProgress);
            EnemySprite.color = EnemyColor;
            //when it's completely faded out, delete the enemy 
            if (fadeProgress >= 1.0f)
            {
                //remove it from the list
                EnemyList.RemoveAt(index);
                //reset the indices of all the enemies
                for (int i = 0; i < EnemyList.Count; i++)
                    EnemyList[i].index = i;
                Destroy(gameObject);
            }
        }
        //if it isn't run other logic 
        else
        {

        }
    }

    public static EnemyBehaviour GetClosestEnemyWithinRange(Vector3 pos, float range)
    {
        EnemyBehaviour closestEnemy = null;
        //run through every enemy in the list 
        for(int i = 0; i < EnemyList.Count; i++)
        {
            //check if it's still alive, and within the range
            if(EnemyList[i].hp > 0 && Vector3.Distance(pos, EnemyList[i].transform.position) <= range)
            {
                //if it is then check if any of the earlier enemies were also in range
                if(closestEnemy == null)
                {
                    //if they weren't, then this enemy is the cloest enemy found thus far
                    closestEnemy = EnemyList[i];
                }
                //if they are, compared against the previously found to the be closest and see if this one is closer 
                else if(Vector3.Distance(pos, EnemyList[i].transform.position) < Vector3.Distance(pos, closestEnemy.transform.position))
                {
                    //if this one is closer, it's the new closet
                    closestEnemy = EnemyList[i];
                }
            }
        }
        //after exiting that loop you have the closest enemy if any exist within the range, if they don't it just returns null
        return closestEnemy;
    }

    //deals 1 damage to the enemy
    public void Damage()
    {
        //decrement hp
        hp--;
    }

}
