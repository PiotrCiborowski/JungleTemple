using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;

    bool isGrounded = true;
    bool isRunning = false;
    bool isCrouching = false;
    bool isForward = true;

    [SerializeField] Transform groundCheck;
    [SerializeField] float runSpeed = 2.5f;
    [SerializeField] float crouchSpeed = 1f;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Vector2 raycastTest = new Vector2(0, -0.25f);
        Debug.DrawRay(groundCheck.position, raycastTest, Color.red);

        if (animator.GetBool("canMove"))                        //try to leave inputs here and move behaviour to fixedupdate
        {
            if (Input.GetKey("right") && !Input.GetKey("left shift"))
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
            else if (Input.GetKey("left") && !Input.GetKey("left shift"))
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
            else if (Input.GetKey("down") && !isRunning)
            {
                animator.SetBool("Crouch", true);
                isCrouching = true;
            }
            else if (Input.GetKey("left shift") && !isRunning)
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
                if (isGrounded)
                {
                    animator.SetBool("Running", false);
                    animator.SetBool("Crouch", false);
                    animator.ResetTrigger("Step");
                    animator.ResetTrigger("Jump");
                    isRunning = false;
                    isCrouching = false;
                }
            }
        }
    }

    void ChangeForward()
    {
        animator.ResetTrigger("Turn");
        isForward = !isForward;
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    void ChangeMove()
    {
        animator.SetBool("canMove", !animator.GetBool("canMove"));
    }

    void StopMove()
    {
        rb.velocity = new Vector2(0.5f, rb.velocity.y);
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
        rb.velocity = new Vector2(0.075f, 2.5f);
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
