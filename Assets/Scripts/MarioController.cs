using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MarioController : MonoBehaviour
{
    public AudioSource sfxSource;
    public AudioSource musicSource;
    
    public AudioClip upgrade;
    public AudioClip downgrade;
    public AudioClip jump;
    public AudioClip extraLife;
    public AudioClip death;
    public AudioClip endFanfare;

    Animator animator;

    public Rigidbody2D hitbox;
    float walkSpeed = 5.0f; 
    float runSpeed = 9.0f;
    float horizontal;
    float targetSpeed;

    float groundDrag;
    float gravity = 1;
    float gravMultiplier = 4.0f;

    float inputDelay = 0.25f;
    float jumpTimer;
    float jumpLength;
    bool changingDirections;

    public static int lives;
    public static bool isOnTitle;
    Collider2D c;
    bool disableInput = false;

    public bool left;
    public bool right;
    bool beatLevel;
    bool dead;
    public static bool checkpointReached;

    Vector2 hitboxOffset = new Vector3(0.3f, 0);

    public LayerMask groundLayer;
    public bool isOnGround = false;
    bool jumpSounded;
    public static int coinCount;
    public float powerUpState;

    public bool isInvincible;
    Vector3 savedMomentum;

    // Start is called before the first frame update
    void Start()
    {
        hitbox = GetComponent<Rigidbody2D>();
        c = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        if(checkpointReached == true)
        {
            hitbox.transform.position = new Vector3(75.5f,-4.5f,0);
        }
        if(powerUpState == 1)
        {
            animator.SetBool("Big", true);
        }
        if(powerUpState == 2)
        {
            animator.SetBool("Fire", true);
        }
        if(SceneManager.GetActiveScene().name == "Title")
        {
            hitbox.transform.position = new Vector3(-5f, -4.5f, 0);
            disableInput = true;
            lives = 3;
            isOnTitle = true;
            animator.SetFloat("Speed", -1);
            animator.SetBool("OnGround", true);
            animator.SetFloat("Direction", 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isOnTitle)
        {
            if(Input.GetKeyDown(KeyCode.Return))
            {
                SceneManager.LoadScene("1-1");
            }
        }
        if(hitbox.position.y < -7.5f && !dead)
        {
            deathAnim();
        }
        if(hitbox.transform.position.x > 75.5f && checkpointReached == false)
        {
            checkpointReached = true;
        }
        if(Input.GetKeyDown(KeyCode.X))
        {
            jumpTimer = Time.time + inputDelay;
            jumpLength = Time.time + 0.2f;
        }
        horizontal = Input.GetAxis("Horizontal");
        isOnGround = Physics2D.Raycast(hitbox.position + hitboxOffset, Vector2.down, 0.6f, groundLayer) || Physics2D.Raycast(hitbox.position - hitboxOffset, Vector2.down, 0.6f, groundLayer);
        if(beatLevel)
        {
            if(!isOnGround)
            {
                animator.SetBool("Flagpole", true);
            }
            else if(isOnGround && hitbox.position.x < 196.5f)
            {
                animator.SetBool("Flagpole", false);
                hitbox.velocity = new Vector2(1.0f, 0);
                animator.SetBool("OnGround", true);
                animator.SetFloat("Direction", 1);
                animator.SetFloat("Speed", 1);
            }
            else
            {
                SceneManager.LoadScene("Title");
            }
        }
    }
    void FixedUpdate()
    {
        if(disableInput == false)
        {
            if(Input.GetKey(KeyCode.Z))
            {
                targetSpeed = runSpeed;
                animator.SetBool("Running", true);
            }
            else
            {
                targetSpeed = walkSpeed;
                animator.SetBool("Running", false);
            }
            
            hitbox.AddForce(Vector2.right * horizontal * targetSpeed);

            changingDirections = (horizontal > 0 && hitbox.velocity.x < 0) || (horizontal < 0 && hitbox.velocity.x > 0);

            if(Mathf.Abs(hitbox.velocity.x) > targetSpeed)
            {
                hitbox.velocity = new Vector2(horizontal * targetSpeed, hitbox.velocity.y);
            }
            if(isOnGround)
            {
                jumpSounded = false;
                animator.SetBool("OnGround", true);
                if(Mathf.Sign(hitbox.velocity.x) > 0)
                {
                    animator.SetFloat("Direction", 1);
                    right = true;
                    left = false;
                }
                else
                {
                    animator.SetFloat("Direction", 0);
                    left = true;
                    right = false;
                }
                if(Mathf.Abs(hitbox.velocity.x) > 0.1f)
                {
                    animator.SetFloat("Speed", 1);
                }
                else
                {
                    animator.SetFloat("Speed", -1);
                }
                if(Mathf.Abs(horizontal) < 0.04f || changingDirections)
                {
                    if(changingDirections)
                    {
                        animator.SetBool("Skidding", true);
                        Invoke("stopSkid", 0.3f);
                    }
                    else
                    {
                        animator.SetBool("Skidding", false);
                    }
                    if(targetSpeed == walkSpeed)
                    {
                        groundDrag = 10.0f;
                    }
                    else
                    {
                        groundDrag = 6.0f;
                    }
                }
                else
                {
                    groundDrag = 0.0f;
                }
                hitbox.gravityScale = 0;
                hitbox.drag = groundDrag;
            }
            else
            {
                animator.SetBool("OnGround", false);
                hitbox.gravityScale = gravity;

                if(hitbox.velocity.y < 0)
                {
                    if(Physics2D.Raycast(hitbox.position, Vector2.down, 1.5f, groundLayer))
                    {
                        hitbox.gravityScale = 0;
                        hitbox.transform.position -= new Vector3(0, 0.15f, 0);
                    }
                    else
                    {
                        hitbox.gravityScale = gravity * gravMultiplier;
                    }
                }
                else if((hitbox.velocity.y > 0 && !Input.GetKey(KeyCode.X)) || (jumpLength < Time.time))
                {
                    hitbox.gravityScale = gravity * (gravMultiplier - 0.5f);
                }
                else if(!isOnGround && hitbox.velocity.y == 0)
                {
                    hitbox.gravityScale = gravity * gravMultiplier;
                }
                else
                {
                    hitbox.gravityScale = 0;
                    if(!jumpSounded)
                    {
                        PlaySound(jump);
                        jumpSounded = true;
                    }
                }
            }
            if(jumpTimer > Time.time && isOnGround)
            {
                hitbox.velocity = new Vector2(hitbox.velocity.x, 0);
                
                if(hitbox.velocity.x < 0.01 && horizontal == 0)
                {
                    hitbox.AddForce(Vector2.up * 30.0f, ForceMode2D.Impulse);
                }
                else if(targetSpeed == walkSpeed)
                {
                    hitbox.AddForce(Vector2.up * (10.0f + (Mathf.Abs(targetSpeed) / 3) + 0.2f), ForceMode2D.Impulse);
                }
                else
                {
                    hitbox.AddForce(Vector2.up * (10.0f + (Mathf.Abs(targetSpeed)/3)), ForceMode2D.Impulse);
                }
                jumpTimer = 0;
            }
        }
    }
    public void takeDamage()
    {
        if(powerUpState == 0)
        {
            deathAnim();
        }
        else
        {
            disableInput = true;
            PlaySound(downgrade);
            powerUpState = 0;
            Invoke("moveAgain", 1.5f);
            savedMomentum = hitbox.velocity;
            hitbox.velocity = new Vector3 (0,0,0);
            hitbox.gravityScale = 0;
            isInvincible = true;
            GetComponent<BoxCollider2D>().size = new Vector2(0.65f,0.85f);
            GetComponent<BoxCollider2D>().offset = new Vector2(0,-0.1f);
            animator.SetBool("Big", false);
            animator.SetBool("Fire", false);
        }
    }
    public void deathAnim()
    {
        disableInput = true;
        dead = true;
        musicSource.Stop();
        animator.SetBool("Dead", true);
        PlaySound(death);
        hitbox.velocity = new Vector2(0,0);
        hitbox.AddForce(Vector2.up * 50.0f, ForceMode2D.Impulse);
        hitbox.gravityScale = 5;
        c.isTrigger = true;
        lives--;
        Invoke("reloadLevel", 3.6f);
    }
    public void enemyBounce()
    {
        hitbox.gravityScale = 0;
        hitbox.velocity = new Vector2(hitbox.velocity.x, 0);
        hitbox.AddForce(Vector2.up * 10.0f, ForceMode2D.Impulse);
        jumpSounded = true;
    }
    void reloadLevel()
    {
        if(lives < 0)
        {
            SceneManager.LoadScene("Title");
        }
        else
        {
            SceneManager.LoadScene("1-1");
        }
    }
    public void increaseCoin()
    {
        coinCount++;
    }
    public void PlaySound(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
    public void stopSkid()
    {
        changingDirections = false;
        animator.SetBool("Skidding", false);
    }
    void moveAgain()
    {
        disableInput = false;
        isInvincible = false;
        hitbox.velocity = savedMomentum;
    }
    public void powerUp()
    {
        disableInput = true;
        savedMomentum = hitbox.velocity;
        hitbox.velocity = new Vector3 (0,0,0);
        hitbox.gravityScale = 0;
        powerUpState++;
        isInvincible = true;
        Invoke("moveAgain", 1.5f);
        PlaySound(upgrade);
        GetComponent<BoxCollider2D>().size = new Vector2(0.8f,1.7f);
        GetComponent<BoxCollider2D>().offset = new Vector2(0,0.35f);
        if(powerUpState == 1)
        {
            animator.SetBool("Big", true);
        }
        else if(powerUpState == 2)
        {
            animator.SetBool("Fire", true);
        }
    }
    public void oneUp()
    {
        lives++;
        PlaySound(extraLife);
    }
    public void endLevel()
    {
        disableInput = true;
        hitbox.velocity = new Vector3(0,0,0);
        hitbox.gravityScale = 1;
        hitbox.drag = 0;
        musicSource.Stop();
        PlaySound(endFanfare);
        beatLevel = true;

    }
}
