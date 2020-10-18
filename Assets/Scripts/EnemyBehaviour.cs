using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    //a static list of all the enemies 
    public static List<EnemyBehaviour> EnemyList = new List<EnemyBehaviour>();
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

    //The transform of the player 
    private Transform playerTrans;
    //Speed the enemy moves
    public float enemySpeed;
    //controlling knocking the enemy back after they damage the player
    private float knockBackTimeRemaining = 0.0f;

    //the audiosource for the enemy death sound
    public AudioSource deathSFX;

    //runs first frame the object exists
    void Start()
    {
        //add the enemy to the list 
        EnemyList.Add(this);
        //save the index it is in the list (this is used to cleanly destroy it later) 
        index = EnemyList.Count - 1;
        //set this gameobject tag to enemy 
        gameObject.tag = "Enemy";
        //get the reference to the player's transform
        playerTrans = GameObject.Find("PlayerController").GetComponent<Transform>();
        //get the reference to the deaht sound effect
        deathSFX = GameObject.Find("EnemyDeathSFXHolder").GetComponent<AudioSource>();

        //calculate and set the enemy's position based on a random position, but make sure they never spawn in range to target the player
        transform.position = new Vector3(Random.Range(-8f, 5f), Random.Range(-5f, 1.5f), -0.05f);
        //check if they are in range of the player
        while (Vector3.Distance(transform.position, playerTrans.position) < 3.5f)
        {
            //if they are, randomly generate a new position until you find one that is not
            transform.position = new Vector3(Random.Range(-8f, 5f), Random.Range(-5f, 1.5f), -0.05f);
        }

        //make it so enemies aren't drawn on the minimap
        gameObject.layer = 9;
    }

    //runs every frame
    void Update()
    {
        //check if the enemy is dead
        if(hp <= 0 && GameManage.GetState() == 1)
        {
            //check if the enemy still has box colliders, remove them if it does, we need to do this so the player doesn't take damage from dying enemies
            Component[] colliders;
            colliders = gameObject.GetComponents(typeof(BoxCollider2D));
            foreach (BoxCollider2D collider in colliders)
            {
                //destroy the collider
                Destroy(collider);
            }

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
        else if (GameManage.GetState() == 1)
        {
            //check if it is within 3 units of the player 
            if(Vector3.Distance(transform.position, playerTrans.position) < 3.0f)
            {
                //if it is, then try to move towards the player, but move away if they just damaged the player and are being knocked back
                Vector3 direction = playerTrans.position - transform.position;
                direction.Normalize();
                if(knockBackTimeRemaining <= 0.0f)
                    gameObject.GetComponent<Rigidbody2D>().MovePosition(transform.position += direction * enemySpeed * Time.deltaTime);
                else
                {
                    gameObject.GetComponent<Rigidbody2D>().MovePosition(transform.position += -direction * enemySpeed * Time.deltaTime);
                    knockBackTimeRemaining -= Time.deltaTime;
                }
          
            }
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
        //check if the enemy is at zero hp
        if(hp != 0)
        {
            //if it isn't, then play the damage sound effect
            gameObject.GetComponent<AudioSource>().Play();
        }
        else
        {
            //if it is, then play the death sound effect
            deathSFX.Play();
        }
        
    }

    //tells the enemy to be knocked back
    public void Knockback()
    {
        //set the enemy to be flying back for the next 0.5 seconds
        knockBackTimeRemaining = 0.5f;
    }
}
