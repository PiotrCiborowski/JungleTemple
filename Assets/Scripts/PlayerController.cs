using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    AudioSource audioSource;

    bool isGrounded = true;
    bool isRunning = false;
    bool isCrouching = false;
    bool isForward = true;

    float fallingMomentum;

    [SerializeField] Transform groundCheck;
    [SerializeField] float runSpeed = 2.5f;
    [SerializeField] float crouchSpeed = 1f;

    [SerializeField] AudioClip[] footsteps;
    [SerializeField] AudioClip fallHurt;
    [SerializeField] AudioClip fallDeath;

    [SerializeField] Image hurtFlash;
    [SerializeField] float hurtTime = 0.05f;

    public int heroHP = 3;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {

        Vector2 groundRaycast = new Vector2(0, -0.25f);

        RaycastHit2D feet = Physics2D.Raycast(groundCheck.position, Vector2.down, Mathf.Abs(groundRaycast.y), 1<<8);

        if (feet.collider == null)
        {
            isGrounded = false;
            animator.SetBool("Falling", true);

            //if ()

            if (isCrouching)
            {
                isCrouching = false;
                animator.Play("ToFalling");
            }
            else if (isRunning)
            {
                isRunning = false;
                fallingMomentum = rb.velocity.x;
                animator.Play("ToFalling");
            }
            else if (rb.gravityScale != 0 && animator.GetCurrentAnimatorStateInfo(0).IsName("StandingJump") || rb.gravityScale != 0 && animator.GetCurrentAnimatorStateInfo(0).IsName("RunningJump"))
            {
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.6f)
                    animator.Play("ToFalling");
            }

            rb.velocity = new Vector2(fallingMomentum, rb.velocity.y);
        }
        else if (LayerMask.LayerToName(feet.collider.gameObject.layer) == "Ground")
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Falling"))
            {
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 40)
                {
                    isGrounded = true;
                    animator.SetBool("Falling", false);
                    fallingMomentum = 0;

                    Footstep();
                }
                else if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 40 && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 50)
                {
                    heroHP -= 1;

                    hurtFlash.enabled = true;
                    Invoke("HurtFlash", hurtTime);

                    if (heroHP <= 0)
                    {
                        isGrounded = true;
                        spriteRenderer.flipX = false;
                        animator.SetTrigger("Die");
                        //animator.SetBool("Falling", false);
                        audioSource.pitch = 1;
                        audioSource.PlayOneShot(fallDeath);
                        fallingMomentum = 0;
                        CantMove();
                    }
                    else
                    {
                        isGrounded = true;
                        animator.SetBool("Falling", false);
                        audioSource.pitch = 1;
                        audioSource.PlayOneShot(fallHurt);
                        fallingMomentum = 0;
                        CantMove();

                        animator.SetBool("Crouch", true);
                        Invoke("FallHurt", 2);
                    }
                }
                else if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 50)
                {
                    heroHP = 0;

                    hurtFlash.enabled = true;
                    Invoke("HurtFlash", hurtTime);

                    isGrounded = true;
                    spriteRenderer.flipX = false;
                    animator.SetTrigger("Die");
                    //animator.SetBool("Falling", false);
                    audioSource.pitch = 1;
                    audioSource.PlayOneShot(fallDeath);
                    fallingMomentum = 0;
                    CantMove();
                }
            }
        }

        Debug.DrawRay(groundCheck.position, groundRaycast, Color.red);

        if (Input.GetKeyDown("r"))
        {
            rb.transform.position = new Vector2(6, 0);          //DEBUG
            animator.ResetTrigger("Die");
            animator.Play("Standing");
            CanMove();
        }

        if (animator.GetBool("canMove"))                        //try to leave inputs here and move behaviour to fixedupdate
        {
            if (Input.GetKey("right") && !Input.GetKey("left shift") && isGrounded)
            {
                if (!isCrouching && !animator.GetCurrentAnimatorStateInfo(0).IsName("CrouchMove") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Crouch"))
                {
                    if (!isForward)
                        animator.SetTrigger("Turn");
                    else
                    {
                        animator.SetBool("Running", true);
                        rb.velocity = new Vector2(runSpeed, rb.velocity.y);
                        isRunning = true;

                        if (Input.GetKey("down"))
                        {
                            isRunning = false;
                            animator.Play("ToCrouch");

                            animator.SetBool("Crouch", true);
                            isCrouching = true;
                        }
                        else if (Input.GetKeyDown("up"))
                        {
                            animator.SetTrigger("Jump");
                        }


                        //spriteRenderer.flipX = false;
                    }
                }
                else if (isCrouching)
                {
                    if (isForward)
                    {
                        animator.SetTrigger("Step");
                        rb.velocity = new Vector2(crouchSpeed, rb.velocity.y);

                        if (animator.GetCurrentAnimatorStateInfo(0).IsName("CrouchMove"))
                            animator.ResetTrigger("Step");

                        if (Input.GetKeyUp("down"))
                        {
                            animator.SetBool("Crouch", false);
                            isCrouching = false;
                        }
                    }
                }
            }
            else if (Input.GetKey("left") && !Input.GetKey("left shift") && isGrounded)
            {
                if (!isCrouching && !animator.GetCurrentAnimatorStateInfo(0).IsName("CrouchMove") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Crouch"))
                {
                    if (isForward)
                        animator.SetTrigger("Turn");
                    else
                    {
                        animator.SetBool("Running", true);
                        rb.velocity = new Vector2(-runSpeed, rb.velocity.y);
                        isRunning = true;

                        if (Input.GetKey("down"))
                        {
                            isRunning = false;
                            animator.Play("ToCrouch");

                            animator.SetBool("Crouch", true);
                            isCrouching = true;
                        }
                        else if (Input.GetKeyDown("up"))
                        {
                            animator.SetTrigger("Jump");
                        }

                        //spriteRenderer.flipX = true;
                    }
                }
                else if (isCrouching)
                {
                    if (!isForward)
                    {
                        animator.SetTrigger("Step");
                        rb.velocity = new Vector2(-crouchSpeed, rb.velocity.y);

                        if (animator.GetCurrentAnimatorStateInfo(0).IsName("CrouchMove"))
                            animator.ResetTrigger("Step");

                        if (Input.GetKeyUp("down"))
                        {
                            animator.SetBool("Crouch", false);
                            isCrouching = false;
                        }
                    }
                }
            }
            else if (Input.GetKeyDown("up") && !isRunning)
            {
                animator.SetTrigger("Jump");
            }
            else if (Input.GetKey("down") && isGrounded && !isRunning)
            {
                animator.SetBool("Crouch", true);
                isCrouching = true;
            }
            else if (Input.GetKey("left shift") && isGrounded && !isRunning)
            {
                if (Input.GetKeyDown("right"))
                {
                    if (isForward)
                    {
                        animator.SetTrigger("Step");
                        //rb.velocity = new Vector2(runSpeed, rb.velocity.y);
                    }
                    else
                        animator.SetTrigger("Turn");
                }
                else if (Input.GetKeyDown("left"))
                {
                    if (!isForward)
                    {
                        animator.SetTrigger("Step");
                        //rb.velocity = new Vector2(-runSpeed, rb.velocity.y);
                    }
                    else
                        animator.SetTrigger("Turn");
                }
            }
            else
            {
                animator.SetBool("Running", false);
                animator.SetBool("Crouch", false);
                animator.ResetTrigger("Step");
                animator.ResetTrigger("Jump");
                isRunning = false;
                isCrouching = false;

                if (isGrounded)
                    CanMove();
            }
        }
    }

    void Footstep()
    {
        audioSource.clip = footsteps[Random.Range(0, footsteps.Length)];
        audioSource.pitch = Random.Range(0.9f, 1.1f);

        audioSource.PlayOneShot(audioSource.clip);
    }

    void ChangeForward()
    {
        animator.ResetTrigger("Turn");
        isForward = !isForward;
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    void CantMove()
    {
        animator.SetBool("canMove", false);
    }

    void CanMove()
    {
        animator.SetBool("canMove", true);
    }

    void StopMove()
    {
        rb.velocity = new Vector2(0.5f, rb.velocity.y);
    }

    void HurtFlash()
    {
        hurtFlash.enabled = false;
    }

    void FallHurt()
    {
        animator.SetBool("Crouch", false);
        CanMove();
    }

    void CrouchGetUpMove()
    {
        if (isForward)
            rb.velocity = new Vector2(crouchSpeed, rb.velocity.y);
        else
            rb.velocity = new Vector2(-crouchSpeed, rb.velocity.y);
    }

    void StepMove()
    {
        if (isForward)
            rb.velocity = new Vector2(crouchSpeed * 1.5f, rb.velocity.y);
        else if (!isForward)
            rb.velocity = new Vector2(-crouchSpeed * 1.5f, rb.velocity.y);
    }

    void Jump()
    {
        if (rb.gravityScale == 1)
            rb.gravityScale = 0;
        else
            rb.gravityScale = 1;
    }

    void JumpUpMove()
    {
        if (isForward)
            rb.velocity = new Vector2(0.075f, 2.5f);
        else if (!isForward)
            rb.velocity = new Vector2(-0.075f, 2.5f);
    }

    void JumpUpCheckForward()
    {
        if (isForward)
        {
            if (Input.GetKey("right"))
            {
                animator.ResetTrigger("Jump");
                animator.Play("StandingJump");
            }
        }
        else if (!isForward)
        {
            if (Input.GetKey("left"))
            {
                animator.ResetTrigger("Jump");
                animator.Play("StandingJump");
            }
        }
    }

    void StandingJumpMove()
    {
        if (isForward)
            rb.velocity = new Vector2(4.5f, rb.velocity.y);
        else if (!isForward)
            rb.velocity = new Vector2(-4.5f, rb.velocity.y);
    }

    void RunningJumpMove()
    {
        if (isForward)
            rb.velocity = new Vector2(7.5f, rb.velocity.y);
        else if (!isForward)
            rb.velocity = new Vector2(-7.5f, rb.velocity.y);
    }

    void JumpStop()
    {
        if (isForward)
            rb.velocity = new Vector2(runSpeed, rb.velocity.y);
        else if (!isForward)
            rb.velocity = new Vector2(-runSpeed, rb.velocity.y);
    }
}
