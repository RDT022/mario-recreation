using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class brickBlock : MonoBehaviour
{
    public AudioClip bumpedClip;
    public AudioClip brokenClip;

    Vector3 startingPos;
    Vector3 bumpedPos;
    Animator animator;
    bool bumpMove = false;
    bool bumpBack = false;

    void Start()
    {
        startingPos = new Vector3(transform.position.x, transform.position.y, 0);
        bumpedPos = new Vector3(startingPos.x, startingPos.y + 0.5f, 0);
        animator = GetComponent<Animator>();
    }
    void FixedUpdate()
    {
        if(bumpMove)
        {
            transform.position = Vector3.Lerp(transform.position, bumpedPos, 0.1f);
        }
        if(bumpBack)
        {
            transform.position = Vector3.Lerp(transform.position, startingPos, 0.2f);
        }
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        MarioController player = other.gameObject.GetComponent<MarioController>();
        if(player != null)
        {
            if(player.hitbox.position.y < transform.position.y && Mathf.Abs(transform.position.x - player.hitbox.position.x) < 0.7f)
            {
                if(player.powerUpState == 0)
                {
                    player.PlaySound(bumpedClip);
                    bumpMove = true;
                    Invoke("moveBack", 0.15f);
                }
                else
                {
                    player.PlaySound(brokenClip);
                    Destroy(gameObject);
                }
                
            }
        }
    }
    void moveBack()
    {
        bumpMove = false;
        bumpBack = true;
    }
}
