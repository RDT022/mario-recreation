using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class koopa : MonoBehaviour
{
    Rigidbody2D rb;
    private MarioController mario;
    float direction = -1;
    bool dead = false;
    Collider2D c;
    public LayerMask groundLayer;
    Vector2 marioPos;
    bool closeEnough;
    Animator animator;
    public AudioClip defeat;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GameObject marioObject = GameObject.FindWithTag("MarioController");
        if (marioObject != null)
        {
           mario = marioObject.GetComponent<MarioController>();        
        }
        c = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        animator.SetFloat("Direction", 0);
    }

    // Update is called once per frame
    void Update()
    {
        marioPos = mario.hitbox.position;
        if(Mathf.Abs(rb.position.x - marioPos.x) < 18 && closeEnough == false)
        {
            closeEnough = true;
        }
        if(dead == true)
        {
            c.isTrigger = true;
            rb.gravityScale = 0;
        }
        if(direction > 0)
        {
            animator.SetFloat("Direction", 1);
            if(Physics2D.Raycast(rb.position, Vector2.right, 0.55f, groundLayer))
            {
                direction = -1;
            }
        }
        if(direction < 0)
        {
            animator.SetFloat("Direction", 0);
            if(Physics2D.Raycast(rb.position, Vector2.left, 0.55f, groundLayer))
            {
                direction = 1;
            }
        }
    }

    void FixedUpdate()
    {
        if(dead == false && closeEnough == true)
        {
            rb.transform.position += new Vector3(0.025f * direction, 0); 
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        MarioController player = other.gameObject.GetComponent<MarioController>();
        gooma goomba = other.gameObject.GetComponent<gooma>();
        if(goomba != null)
        {
            direction = direction * -1;
        }
        if(other.gameObject.tag == "powerup")
        {
            Physics2D.IgnoreCollision(other.gameObject.GetComponent<Collider2D>(), c);
        }
        if(player != null)
        {
            if(player.hitbox.velocity.y < 0 && dead == false && player.isOnGround == false)
            {
                player.enemyBounce();
                dead = true;
                animator.SetBool("Dead", true);
                player.PlaySound(defeat);
                Invoke("die", 1);
            }
            else if(!player.isInvincible)
            {
                player.takeDamage();
            }
        }
    }
    void die()
    {
        Destroy(gameObject);
    }
}
