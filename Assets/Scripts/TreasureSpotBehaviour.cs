using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureSpotBehaviour : MonoBehaviour
{
    //list of points from which to use catmull-rom interpolation and random number generation in order to determine where to spawn the treasure
    private static List<Vector3> pathCurve1 = new List<Vector3> {new Vector3(7.9f, -4.6f, -6f), 
                                                                 new Vector3(6.4f, -6.2f, -6f),
                                                                 new Vector3(3.4f, -6.7f, -6f),
                                                                 new Vector3(-2.2f, -5.5f, -6f),
                                                                 new Vector3(-4.9f, -7.6f, -6f),
                                                                 new Vector3(-7.7f, -6.7f, -6f)}; //bottom of island

    private static List<Vector3> pathCurve2 = new List<Vector3> {new Vector3(-8.6f, -3.5f, -6f),
                                                                 new Vector3(-6.8f, -4.1f, -6f),
                                                                 new Vector3(-2.8f, -2.4f, -6f),
                                                                 new Vector3(0.4f, 0.4f, -6f),
                                                                 new Vector3(3f, -1.5f, -6f),
                                                                 new Vector3(5.4f, -3.5f, -6f)}; //middle of island

    private static List<Vector3> pathCurve3 = new List<Vector3> {new Vector3(6.7f, 0f, -6f),
                                                                 new Vector3(6.2f, 1.5f, -6f),
                                                                 new Vector3(7.1f, 3.4f, -6f),
                                                                 new Vector3(4.8f, 4f, -6f),
                                                                 new Vector3(2.2f, 3.3f, -6f),
                                                                 new Vector3(0f, 2.5f, -6f)}; //beach area at top of island

    private static List<Vector3> pathCurve4 = new List<Vector3> {new Vector3(-4.4f, 3.9f, -6f),
                                                                 new Vector3(-6.1f, 3.1f, -6f),
                                                                 new Vector3(-7.7f, 2.4f, -6f),
                                                                 new Vector3(-8.4f, 1.2f, -6f),
                                                                 new Vector3(-8.9f, -0.5f, -6f),
                                                                 new Vector3(-8f, -1.8f, -6f)}; //far edge of island


    // Start is called before the first frame update
    void Start()
    {
        //when the object is first created, set the transform to a random point along a random line above
        //first pick a random path
        int path = Random.Range(0, 4);
        //next pick a random point (from 0 to 2) to start at 
        int startPoint = Random.Range(0, 3);
        //finally pick a random spot (in percentage, 0.0 to 1.0) along the line segement
        float t = Random.Range(0.0f, 1.0f);

        //check if it picked the path along the bottom of the island
        if(path == 0)
        {
            //if it did, then set the position to the point it generated along that curve
            transform.position = Catmull(pathCurve1[startPoint], pathCurve1[startPoint+1], pathCurve1[startPoint+2], pathCurve1[startPoint+3], t);
        }
        //if not check if it picked the path along the middle on the island
        else if (path == 1)
        {
            //if it did, then set the position to the point it generated along that curve
            transform.position = Catmull(pathCurve2[startPoint], pathCurve2[startPoint + 1], pathCurve2[startPoint + 2], pathCurve2[startPoint + 3], t);
        }
        //if not check if it picked the path along the beach part near the top
        else if (path == 2)
        {
            //if it did, then set the position to the point it generated along that curve
            transform.position = Catmull(pathCurve3[startPoint], pathCurve3[startPoint + 1], pathCurve3[startPoint + 2], pathCurve3[startPoint + 3], t);
        }
        //if not, it must have picked the path along the far edge of the island
        else
        {
            //so set the position to the point it generated along that curve
            transform.position = Catmull(pathCurve4[startPoint], pathCurve4[startPoint + 1], pathCurve4[startPoint + 2], pathCurve4[startPoint + 3], t);
        }

        //set the tag to treasure spot
        //set this gameobject tag to enemy 
        gameObject.tag = "TreasureSpot";
        //set this gameobject's layer to minimap (so the x marking the treasure can only be seen on the minimap)
        gameObject.layer = 8;

        //make the ground sprite for it appear at the same location as the trigger
        GameObject.Find("BuriedTreasure").GetComponent<Transform>().position = transform.position;
    }

    //function to calculate a point on a catmull rom curve, used to position the spawning of treasure
    public static Vector3 Catmull(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        return 0.5f * (2 * p1 + t * (-p0 + p2)
           + t * t * (2 * p0 - 5 * p1 + 4 * p2 - p3)
           + t * t * t * (-p0 + 3 * p1 - 3 * p2 + p3));
    }
}
