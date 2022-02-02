using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flagpole : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        MarioController player = other.GetComponent<MarioController>();
        if(player != null)
        {
            player.endLevel();
        }
    }
}
