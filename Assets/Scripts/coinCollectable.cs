using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coinCollectable : MonoBehaviour
{
    public AudioClip collectedClip;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        MarioController player = other.GetComponent<MarioController>();

        if(player != null)
        {
            player.increaseCoin();
            Destroy(gameObject);
            player.PlaySound(collectedClip);
        }
    }
}
