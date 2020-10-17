﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //normalized vector representing the players direction
    private Vector3 velocity = Vector3.zero;
    //how fast the player should be moving
    public float speed = 1;


    //reference to our camera (needed to convert mouse coordinates to world space)
    public Camera cam; 
    //position of the mouse
    private Vector2 mousePos;
    //transform of the crosshair 
    public Transform crosshair;
    //the gameobject for the bullet prefab
    public GameObject bulletPrefab;
    //the transform for the gun, this is needed for the rotation
    public Transform gunTrans;


    //called when the program first starts
    void Start()
    {
        //make it so the cursor isn't visible, it'll be on the crosshair anyways
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        //run player movement
        PlayerMove();

        //set the players aim (crosshair position)
        Aim();

        //check when the player clicks the left mouse button
        if(Input.GetMouseButtonDown(0))
        {
            //if they have, fire a bullet 
            FireBullet();
        }
    }

    //function to check for player movement input and move the player
    private void PlayerMove()
    {
        //reset velocity to zero
        velocity = Vector3.zero;

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

        //move the player
        transform.Translate(velocity * speed * Time.deltaTime);
    }

    //function to set the crosshair position the player is aiming at
    private void Aim()
    {
        //player aiming and shooting
        //get the mouse position in world coordinates
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
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
    }
}
