using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    //the direction the bullet should be travelling
    private Vector3 dir;
    //the speed the bullet should be moving at 
    public float speed = 10;
    //gameobject with the particle system for when an enemy is hit with a shot
    public GameObject shotParticles;

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

        //check if the bullet has hit an enemy (if it's within 1.0f units of an enemy's position)
        EnemyBehaviour enemy = EnemyBehaviour.GetClosestEnemyWithinRange(transform.position, 0.1f);
        if (enemy != null)
        {
            //if it did hit an enemy, damage that enemy
            enemy.Damage();
            //create a copy of the particle system at the bullet's position
            GameObject particleEffect = Instantiate(shotParticles, transform.position, Quaternion.Euler(0.0f, 0.0f, 0.0f));
            //and play it
            particleEffect.GetComponent<ParticleSystem>().Play();
            //and destroy the bullet
            Destroy(gameObject);
        }
    }
}
