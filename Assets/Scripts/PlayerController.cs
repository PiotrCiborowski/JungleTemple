using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;

    bool isGrounded = true;
    bool isForward = true;

    [SerializeField] Transform groundCheck;
    [SerializeField] private float runSpeed = 2.5f;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (animator.GetBool("canMove"))
        {
            if (Input.GetKey("right"))
            {
                if (!isForward)
                {
                    animator.SetTrigger("testTurn");
                }
                else
                {
                    rb.velocity = new Vector2(runSpeed, rb.velocity.y);

                    if (isGrounded)
                        animator.SetBool("testRunning", true);

                    //spriteRenderer.flipX = false;
                }
            }
            else if (Input.GetKey("left"))
            {
                if (isForward)
                {
                    animator.SetTrigger("testTurn");
                }
                else
                {
                    rb.velocity = new Vector2(-runSpeed, rb.velocity.y);

                    if (isGrounded)
                        animator.SetBool("testRunning", true);

                    //spriteRenderer.flipX = true;
                }
            }
            else
            {
                if (isGrounded)
                {
                    animator.SetBool("testRunning", false);
                }
            }
        }
    }

    void changeForward()
    {
        animator.ResetTrigger("testTurn");
        isForward = !isForward;
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    void changeMove()
    {
        animator.SetBool("canMove", !animator.GetBool("canMove"));
    }
}
