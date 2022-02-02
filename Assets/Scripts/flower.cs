using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flower : MonoBehaviour
{
    Rigidbody2D rb;
    float direction;
    Collider2D c;
    public LayerMask groundLayer;
    Vector2 marioPos;
    MarioController player;
    bool spawnedIn;
    Vector3 finishedPos;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        c = GetComponent<Collider2D>();
        GameObject marioObject = GameObject.FindWithTag("MarioController");
        if (marioObject != null)
        {
           player = marioObject.GetComponent<MarioController>();        
        }
        finishedPos = new Vector3 (rb.position.x, rb.position.y + 1, 0);
        Invoke("finishedAppearing", 1);
    }

    // Update is called once per frame
    void Update()
    {
        if(!spawnedIn)
        {
            rb.gravityScale = 0;
            c.isTrigger = true;
            transform.position = Vector3.Lerp(transform.position, finishedPos, 0.025f);
        }
        else
        {
            rb.gravityScale = 4;
            c.isTrigger = false;
        }
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        player = other.gameObject.GetComponent<MarioController>();
        if(player != null && player.powerUpState < 2)
        {
            player.powerUp();
            Destroy(gameObject);
        }
    }

    void finishedAppearing()
    {
        spawnedIn = true;
    }
}
