using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //normalized vector representing the players direction
    private Vector3 velocity = Vector3.zero;
    //how fast the player should be moving
    public float speed = 1;


    //reference to our camera (needed to convert mouse coordinates to world space)
    public GameObject cam; 
    //position of the mouse
    private Vector2 mousePos;
    //transform of the crosshair 
    public Transform crosshair;
    //the gameobject for the bullet prefab
    public GameObject bulletPrefab;
    //the transform for the gun, this is needed for the rotation
    public Transform gunTrans;
    //gameobject with the particle system for when the gun fires
    public GameObject fireParticles;
    //gameobject with the gun sprite 
    public GameObject gunSprite;
    //audio source for the soundshot sound effect
    public AudioSource gunShotSFX;

    //player health
    public int maxHp = 5;
    private int hp;
    //audio source for the player taking damage sound effect
    public AudioSource playerDmgSFX;
    //audio source for the game over sound effect
    public AudioSource gameOverSFX;
    //reference to the healthbar
    public Healthbar bar;

    //variable controlling how long the player is being knocked back after getting hurt by an enemy
    private float knockBackTimeRemaining = 0.0f;
    //the direction the player should be knocked back into
    private Vector3 knockBackDir;

    //variable for wheter or not the player currently has treasure they need to deliver to their ship
    private bool hasTreasure;
    //gameobject to spawn a new treasure spot when the game starts or when the player delivers treasure to the ship
    public GameObject treasureSpawn;
    //particle system for when the score goes up
    public GameObject scoreIncreaseParticleSystem;
    //particle system for when the player discovers treasure
    public GameObject treasureFoundParticleSystem;
    //soundeffect when you deliver treasure and get a point
    public AudioSource scoreUpSFX;
    //sound effect when you dig up treasure
    public AudioSource treasureFoundSFX;

    //variable representing the pop up text telling the player how to get points
    public Text pointPrompt;

    //control player animator 
    public Animator animator;
    //control the player's sprite renderer
    public SpriteRenderer playerSprite;
    

    //called when the program first starts
    void Start()
    {
        //make it so the cursor isn't visible, it'll be on the crosshair anyways
        Cursor.visible = false;
        //make the player health equal to the max health 
        hp = maxHp;
        //reset the score and possesion of treasure
        hasTreasure = false;
        //spawn a treasure trigger
        Instantiate(treasureSpawn);
    }

    // Update is called once per frame
    void Update()
    {
        //if the player is alive run their logic
        if(hp > 0 && GameManage.GetState() == 1 && !GameManage.isOver)
        {
            //run player movement
            PlayerMove();

            //set the players aim (crosshair position)
            Aim();

            //check when the player clicks the left mouse button
            if (Input.GetMouseButtonDown(0))
            {
                //if they have, fire a bullet 
                FireBullet();
            }

            //if the prompt for getting points is still visible slowly fade it 
            if(pointPrompt.color.a > 0.0f)
            {
                Color promptCol = pointPrompt.color;
                promptCol.a -= Time.deltaTime / 5;
                pointPrompt.color = promptCol;
            }
        }
        //else, end the game
        else if (GameManage.GetState() == 1 && !GameManage.isOver)
        {
            //set the game to be over
            FindObjectOfType<GameManage>().GameOver();
            //and play the game over sound effect
            gameOverSFX.Play();
            //stop playing main soundtrack
            cam.GetComponent<AudioSource>().Stop();
            //stop playing animation
            animator.SetBool("GameOver", true);
        }
    }

    //function to check for player movement input and move the player
    private void PlayerMove()
    {
        //reset velocity to zero
        velocity = Vector3.zero;

        //if the player is not being knocked back allow them to control their movement
        if(knockBackTimeRemaining <= 0.0f)
        {
            //check if the player has given input to move the player
            //is the player moving up (w or uparrow)
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                //add positive movement in the y direction
                velocity += new Vector3(0.0f, 1.0f, 0.0f);
            }
            //is the player moving down (s or down arrow)
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                //add negative movement in the y direction
                velocity -= new Vector3(0.0f, 1.0f, 0.0f);
            }
            //is the player moving left (a or right arrow)
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.RightArrow))
            {
                //add positive movement in the x direction
                velocity -= new Vector3(1.0f, 0.0f, 0.0f);
            }
            //is the player moving right (d or left arrow)
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftArrow))
            {
                velocity += new Vector3(1.0f, 0.0f, 0.0f);
            }
        }
        else
        {
            //otherwise knock them away from the enemy that just hit them
            velocity = knockBackDir * 2;
            //and decrement the time remaining on the knockback
            knockBackTimeRemaining -= Time.deltaTime;
        }

        //move the player
        transform.Translate(velocity * speed * Time.deltaTime);
        //set the parameters of the animator controller
        animator.SetFloat("Xspeed", Mathf.Abs(velocity.x));
        animator.SetFloat("Yspeed", velocity.y);
        //flip the player's x 
        if (velocity.x < 0f)
            playerSprite.flipX = true;
        else if (velocity.x > 0f)
            playerSprite.flipX = false;
    }

    //function to set the crosshair position the player is aiming at
    private void Aim()
    {
        //player aiming and shooting
        //get the mouse position in world coordinates
        mousePos = cam.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
        crosshair.transform.position = mousePos;
    }

    //function to fire a bullet when the player hits the left mouse button
    private void FireBullet()
    {
        //create a new bullet object
        GameObject NewBullet = Instantiate(bulletPrefab, transform.position, gunTrans.rotation);
        //Calculate the direction it should be fired in
        Vector3 BulletDir = new Vector3(mousePos.x - transform.position.x, mousePos.y - transform.position.y, 0.0f);
        //and pass that direction to the script that moves it 
        NewBullet.GetComponent<BulletMovement>().onFire(BulletDir);
        //delete the bullet after a second
        Destroy(NewBullet, 1);

        //set the rotation of the particle effect and gun sprite to be facing the same way as the bullet
        float particleRot = Vector2.Angle(new Vector2(0.0f, -1.0f), BulletDir);
        if (BulletDir.x > 0) particleRot *= -1;
        var particleShape = fireParticles.GetComponent<ParticleSystem>().shape;
        particleShape.rotation = new Vector3(0, particleRot, 0);
        //rotate the gun sprite
        if (BulletDir.x > 0)
        {
            //make sure the x is not flipped 
            gunSprite.GetComponent<SpriteRenderer>().flipX = false;
            gunSprite.transform.rotation = Quaternion.Euler(0.0f, 0.0f, Vector2.Angle(new Vector2(0.0f, -1.0f), BulletDir) - 90.0f); 
        }
        else
        {
            //flip the x 
            gunSprite.GetComponent<SpriteRenderer>().flipX = true;
            gunSprite.transform.rotation = Quaternion.Euler(0.0f, 0.0f, Vector2.Angle(new Vector2(0.0f, 1.0f), BulletDir) - 90.0f);
        }
        fireParticles.transform.position = gunSprite.transform.position;
        //play the particle effect
        fireParticles.GetComponent<ParticleSystem>().Play();
        //trigger a small short screenshake
        cam.GetComponent<ScreenShake>().TriggerShake(0.2f, 0.045f);

        //play the sound effect
        gunShotSFX.Play();
    }

    //calls when a trigger collision begins
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(GameManage.GetState() == 1 && !GameManage.isOver)
        {
            //check if it's an enemy that the player has collided with
            if (collider.tag == "Enemy")
            {
                //decrement player health
                hp--;
                //play the damage sound effect
                playerDmgSFX.Play();
                //Shake the screen
                cam.GetComponent<ScreenShake>().TriggerShake(0.2f, 0.100f);
                //update health bar
                bar.SetSize(((float)hp / (float)maxHp));

                //make the player and enemy bounce away from each other 
                GameObject enemyObj = GameObject.Find(collider.name);
                //calculate the direction from the enemy to the player, and set the player to be knocked back in that direction
                Vector3 dir = transform.position - enemyObj.transform.position;
                dir.Normalize();
                knockBackDir = dir;
                knockBackTimeRemaining = 0.25f;
                //knock the enemy back
                enemyObj.GetComponent<EnemyBehaviour>().Knockback();
            }
            //check if it's the ship and that the player has treasure to drop off
            if(collider.tag == "Ship" && hasTreasure)
            {
                //if they do, then remove the treasure from their possesion
                hasTreasure = false;
                //increase their score
                ScoreScript.score++;
                //play the score increase particle system
                //create a copy of the particle system at the score display position
                GameObject particleEffect = Instantiate(scoreIncreaseParticleSystem, cam.transform.position + new Vector3(3.1f, 1.3f, 0f), Quaternion.Euler(0.0f, 0.0f, 0.0f));
                //and play it
                particleEffect.GetComponent<ParticleSystem>().Play();
                //create a new trigger for the next treasure location
                Instantiate(treasureSpawn);
                //play the sound effect
                scoreUpSFX.Play();
                //restore player hp
                hp = maxHp;
                //update the health bar
                bar.SetSize(((float)hp / (float)maxHp));
            }
        }
    }

    //called when the player leaves a trigger box
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(GameManage.GetState() == 1 && !GameManage.isOver)
        {
            //check to see if it's the treasure spot the player is colliding with
            if (collision.tag == "TreasureSpot")
            {
                //if it is, check to see if the player is digging (right click)
                if (Input.GetMouseButtonDown(1))
                {
                    //set it so the player now has treasure in their possession
                    hasTreasure = true;
                    //deletes the trigger
                    Destroy(FindObjectOfType<TreasureSpotBehaviour>().gameObject);
                    //play the particle effect for finding treasure 
                    //create a copy of the particle system at the player's position
                    GameObject particleEffect = Instantiate(treasureFoundParticleSystem, transform.position, Quaternion.Euler(0.0f, 0.0f, 0.0f));
                    //and play it
                    particleEffect.GetComponent<ParticleSystem>().Play();
                    //and play the sound effect
                    treasureFoundSFX.Play();

                    //if the player doesn't have any points yet, tell them how to get points
                    if(ScoreScript.score == 0)
                    {
                        Color promptCol = pointPrompt.color;
                        promptCol.a = 1.0f;
                        pointPrompt.color = promptCol;
                    }
                }
            }
        }
    }
}
