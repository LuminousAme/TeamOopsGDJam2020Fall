using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    //the direction the bullet should be travelling
    private Vector3 dir;
    //the speed the bullet should be moving at 
    public float speed = 10;

    //when the bullet is first fired
    public void onFire(Vector3 shootDir)
    {
        //copy the direction the bullet was fired in 
        dir = shootDir.normalized;
    }

    // Update is called once per frame
    void Update()
    {
        //move the bullet in that direction
        transform.position += dir * speed * Time.deltaTime;        
    }
}
