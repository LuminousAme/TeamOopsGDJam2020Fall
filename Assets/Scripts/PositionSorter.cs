using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionSorter : MonoBehaviour
{
    //renderer for our object
    private Renderer thisObjectsRenderer;
    //the base order in layer value, from which the rest will be subtracted
    public int orderBase = 10000;
    //offset so you can control how far back a sprite is before they appear behind another object
    public float yOffset;
    //bool to control wheter or not this is a moving object that's order in layer needs to be recacaluted every frame
    public bool movingObject = true;


    // Start is called before the first frame update
    void Start()
    {
        //set the renderer to the renderer on the script this is attached to
        thisObjectsRenderer = gameObject.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManage.GetState() == 1)
        {
            //calculate the order in layer value for the attached object based on y-value
            thisObjectsRenderer.sortingOrder = (int)(orderBase - transform.position.y * 10.0f - yOffset * 10.0f);
            //check if the object is one that moves
            if (!movingObject)
            {
                //if it is not, then this script destroys itself as the layer in order need only be caculated once for non-moving objects
                Destroy(this);
            }
        }
    }
}
