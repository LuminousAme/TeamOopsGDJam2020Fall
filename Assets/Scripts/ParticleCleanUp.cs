using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCleanUp : MonoBehaviour
{
    //how long the particle system should exist
    public float lifeTime = 5.0f;

    //how long the particle system has existed
    private float timeAlive = 0.0f;

    //called before first update
    void Start()
    {
        gameObject.layer = 9;    
    }

    // Update is called once per frame
    void Update()
    {
        //add to how long this particle system has existed
        timeAlive += Time.deltaTime;
        //if it is over it's lifetime, destroy it, we need this because we instantiat a prefab particle system, and don't want it's clones wasting memeory
        if (timeAlive >= lifeTime)
            Destroy(gameObject);
    }
}
