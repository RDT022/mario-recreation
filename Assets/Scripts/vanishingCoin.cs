using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vanishingCoin : MonoBehaviour
{
    Vector3 startingPos;
    Vector3 launchedPos;

    bool reachedPeak;

    // Start is called before the first frame update
    void Start()
    {
        startingPos = new Vector3(transform.position.x, transform.position.y, 0);
        launchedPos = new Vector3(transform.position.x, transform.position.y + 1, 0);
        Invoke("vanish", 0.4f);
        Invoke("fall",0.2f);
    }

    void Update()
    {
        if(!reachedPeak)
        {
            transform.position = Vector3.Lerp(transform.position, launchedPos, 0.05f);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, startingPos, 0.05f);
        }
    }

    void vanish()
    {
        Destroy(gameObject);
    }

    void fall()
    {
        reachedPeak = true;
    }
}
