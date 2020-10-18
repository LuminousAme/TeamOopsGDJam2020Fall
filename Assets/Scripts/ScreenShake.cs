using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    //how long should the screenshake effect last 
    private float ShakeDur = 0.0f;

    //magnitude for how much it should shake 
    private float shakeMag = 0.5f;

    //the position of the transform when the shake beings
    private Vector3 initPos;

    private void Start()
    {
        initPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManage.GetState() == 1)
        {
            if (ShakeDur > 0.0f)
            {
                transform.localPosition = initPos + Random.insideUnitSphere * shakeMag;

                ShakeDur -= Time.deltaTime;
            }
            else
            {
                transform.localPosition = new Vector3(0f, 0f, -10f);
                ShakeDur = 0;
            }
        }
    }

    public void TriggerShake(float duration, float magnintude)
    {
        ShakeDur = duration;
        shakeMag = magnintude;
        initPos = transform.localPosition;
    }
}
