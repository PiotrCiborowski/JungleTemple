using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerControllerCopy : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    AudioSource audioSource;

    bool isGrounded = true;
    bool isRunning = false;
    bool isCrouching = false;
    bool isForward = true;
    bool isJumping = false;
    bool isJumpingUp = false;
    bool isClimbing = false;
    bool isDescending = false;
    bool isDead = false;
    bool isWinning = false;

    public static bool almostPressButton = true;
    public static bool pressButton = true;

    float fallingMomentum;

    [SerializeField] Transform groundCheck;
    [SerializeField] Transform grabCheck;
    [SerializeField] Transform bodyCheck;
    [SerializeField] float runSpeed = 2.5f;
    [SerializeField] float crouchSpeed = 1f;

    [SerializeField] AudioClip[] footsteps;
    [SerializeField] AudioClip fallHurt;
    [SerializeField] AudioClip fallDeath;

    [SerializeField] Image hurtFlash;
    [SerializeField] float hurtTime = 0.05f;

    public static int heroHP = 3;

    float Pythagoras(float a, float b)
    {
        return Mathf.Sqrt(a * a + b * b);
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        fallingMomentum = runSpeed * 0.85f;
    }

    void Update()
    {
        spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.x - 0.9f) + Mathf.RoundToInt(transform.position.y - 0.9f);

        //-------------------RAYCASTS-------------------

        Vector2 groundRaycast = new Vector2(0, -0.25f);
        Vector2 groundCheckRaycastShort;
        Vector2 groundCheckRaycastLong;
        Vector2 wallAboveRaycast = new Vector2(0, 0.5f);
        Vector2 grabRaycast;
        Vector2 dropRaycast;
        Vector2 bodyRaycast;

        if (isForward)
        {
            groundCheckRaycastShort = new Vector2(1.1f, -0.25f);
            groundCheckRaycastLong = new Vector2(1.5f, -0.25f);
            grabRaycast = new Vector2(0.6f, 0.8f);
            dropRaycast = new Vector2(-0.5f, 0);
            bodyRaycast = new Vector2(0.3f, 0);
        }
        else
        {
            groundCheckRaycastShort = new Vector2(-1.1f, -0.25f);
            groundCheckRaycastLong = new Vector2(-1.5f, -0.25f);
            grabRaycast = new Vector2(-0.6f, 0.8f);
            dropRaycast = new Vector2(0.5f, 0);
            bodyRaycast = new Vector2(-0.3f, 0);
        }


        RaycastHit2D feet = Physics2D.Raycast(groundCheck.position, Vector2.down, Mathf.Abs(groundRaycast.y), 1 << 8);
        RaycastHit2D feetGroundShort = Physics2D.Raycast(groundCheck.position, groundCheckRaycastShort, Mathf.Abs(Pythagoras(groundCheckRaycastShort.x, groundCheckRaycastShort.y)), 1 << 8);
        RaycastHit2D feetGroundLong = Physics2D.Raycast(groundCheck.position, groundCheckRaycastLong, Mathf.Abs(Pythagoras(groundCheckRaycastLong.x, groundCheckRaycastLong.y)), 1 << 8);
        RaycastHit2D wallAboveCheck = Physics2D.Raycast(grabCheck.position, Vector2.up, Mathf.Abs(wallAboveRaycast.y), 1 << 9);
        RaycastHit2D body = Physics2D.Raycast(bodyCheck.position, bodyRaycast, Mathf.Abs(bodyRaycast.x), 1 << 9);
        RaycastHit2D exitCheck = Physics2D.Raycast(bodyCheck.position, bodyRaycast, Mathf.Abs(bodyRaycast.x));
        RaycastHit2D hands = Physics2D.Raycast(grabCheck.position, grabRaycast, Mathf.Abs(Pythagoras(grabRaycast.x, grabRaycast.y)), 1 << 10);
        RaycastHit2D handsDrop = Physics2D.Raycast(groundCheck.position, dropRaycast, Mathf.Abs(dropRaycast.x), 1 << 10);


        //-------------------GROUND CHECK-------------------

        if (feet.collider == null)
        {
            if (isJumping || isJumpingUp || isCrouching || isClimbing || isDescending || isWinning || animator.GetCurrentAnimatorStateInfo(0).IsName("StandingJump") || animator.GetCurrentAnimatorStateInfo(0).IsName("RunningJump"))
                ;
            else if (isRunning || !isRunning)
            {
                isGrounded = false;
                animator.SetBool("Falling", true);
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("ToFalling") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Falling"))
                    animator.Play("ToFalling");
            }
            else
            {
                isGrounded = false;
                animator.SetBool("Falling", true);
                animator.Play("ToFalling");
            }

            if (isForward && !isJumpingUp && !isClimbing && !isDescending && !isWinning && !animator.GetCurrentAnimatorStateInfo(0).IsName("StandingJump") && !animator.GetCurrentAnimatorStateInfo(0).IsName("RunningJump"))
            {
                if (body.collider == null)
                    rb.velocity = new Vector2(fallingMomentum, rb.velocity.y);
                else
                    rb.velocity = new Vector2(0, rb.velocity.y);
            }
            else if (!isForward && !isJumpingUp && !isClimbing && !isDescending && !isWinning && !animator.GetCurrentAnimatorStateInfo(0).IsName("StandingJump") && !animator.GetCurrentAnimatorStateInfo(0).IsName("RunningJump"))
            {
                if (body.collider == null)
                    rb.velocity = new Vector2(-fallingMomentum, rb.velocity.y);
                else
                    rb.velocity = new Vector2(0, rb.velocity.y);
            }
               
            /*isGrounded = false;
            animator.SetBool("Falling", true);

            if (isJumping)
            {
                isGrounded = true;
                animator.SetBool("Falling", false);
            }
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
            }*/
        }
        else if (LayerMask.LayerToName(feet.collider.gameObject.layer) == "Ground")
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Falling"))
            {
                Debug.Log("Time in air: " + animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 35)
                {
                    isGrounded = true;
                    animator.SetBool("Falling", false);
                    //fallingMomentum = 0;

                    Footstep();
                }
                else if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 35 && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 50)
                {
                    heroHP -= 1;

                    hurtFlash.enabled = true;
                    Invoke("HurtFlash", hurtTime);

                    if (heroHP <= 0)
                    {
                        isGrounded = true;
                        isDead = true;
                        spriteRenderer.flipX = false;
                        animator.SetTrigger("Die");
                        //animator.SetBool("Falling", false);
                        audioSource.pitch = 1;
                        audioSource.volume = 1;
                        audioSource.PlayOneShot(fallDeath);
                        //fallingMomentum = 0;
                        CantMove();
                    }
                    else
                    {
                        isGrounded = true;
                        animator.SetBool("Falling", false);
                        audioSource.pitch = 1;
                        audioSource.volume = 1;
                        audioSource.PlayOneShot(fallHurt);
                        //fallingMomentum = 0;
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
                    isDead = true;
                    spriteRenderer.flipX = false;
                    animator.SetTrigger("Die");
                    //animator.SetBool("Falling", false);
                    audioSource.pitch = 1;
                    audioSource.volume = 1;
                    audioSource.PlayOneShot(fallDeath);
                    //fallingMomentum = 0;
                    CantMove();
                }
            }
        }


        //-------------------GRAB CHECK-------------------

        if (hands.collider == null)
            ;
        else if (LayerMask.LayerToName(hands.collider.gameObject.layer) == "Grab")
            ;//Debug.Log("Works + " + hands.collider.gameObject.name);

        Debug.DrawRay(groundCheck.position, groundRaycast, Color.red);
        Debug.DrawRay(groundCheck.position, groundCheckRaycastShort, Color.blue);
        Debug.DrawRay(groundCheck.position, groundCheckRaycastLong, Color.red);
        Debug.DrawRay(grabCheck.position, wallAboveRaycast, Color.magenta);
        Debug.DrawRay(bodyCheck.position, bodyRaycast, Color.white);
        Debug.DrawRay(grabCheck.position, grabRaycast, Color.red);
        Debug.DrawRay(groundCheck.position, dropRaycast, Color.yellow);


        //-------------------MOVEMENT-------------------

        if (Input.GetKeyDown("r"))
        {
            /*if (isDead)
            {*/
                rb.transform.position = new Vector2(-14.5f, 2);          //DEBUG
                heroHP = 3;
                animator.ResetTrigger("Die");
                animator.Play("Standing");
                spriteRenderer.flipX = false;
                isForward = true;
                isJumping = false;
                isJumpingUp = false;
                isDead = false;
                rb.gravityScale = 1;
                CanMove();
            //}
        }

        if (Input.GetKeyDown("escape"))
            QuitGame();

        if (animator.GetBool("canMove"))                        //try to leave inputs here and move behaviour to fixedupdate
        {
            //-------------------RIGHT-------------------

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


            //-------------------LEFT-------------------

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


            //-------------------UP-------------------

            else if (Input.GetKeyDown("up") && !isRunning && !isClimbing)
            {
                if (exitCheck.collider == null || exitCheck.collider.tag != "Exit")
                {
                    if (wallAboveCheck.collider != null)
                        animator.SetTrigger("JumpWallAbove");
                    else
                    {
                        animator.SetTrigger("Jump");

                        if (hands.collider == null)
                            ;
                        else if (LayerMask.LayerToName(hands.collider.gameObject.layer) == "Grab" && !isClimbing)
                        {
                            //Debug.Log("Can grab");

                            if (isForward)
                                rb.position = new Vector2(hands.collider.gameObject.transform.position.x - 0.3f, rb.position.y);
                            else if (!isForward)
                                rb.position = new Vector2(hands.collider.gameObject.transform.position.x + 0.3f, rb.position.y);

                            isGrounded = false;
                            isClimbing = true;
                            animator.SetBool("Grip", true);
                        }
                    }
                }
                else if (exitCheck.collider.tag == "Exit" && !isCrouching)
                {
                    CantMove();
                    isWinning = true;
                    rb.gravityScale = 0;
                    spriteRenderer.flipX = false;
                    rb.transform.position = new Vector2(exitCheck.collider.transform.position.x, exitCheck.collider.transform.position.y - 0.3f);
                    animator.SetTrigger("Win");

                    foreach (Transform child in exitCheck.transform)
                    {
                        child.GetComponent<Renderer>().sortingLayerName = "Foreground";
                    }
                }    
            }

            else if (Input.GetKey("up") && isClimbing)
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("HangingWall") || animator.GetCurrentAnimatorStateInfo(0).IsName("Swinging") || animator.GetCurrentAnimatorStateInfo(0).IsName("SwingingSecond"))
                {
                    animator.SetTrigger("Climb");
                }
            }

            else if (!Input.GetKey("up") && !Input.GetKey("left shift") && isClimbing && !isDescending)
            {
                isClimbing = false;
                animator.SetBool("Grip", false);
            }

            /*else if (Input.GetKey("up") && isClimbing || Input.GetKey("left shift") && isClimbing)
            {
                animator.SetBool("Grip", true);
                //rb.gravityScale = 0;
                if (!Input.GetKey("up") && !Input.GetKey("left shift"))
                {
                    animator.SetBool("Grip", false);
                    animator.ResetTrigger("Jump");
                    isClimbing = false;
                    //rb.gravityScale = 1;
                }
            }*/


            //-------------------DOWN-------------------

            else if (Input.GetKey("down") && isGrounded && !isRunning)
            {
                if (handsDrop.collider == null)
                {
                    animator.SetBool("Crouch", true);
                    isCrouching = true;
                }
                else if (LayerMask.LayerToName(handsDrop.collider.gameObject.layer) == "Grab" && !animator.GetCurrentAnimatorStateInfo(0).IsName("Descending"))
                {
                    if (isForward)
                        rb.position = new Vector2(handsDrop.collider.gameObject.transform.position.x + 0.1f, rb.position.y);
                    else if (!isForward)
                        rb.position = new Vector2(handsDrop.collider.gameObject.transform.position.x - 0.15f, rb.position.y);

                    isGrounded = false;
                    isClimbing = true;
                    isDescending = true;
                    animator.SetTrigger("Descend");
                    animator.SetBool("Grip", true);
                }
            }


            //-------------------SHIFT-------------------

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
                animator.ResetTrigger("JumpWallAbove");
                isRunning = false;
                isCrouching = false;
                //isClimbing = false;

                if (isGrounded)
                    CanMove();
            }
        }

        if (isCrouching)
        {
            //GetComponent<CapsuleCollider2D>().size;
        }
    }

    void Footstep()
    {
        audioSource.clip = footsteps[Random.Range(0, footsteps.Length)];
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Step"))
        {
            audioSource.pitch = Random.Range(0.85f, 0.95f);
            audioSource.volume = 0.35f;
        }
        else
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.volume = 1;
        }

        audioSource.PlayOneShot(audioSource.clip);
    }

    void ChangeForward()
    {
        animator.ResetTrigger("Turn");
        isForward = !isForward;
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    void ChangeJump()
    {
        isJumpingUp = !isJumpingUp;
    }

    void CheckFallShort()
    {
        if (isForward)
        {
            if (Physics2D.Raycast(groundCheck.position, new Vector2(1.1f, -0.25f), Mathf.Abs(Pythagoras(1.1f, -0.25f)), 1 << 8).collider == null)
            {
                isGrounded = false;
                isJumping = false;
                rb.gravityScale = 1;
                animator.SetBool("Falling", true);
                animator.Play("ToFalling");
            }
            else
                ;
        }
        else if (!isForward)
        {
            if (Physics2D.Raycast(groundCheck.position, new Vector2(-1.1f, -0.25f), Mathf.Abs(Pythagoras(-1.1f, -0.25f)), 1 << 8).collider == null)
            {
                isGrounded = false;
                isJumping = false;
                rb.gravityScale = 1;
                animator.SetBool("Falling", true);
                animator.Play("ToFalling");
            }
            else
                ;
        }
    }

    void CheckFallLong()
    {
        if (isForward)
        {
            if (Physics2D.Raycast(groundCheck.position, new Vector2(1.5f, -0.25f), Mathf.Abs(Pythagoras(1.5f, -0.25f)), 1 << 8).collider == null)
            {
                isGrounded = false;
                isJumping = false;
                rb.gravityScale = 1;
                animator.SetBool("Falling", true);
                animator.Play("ToFalling");
            }
            else
                ;
        }
        else if (!isForward)
        {
            if (Physics2D.Raycast(groundCheck.position, new Vector2(-1.5f, -0.25f), Mathf.Abs(Pythagoras(-1.5f, -0.25f)), 1 << 8).collider == null)
            {
                isGrounded = false;
                isJumping = false;
                rb.gravityScale = 1;
                animator.SetBool("Falling", true);
                animator.Play("ToFalling");
            }
            else
                ;
        }
    }

    void CantMove()
    {
        animator.SetBool("canMove", false);
    }

    void CanMove()
    {
        animator.SetBool("canMove", true);
    }

    void GetGrounded()
    {
        isGrounded = true;
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
            rb.velocity = new Vector2(crouchSpeed * 1.25f, rb.velocity.y);
        else if (!isForward)
            rb.velocity = new Vector2(-crouchSpeed * 1.25f, rb.velocity.y);
    }

    void Jump()
    {
        isJumping = !isJumping;

        if (rb.gravityScale == 1)
            rb.gravityScale = 0;
        else
        {
            rb.gravityScale = 1;
            //CheckFall();
        }
    }

    void JumpUpMove()
    {
        if (isForward)
            rb.velocity = new Vector2(0.15f, 2.8f);
        else if (!isForward)
            rb.velocity = new Vector2(-0.15f, 2.8f);
    }

    void JumpUpMoveWallAbove()
    {
        if (isForward)
            rb.velocity = new Vector2(0.15f, 1.35f);
        else if (!isForward)
            rb.velocity = new Vector2(-0.15f, 1.35f);
    }

    void JumpUpCheckForward()
    {
        if (isForward)
        {
            if (Input.GetKey("right"))
            {
                animator.ResetTrigger("Jump");
                animator.ResetTrigger("JumpWallAbove");
                animator.Play("StandingJump");
            }
        }
        else if (!isForward)
        {
            if (Input.GetKey("left"))
            {
                animator.ResetTrigger("Jump");
                animator.ResetTrigger("JumpWallAbove");
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
            rb.velocity = new Vector2(9, rb.velocity.y);
        else if (!isForward)
            rb.velocity = new Vector2(-9, rb.velocity.y);
    }

    void JumpStop()
    {
        if (isForward)
            rb.velocity = new Vector2(runSpeed * 0.75f, rb.velocity.y);
        else if (!isForward)
            rb.velocity = new Vector2(-runSpeed * 0.75f, rb.velocity.y);
    }

    void JumpStopLong()
    {
        if (isForward)
            rb.velocity = new Vector2(runSpeed, rb.velocity.y);
        else if (!isForward)
            rb.velocity = new Vector2(-runSpeed, rb.velocity.y);
    }

    void HangingMoveToLedge()
    {
        if (isForward)
            rb.velocity = new Vector2(0.85f, rb.velocity.y);
        else if (!isForward)
            rb.velocity = new Vector2(-0.85f, rb.velocity.y);
    }

    void BeforeHanging()
    {
        rb.gravityScale = 0;
        rb.velocity = new Vector2(0, 0);
    }

    void ClimbingUpMove()
    {
        rb.velocity = new Vector2(0, 2.225f);
    }

    void ClimbingUpMoveForward()
    {
        if (isForward)
            rb.velocity = new Vector2(0.665f, 0);
        else if (!isForward)
            rb.velocity = new Vector2(-0.665f, 0);
    }

    void DescendingMoveBack()
    {
        if (isForward)
            rb.velocity = new Vector2(-0.665f, 0);
        else if (!isForward)
            rb.velocity = new Vector2(0.665f, 0);
    }

    void DescendingMove()
    {
        rb.velocity = new Vector2(0, -2.95f);
    }

    void StopDescending()
    {
        isDescending = false;
    }

    void AfterHanging()
    {
        rb.gravityScale = 1;
    }

    void FalseBothButton()
    {
        almostPressButton = false;
        pressButton = false;
    }

    void AlmostPressButton()
    {
        almostPressButton = true;
    }

    void PressButton()
    {
        pressButton = true;
    }

    void LevelFinish()
    {
        rb.velocity = new Vector2(0.16f, 0.36f);
    }

    void LevelFinishStop()
    {
        rb.velocity = new Vector2(0, 0);
        spriteRenderer.enabled = false;
        Invoke("NextLevel", 3);
    }

    void NextLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex < SceneManager.sceneCountInBuildSettings - 1)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            Application.Quit();
            Debug.Log("Quit");
        }
    }

    void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
}
