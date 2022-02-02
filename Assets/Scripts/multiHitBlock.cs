using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class multiHitBlock : MonoBehaviour
{
    public AudioClip bumpedClip;

    public AudioClip collectedClip;

    bool collected = false;

    Vector3 startingPos;
    Vector3 bumpedPos;
    Animator animator;
    bool bumpMove = false;
    bool bumpBack = false;

    float initialHit;
    float hitTimer;
    float coinCount;
    bool firstHit;

    public GameObject coinParticle;

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
                player.PlaySound(bumpedClip);
                if(collected == false)
                {
                    if(!firstHit)
                    {
                        initialHit = Time.deltaTime;
                        hitTimer = initialHit + 10.0f;
                        firstHit = true;
                    }
                    player.increaseCoin();
                    player.PlaySound(collectedClip);
                    Instantiate(coinParticle, startingPos, Quaternion.identity);
                    bumpMove = true;
                    Invoke("moveBack", 0.15f);
                    if(coinCount == 9 || hitTimer < Time.deltaTime)
                    {
                        collected = true;
                        animator.SetBool("Empty", true);
                    }
                    coinCount++;
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
