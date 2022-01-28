using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    AudioSource audioSource;

    Vector3 startingPoint;

    bool isGrounded = true;
    bool isRunning = false;
    bool isCrouching = false;
    bool isForward = true;
    bool isJumping = false;
    bool isClimbing = false;
    bool isDescending = false;

    public static bool isDead = false;

    bool isWinning = false;

    bool beforeDeath = true;
    bool afterDeath = false;

    float fallingMomentum;

    Vector2 beforeFall;
    Vector2 afterFall;

    [SerializeField] Transform groundCheck;
    [SerializeField] Transform grabCheck;
    [SerializeField] Transform bodyCheck;
    [SerializeField] float runSpeed = 2.5f;
    [SerializeField] float crouchSpeed = 1f;

    [SerializeField] AudioClip[] footsteps;
    [SerializeField] AudioClip fallHurt;
    [SerializeField] AudioClip fallDeath;
    [SerializeField] AudioClip sliceDeath;

    [SerializeField] Image hurtFlash;
    [SerializeField] float hurtTime = 0.05f;

    [SerializeField] TextMeshProUGUI deathText;

    public static int heroHP = 3;

    CapsuleCollider2D cap;
    float defaultValue;
    float defaultOffset;

    public RaycastHit2D feet;
    public RaycastHit2D feetLong;
    public RaycastHit2D feetGroundShort;
    public RaycastHit2D feetGroundLong;
    public RaycastHit2D wallAboveCheck;
    public RaycastHit2D body;
    public RaycastHit2D bodyTrap;
    public RaycastHit2D slowWalkCheck;

    [HideInInspector] public bool oneStep = false;

    public float defaultFall;
    public float shortFall;
    public float longFall;
    [HideInInspector] public float mainFall;

    [HideInInspector] public Vector3 gravity;

    float Pythagoras(float a, float b)
    {
        return Mathf.Sqrt(a * a + b * b);
    }

    public void ChomperKill(GameObject chomper)
    {
        if (!isDead)
        {
            heroHP = 0;

            hurtFlash.enabled = true;
            Invoke("HurtFlash", hurtTime);

            rb.position = new Vector2(chomper.transform.position.x + 0.2f, chomper.transform.position.y);

            SpriteRenderer half = GameObject.Find("Hero/SliceDeathHalf").GetComponent<SpriteRenderer>();

            chomper.GetComponent<SpriteRenderer>().sortingLayerName = "Main";

            if (isForward)
            {
                //rb.position = new Vector2(chomper.transform.position.x + 0.1f, chomper.transform.position.y);
                spriteRenderer.sortingLayerName = "GateInBack";
                half.enabled = true;
                half.sortingLayerName = "GateInFront";

            }
            else if (!isForward)
            {
                //rb.position = new Vector2(chomper.transform.position.x + 0.2f, chomper.transform.position.y);
                spriteRenderer.sortingLayerName = "GateInFront";
                half.enabled = true;
                half.flipX = true;
                half.sortingLayerName = "GateInBack";
            }

            isGrounded = true;
            isDead = true;
            //spriteRenderer.flipX = false;
            animator.SetTrigger("Die");
            animator.Play("SliceDeath");
            //animator.SetBool("Falling", false);
            audioSource.pitch = 1;
            audioSource.volume = 1;
            audioSource.PlayOneShot(sliceDeath);
            CantMove();
        }
    }

    void Start()
    {
        startingPoint = transform.position;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        fallingMomentum = runSpeed * 0.85f;

        isDead = false;
        heroHP = 3;

        cap = GetComponent<CapsuleCollider2D>();
        defaultValue = cap.size.y;
        defaultOffset = cap.offset.y;

        defaultFall -= 0.001f;
        /*shortFall = 0.05f;
        longFall = 0.08f;*/
        mainFall = defaultFall;

        /*float a = defaultFall;
        defaultFall = 0.123f;
        defaultFall = a;
        float b = shortFall;
        shortFall = 0.123f;
        shortFall = b;
        float c = longFall;
        longFall = 0.123f;
        longFall = c;*/
    }

    void Update()
    {
        /*if (animator.GetCurrentAnimatorStateInfo(0).IsName("Falling"))
            Debug.Log("Can Move: " + animator.GetBool("canMove"));*/
        //Debug.Log("isClimbing: " + isClimbing);
        //Debug.Log("isGrounded: " + isGrounded);
        //Debug.Log("Velocity: " + rb.velocity);
        spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.x - 0.9f) + Mathf.RoundToInt(transform.position.y - 0.9f);

        if (spriteRenderer.flipX && !animator.GetBool("isLeft"))
            animator.SetBool("isLeft", true);
        else if (!spriteRenderer.flipX && animator.GetBool("isLeft"))
            animator.SetBool("isLeft", false);

        if (isDead)
        {
            if (beforeDeath)
                Invoke("AfterDeath", 2);
            else if (Input.anyKeyDown && afterDeath)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

            beforeDeath = false;
        }

        //-------------------RAYCASTS-------------------

        Vector2 groundRaycast = new Vector2(0, -0.25f);
        Vector2 groundLongRaycast = new Vector2(0, -0.6f);
        Vector2 groundCheckRaycastShort;
        Vector2 groundCheckRaycastLong;
        Vector2 wallAboveRaycast = new Vector2(0, 0.5f);
        Vector2 grabRaycast;
        Vector2 dropRaycast;
        Vector2 bodyRaycast;
        Vector2 bodyTrapRaycast;
        Vector2 slowWalkRaycast;

        if (isForward)
        {
            groundCheckRaycastShort = new Vector2(1.4f, -0.25f);
            groundCheckRaycastLong = new Vector2(1.8f, -0.15f);
            grabRaycast = new Vector2(0.6f, 0.8f);
            dropRaycast = new Vector2(-0.5f, 0);
            bodyRaycast = new Vector2(0.3f, 0);
            bodyTrapRaycast = new Vector2(0.75f, 0);
            slowWalkRaycast = new Vector2(0.75f, -1f);
        }
        else
        {
            groundCheckRaycastShort = new Vector2(-1.1f, -0.25f);
            groundCheckRaycastLong = new Vector2(-1.5f, -0.25f);
            grabRaycast = new Vector2(-0.6f, 0.8f);
            dropRaycast = new Vector2(0.5f, 0);
            bodyRaycast = new Vector2(-0.3f, 0);
            bodyTrapRaycast = new Vector2(-0.75f, 0);
            slowWalkRaycast = new Vector2(-0.75f, -1f);
        }

        //layer  8 - Ground
        //layer  9 - Wall
        //layer 10 - Grab
        //layer 11 - Trap

        feet = Physics2D.Raycast(groundCheck.position, Vector2.down, Mathf.Abs(groundRaycast.y), 1 << 8);
        feetLong = Physics2D.Raycast(groundCheck.position, Vector2.down, Mathf.Abs(groundLongRaycast.y), 1 << 8);
        feetGroundShort = Physics2D.Raycast(groundCheck.position, groundCheckRaycastShort, Mathf.Abs(Pythagoras(groundCheckRaycastShort.x, groundCheckRaycastShort.y)), 1 << 8);
        feetGroundLong = Physics2D.Raycast(groundCheck.position, groundCheckRaycastLong, Mathf.Abs(Pythagoras(groundCheckRaycastLong.x, groundCheckRaycastLong.y)), 1 << 8);
        RaycastHit2D groundAboveCheck = Physics2D.Raycast(grabCheck.position, Vector2.up, Mathf.Abs(wallAboveRaycast.y), 1 << 8);
        wallAboveCheck = Physics2D.Raycast(grabCheck.position, Vector2.up, Mathf.Abs(wallAboveRaycast.y), 1 << 9);
        body = Physics2D.Raycast(bodyCheck.position, bodyRaycast, Mathf.Abs(bodyRaycast.x), 1 << 9);
        bodyTrap = Physics2D.Raycast(bodyCheck.position, bodyTrapRaycast, Mathf.Abs(bodyTrapRaycast.x), 1 << 11);
        RaycastHit2D exitCheck = Physics2D.Raycast(bodyCheck.position, bodyRaycast, Mathf.Abs(bodyRaycast.x));
        RaycastHit2D hands = Physics2D.Raycast(grabCheck.position, grabRaycast, Mathf.Abs(Pythagoras(grabRaycast.x, grabRaycast.y)), 1 << 10);
        RaycastHit2D handsVertical = Physics2D.Raycast(grabCheck.position, Vector2.up, Mathf.Abs(wallAboveRaycast.y), 1 << 10);
        RaycastHit2D handsDrop = Physics2D.Raycast(groundCheck.position, dropRaycast, Mathf.Abs(dropRaycast.x), 1 << 10);
        slowWalkCheck = Physics2D.Raycast(bodyCheck.position, slowWalkRaycast, Mathf.Abs(Pythagoras(slowWalkRaycast.x, slowWalkRaycast.y)), 1 << 8);


        //-------------------GROUND CHECK-------------------

        if (feet.collider == null)
        {
            if (isJumping || isCrouching || isClimbing || isDescending || isWinning || /*animator.GetCurrentAnimatorStateInfo(0).IsName("Standing") || animator.GetCurrentAnimatorStateInfo(0).IsName("StandingJump") || animator.GetCurrentAnimatorStateInfo(0).IsName("StandingJumpLeft") ||*/ animator.GetCurrentAnimatorStateInfo(0).IsName("RunningJump") || animator.GetCurrentAnimatorStateInfo(0).IsName("RunningJumpLeft") || animator.GetCurrentAnimatorStateInfo(0).IsName("JumpUp") || animator.GetCurrentAnimatorStateInfo(0).IsName("JumpUpLeft") || animator.GetCurrentAnimatorStateInfo(0).IsName("FromDescending") || animator.GetCurrentAnimatorStateInfo(0).IsName("FromDescendingLeft"))
                ;
            /*else if (isRunning /*|| !isRunning)
            {
                Debug.Log("test " + mainFall);
                isGrounded = false;
                animator.SetBool("Falling", true);
                if (body.collider == null && isForward)
                    transform.position = new Vector3(transform.position.x + mainFall, transform.position.y, transform.position.z);
                else if (body.collider == null && !isForward)
                    transform.position = new Vector3(transform.position.x - mainFall, transform.position.y, transform.position.z);
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("ToFalling") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Falling"))
                {
                    animator.Play("ToFalling");
                }

                if (beforeFall.y == 0)
                    beforeFall = transform.position;
            }*/
            else
            {
                Debug.Log(mainFall);
                mainFall -= 0.001f;
                isGrounded = false;
                animator.SetBool("Falling", true);
                if (body.collider == null && isForward)
                    gravity = new Vector3(mainFall, 0, 0);
                else if (body.collider == null && !isForward)
                    gravity = new Vector3(-mainFall, 0, 0);
                else if (body.collider != null)
                    gravity = new Vector3(0, 0, 0);
                transform.position += gravity;
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("ToFalling") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Falling"))
                {
                    animator.Play("ToFalling");
                }

                if (beforeFall.y == 0)
                    beforeFall = transform.position;

                mainFall += 0.001f;
            }

            /*if (isForward && !isJumpingUp && !isClimbing && !isDescending && !isWinning && !animator.GetCurrentAnimatorStateInfo(0).IsName("StandingJump") && !animator.GetCurrentAnimatorStateInfo(0).IsName("RunningJump"))
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
            }*/

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
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Falling") && !isGrounded)
            {
                afterFall = transform.position;
                mainFall = defaultFall;

                //Debug.Log("Position before: " + beforeFall.y);
                //Debug.Log("Position after: " + afterFall.y);
                //Debug.Log("Difference: " + Mathf.Abs(beforeFall.y - afterFall.y));

                if (Mathf.Abs(beforeFall.y - afterFall.y) < 2f)
                {
                    isGrounded = true;
                    animator.SetBool("Falling", false);
                    //fallingMomentum = 0;

                    Footstep();
                }
                else if (Mathf.Abs(beforeFall.y - afterFall.y) >= 2f && Mathf.Abs(beforeFall.y - afterFall.y) < 4f)
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
                else if (Mathf.Abs(beforeFall.y - afterFall.y) >= 4.5f)
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

                /*if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 35)
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
                }*/

                beforeFall = new Vector2(0, 0);
                afterFall = new Vector2(0, 0);

                rb.bodyType = RigidbodyType2D.Dynamic;
                //rb.velocity = new Vector2(0, 0);
            }
        }


        //-------------------GRAB CHECK-------------------

        Debug.DrawRay(groundCheck.position, groundRaycast, Color.red);
        Debug.DrawRay(groundCheck.position, groundLongRaycast, Color.black);
        Debug.DrawRay(groundCheck.position, groundCheckRaycastShort, Color.blue);
        Debug.DrawRay(groundCheck.position, groundCheckRaycastLong, Color.red);
        Debug.DrawRay(grabCheck.position, wallAboveRaycast, Color.magenta);
        Debug.DrawRay(bodyCheck.position, bodyRaycast, Color.white);
        Debug.DrawRay(bodyCheck.position, bodyTrapRaycast, Color.cyan);
        Debug.DrawRay(grabCheck.position, grabRaycast, Color.red);
        Debug.DrawRay(groundCheck.position, dropRaycast, Color.yellow);
        Debug.DrawRay(bodyCheck.position, slowWalkRaycast, Color.green);


        //-------------------MOVEMENT-------------------

        if (Input.GetKeyDown("r"))
        {
            /*if (isDead)
            {*/
            /*rb.transform.position = startingPoint;          //DEBUG
            heroHP = 3;
            animator.ResetTrigger("Die");
            animator.Play("Standing");
            spriteRenderer.flipX = false;
            isForward = true;
            isJumping = false;
            isDead = false;
            rb.gravityScale = 1;
            CanMove();*/
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            //}
        }

        if (Input.GetKeyDown("t"))
        {
            Debug.Log("mainFall = defaultFall");
            defaultFall -= 0.001f;
            defaultFall += 0.001f;
            mainFall = defaultFall;
        }

        if (Input.GetKeyDown("u"))
        {
            Debug.Log(hands.collider.gameObject.transform.position);
        }

        if (Input.GetKeyDown("escape"))
            QuitGame();

        /*if (animator.GetCurrentAnimatorStateInfo(0).IsName("Falling"))
        {
            if (Input.GetKey("left shift"))
            {
                if (hands.collider == null)
                    ;
                else if (LayerMask.LayerToName(hands.collider.gameObject.layer) == "Grab")
                {
                    Debug.Log("Could grab midair - diagonal");
                    if (isForward && !hands.collider.GetComponent<GrabController>().isLeft)
                    {
                        if (hands.collider.GetComponent<GrabController>().noWall)
                            animator.SetBool("Swing", true);
                        else if (!hands.collider.GetComponent<GrabController>().noWall)
                            animator.SetBool("Swing", false);

                        if (hands.collider.GetComponent<GrabController>().noFloor)
                            animator.SetBool("NoFloor", true);
                        else if (!hands.collider.GetComponent<GrabController>().noFloor)
                            animator.SetBool("NoFloor", false);

                        //rb.gravityScale = 0;
                        rb.bodyType = RigidbodyType2D.Kinematic;
                        isJumping = true;
                        animator.SetBool("Falling", false);
                        Debug.Log("RigidbodyType: " + rb.bodyType);

                        rb.position = new Vector2(hands.collider.gameObject.transform.position.x - 0.36f + 0.294f, hands.collider.gameObject.transform.position.y + 0.6f /*- 1.131f + 0.51154f);

                        isGrounded = false;
                        isClimbing = true;
                        animator.SetBool("Grip", true);
                    }
                    else if (!isForward && hands.collider.GetComponent<GrabController>().isLeft)
                    {
                        if (hands.collider.GetComponent<GrabController>().noWall)
                            animator.SetBool("Swing", true);
                        else if (!hands.collider.GetComponent<GrabController>().noWall)
                            animator.SetBool("Swing", false);

                        rb.position = new Vector2(hands.collider.gameObject.transform.position.x + 0.3f, rb.position.y);

                        isGrounded = false;
                        isClimbing = true;
                        animator.SetBool("Grip", true);
                    }
                    if (isForward && !hands.collider.GetComponent<GrabController>().isLeft)
                    {
                        if (hands.collider.GetComponent<GrabController>().noFloor)
                        {
                            animator.SetBool("NoFloor", true);
                            //rb.gravityScale = 0;
                        }

                        rb.position = new Vector2(hands.collider.gameObject.transform.position.x - 0.36f, rb.position.y);

                        //isGrounded = false;
                        isClimbing = true;
                        animator.SetBool("Grip", true);
                    }
                    else if (!isForward && hands.collider.GetComponent<GrabController>().isLeft)
                    {
                        if (hands.collider.GetComponent<GrabController>().noFloor)
                        {
                            animator.SetBool("NoFloor", true);
                            //rb.gravityScale = 0;
                        }

                        rb.position = new Vector2(hands.collider.gameObject.transform.position.x + 0.3f, rb.position.y);

                        isGrounded = false;
                        isClimbing = true;
                        animator.SetBool("Grip", true);
                    }
                    animator.Play("BeforeHanging");
                }

                if (handsVertical.collider == null)
                    ;
                else if (LayerMask.LayerToName(handsVertical.collider.gameObject.layer) == "Grab" && animator.GetCurrentAnimatorStateInfo(0).IsName("Falling"))
                {
                    Debug.Log("Could grab midair - diagonal");
                    if (isForward && !handsVertical.collider.GetComponent<GrabController>().isLeft)
                    {
                        if (handsVertical.collider.GetComponent<GrabController>().noFloor)
                        {
                            animator.SetBool("NoFloor", true);
                            //rb.gravityScale = 0;
                        }

                        rb.position = new Vector2(handsVertical.collider.gameObject.transform.position.x - 0.36f, rb.position.y);

                        //isGrounded = false;
                        isClimbing = true;
                        animator.SetBool("Grip", true);
                    }
                    else if (!isForward && handsVertical.collider.GetComponent<GrabController>().isLeft)
                    {
                        if (handsVertical.collider.GetComponent<GrabController>().noFloor)
                        {
                            animator.SetBool("NoFloor", true);
                            //rb.gravityScale = 0;
                        }

                        rb.position = new Vector2(handsVertical.collider.gameObject.transform.position.x + 0.3f, rb.position.y);

                        isGrounded = false;
                        isClimbing = true;
                        animator.SetBool("Grip", true);
                    }
                    animator.Play("BeforeHanging");
                }
            }
        }*/

        /*if (LayerMask.LayerToName(hands.collider.gameObject.layer) == "Grab" && !isGrounded)
            {
                Debug.Log("Could grab midair - diagonal");
                if (isForward && !hands.collider.GetComponent<GrabController>().isLeft)
                {
                    if (hands.collider.GetComponent<GrabController>().noFloor)
                    {
                        animator.SetBool("NoFloor", true);
                        //rb.gravityScale = 0;
                    }

                    rb.position = new Vector2(hands.collider.gameObject.transform.position.x - 0.36f, rb.position.y);

                    //isGrounded = false;
                    isClimbing = true;
                    animator.SetBool("Grip", true);
                }
                else if (!isForward && hands.collider.GetComponent<GrabController>().isLeft)
                {
                    if (hands.collider.GetComponent<GrabController>().noFloor)
                    {
                        animator.SetBool("NoFloor", true);
                        //rb.gravityScale = 0;
                    }

                    rb.position = new Vector2(hands.collider.gameObject.transform.position.x + 0.3f, rb.position.y);

                    isGrounded = false;
                    isClimbing = true;
                    animator.SetBool("Grip", true);
                }
                animator.Play("BeforeHanging");
            }
            else if (LayerMask.LayerToName(hands.collider.gameObject.layer) == "Grab" && !isGrounded)
            {
                Debug.Log("Could grab midair - vertical");
                if (isForward && !hands.collider.GetComponent<GrabController>().isLeft)
                {
                    if (hands.collider.GetComponent<GrabController>().noFloor)
                    {
                        animator.SetBool("NoFloor", true);
                        //rb.gravityScale = 0;
                    }

                    rb.position = new Vector2(hands.collider.gameObject.transform.position.x - 0.36f, rb.position.y);

                    //isGrounded = false;
                    isClimbing = true;
                    animator.SetBool("Grip", true);
                }
                else if (!isForward && hands.collider.GetComponent<GrabController>().isLeft)
                {
                    if (hands.collider.GetComponent<GrabController>().noFloor)
                    {
                        animator.SetBool("NoFloor", true);
                        //rb.gravityScale = 0;
                    }

                    rb.position = new Vector2(hands.collider.gameObject.transform.position.x + 0.3f, rb.position.y);

                    isGrounded = false;
                    isClimbing = true;
                    animator.SetBool("Grip", true);
                }
                animator.Play("BeforeHanging");
            }*/

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
                        //rb.velocity = new Vector2(runSpeed, rb.velocity.y);
                        isRunning = true;

                        defaultFall -= 0.001f;
                        defaultFall += 0.001f;

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
                        //rb.velocity = new Vector2(crouchSpeed, rb.velocity.y);

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
                        //rb.velocity = new Vector2(-runSpeed, rb.velocity.y);
                        isRunning = true;

                        defaultFall -= 0.001f;
                        defaultFall += 0.001f;

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
                        //rb.velocity = new Vector2(-crouchSpeed, rb.velocity.y);

                        if (animator.GetCurrentAnimatorStateInfo(0).IsName("CrouchMoveLeft"))
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

            else if (Input.GetKeyDown("up") && !isRunning && !isClimbing && !animator.GetCurrentAnimatorStateInfo(0).IsName("StartRunning") && !animator.GetCurrentAnimatorStateInfo(0).IsName("StartRunningLeft") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Climbing") && !animator.GetCurrentAnimatorStateInfo(0).IsName("ClimbingLeft"))
            {
                if (exitCheck.collider == null || exitCheck.collider.tag != "Exit")
                {
                    if (groundAboveCheck.collider != null && handsVertical.collider == null || wallAboveCheck.collider != null && handsVertical.collider == null)
                    {
                        //animator.SetTrigger("JumpWallAbove");
                        if (!animator.GetBool("isLeft"))
                            animator.Play("JumpUp with Wall");
                        else if (animator.GetBool("isLeft"))
                            animator.Play("JumpUpLeft with Wall");

                        defaultFall -= 0.001f;
                        defaultFall += 0.001f;
                    }
                    else
                    {
                        //animator.SetTrigger("Jump");
                        if (!animator.GetBool("isLeft"))
                            animator.Play("JumpUp");
                        else if (animator.GetBool("isLeft"))
                            animator.Play("JumpUpLeft");

                        defaultFall -= 0.001f;
                        defaultFall += 0.001f;

                        if (hands.collider == null)
                            ;
                        else if (LayerMask.LayerToName(hands.collider.gameObject.layer) == "Grab" && !isClimbing)
                        {
                            //Debug.Log("Can grab");

                            if (isForward && !hands.collider.GetComponent<GrabController>().isLeft)
                            {
                                if (hands.collider.GetComponent<GrabController>().noWall)
                                    animator.SetBool("Swing", true);
                                else if (!hands.collider.GetComponent<GrabController>().noWall)
                                    animator.SetBool("Swing", false);

                                rb.position = new Vector2(hands.collider.gameObject.transform.position.x - 0.36f, rb.position.y);

                                isGrounded = false;
                                isClimbing = true;
                                animator.SetBool("Grip", true);
                            }
                            else if (!isForward && hands.collider.GetComponent<GrabController>().isLeft)
                            {
                                if (hands.collider.GetComponent<GrabController>().noWall)
                                    animator.SetBool("Swing", true);
                                else if (!hands.collider.GetComponent<GrabController>().noWall)
                                    animator.SetBool("Swing", false);

                                rb.position = new Vector2(hands.collider.gameObject.transform.position.x + 0.3f, rb.position.y);

                                isGrounded = false;
                                isClimbing = true;
                                animator.SetBool("Grip", true);
                            }
                            else
                            {
                                //animator.SetTrigger("Jump");
                                if (!animator.GetBool("isLeft"))
                                    animator.Play("JumpUp");
                                else if (animator.GetBool("isLeft"))
                                    animator.Play("JumpUpLeft");
                            }
                        }

                        if (handsVertical.collider == null)
                            ;
                        else if (LayerMask.LayerToName(handsVertical.collider.gameObject.layer) == "Grab" && !isClimbing)
                        {
                            if (isForward && !handsVertical.collider.GetComponent<GrabController>().isLeft)
                            {
                                if (handsVertical.collider.GetComponent<GrabController>().noFloor)
                                {
                                    animator.SetBool("NoFloor", true);
                                    rb.gravityScale = 0;
                                }
                                if (handsVertical.collider.GetComponent<GrabController>().noWall)
                                    animator.SetBool("Swing", true);
                                else if (!handsVertical.collider.GetComponent<GrabController>().noWall)
                                    animator.SetBool("Swing", false);

                                rb.position = new Vector2(handsVertical.collider.gameObject.transform.position.x - 0.36f, rb.position.y);

                                isGrounded = false;
                                isClimbing = true;
                                animator.SetBool("Grip", true);
                            }
                            else if (!isForward && handsVertical.collider.GetComponent<GrabController>().isLeft)
                            {
                                if (handsVertical.collider.GetComponent<GrabController>().noFloor)
                                {
                                    animator.SetBool("NoFloor", true);
                                    rb.gravityScale = 0;
                                }
                                if (handsVertical.collider.GetComponent<GrabController>().noWall)
                                    animator.SetBool("Swing", true);
                                else if (!handsVertical.collider.GetComponent<GrabController>().noWall)
                                    animator.SetBool("Swing", false);

                                rb.position = new Vector2(handsVertical.collider.gameObject.transform.position.x + 0.3f, rb.position.y);

                                isGrounded = false;
                                isClimbing = true;
                                animator.SetBool("Grip", true);
                            }
                            else
                            {
                                //animator.SetTrigger("Jump");
                                if (!animator.GetBool("isLeft"))
                                    animator.Play("JumpUp");
                                else if (animator.GetBool("isLeft"))
                                    animator.Play("JumpUpLeft");
                            }
                        }
                    }
                }
                else if (exitCheck.collider.tag == "Exit" && !isCrouching)
                {
                    CantMove();
                    isWinning = true;
                    rb.gravityScale = 0;
                    spriteRenderer.flipX = false;
                    rb.transform.position = new Vector3(exitCheck.collider.transform.position.x, exitCheck.collider.transform.position.y - 0.3f, 1);
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

                    defaultFall -= 0.001f;
                    defaultFall += 0.001f;
                }
                else if (LayerMask.LayerToName(handsDrop.collider.gameObject.layer) == "Grab" && !animator.GetCurrentAnimatorStateInfo(0).IsName("Descending"))
                {
                    if (isForward && !handsDrop.collider.GetComponent<GrabController>().isLeft)
                    {
                        if (handsDrop.collider.GetComponent<GrabController>().noFloor)
                            animator.SetBool("NoFloor", true);
                        else if (!handsDrop.collider.GetComponent<GrabController>().noFloor)
                            animator.SetBool("NoFloor", false);
                        if (handsDrop.collider.GetComponent<GrabController>().noWall)
                            animator.SetBool("Swing", true);
                        else if (!handsDrop.collider.GetComponent<GrabController>().noWall)
                            animator.SetBool("Swing", false);

                        rb.position = new Vector2(handsDrop.collider.gameObject.transform.position.x + 0.1f, rb.position.y);
                        isGrounded = false;
                        isClimbing = true;
                        isDescending = true;
                        animator.SetTrigger("Descend");
                        animator.SetBool("Grip", true);
                    }
                    else if (!isForward && handsDrop.collider.GetComponent<GrabController>().isLeft)
                    {
                        if (handsDrop.collider.GetComponent<GrabController>().noFloor)
                            animator.SetBool("NoFloor", true);
                        else if (!handsDrop.collider.GetComponent<GrabController>().noFloor)
                            animator.SetBool("NoFloor", false);
                        if (handsDrop.collider.GetComponent<GrabController>().noWall)
                            animator.SetBool("Swing", true);
                        else if (!handsDrop.collider.GetComponent<GrabController>().noWall)
                            animator.SetBool("Swing", false);

                        rb.position = new Vector2(handsDrop.collider.gameObject.transform.position.x - 0.15f, rb.position.y);
                        isGrounded = false;
                        isClimbing = true;
                        isDescending = true;
                        animator.SetTrigger("Descend");
                        animator.SetBool("Grip", true);
                    }
                    else
                    {
                        animator.SetBool("Crouch", true);
                        isCrouching = true;
                    }
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
            else if (Input.GetKeyUp("left shift"))
                ResetOneStep();
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Swinging") && isDescending)
            {
                if (Input.GetKey("left shift"))
                {
                    animator.SetBool("Grip", true);
                }
                else
                {
                    isDescending = false;
                    animator.SetBool("Grip", false);
                }
            }
            else
            {
                animator.SetBool("Running", false);
                animator.SetBool("Crouch", false);
                //animator.SetBool("Swing", false);
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
            cap.size = new Vector2(cap.size.x, 0.7557176f);
            cap.offset = new Vector2(cap.offset.x, -0.4464909f);
        }
        if (isJumping)
        {
            cap.size = new Vector2(cap.size.x, 1.067267f);
            cap.offset = new Vector2(cap.offset.x, -0.00194484f);
        }
        else if (!isCrouching && !isJumping)
        {
            cap.size = new Vector2(cap.size.x, defaultValue);
            cap.offset = new Vector2(cap.offset.x, defaultOffset);
        }
    }

    void AfterDeath()
    {
        afterDeath = true;

        deathText.enabled = true;
    }

    void Footstep()
    {
        audioSource.clip = footsteps[Random.Range(0, footsteps.Length)];
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Step") || animator.GetCurrentAnimatorStateInfo(0).IsName("StepLeft"))
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

    void SpecialFootstep()
    {
        if (feetLong.collider != null)
        {
            audioSource.clip = footsteps[Random.Range(0, footsteps.Length)];
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.volume = 1;
            audioSource.PlayOneShot(audioSource.clip);
        }
    }

    void ChangeToFalling()
    {
        if (feetLong.collider == null)
        {
            isGrounded = false;
            isClimbing = false;
            isDescending = false;
            animator.SetBool("Falling", true);
            animator.Play("Falling");
        }
    }

    void ThereIsFloorNow()
    {
        animator.SetBool("NoFloor", false);
    }

    void LetItSwing()
    {
        isClimbing = false;
        isDescending = false;
        animator.SetBool("Grip", false);
    }

    void AboveWallChangeLayer()
    {
        if (wallAboveCheck.collider != null)
            if (wallAboveCheck.collider.tag == "Above Wall")
                wallAboveCheck.collider.GetComponent<WallController>().ChangeLayer(1.3f);
    }

    void ChangeForward()
    {
        animator.ResetTrigger("Turn");
        isForward = !isForward;
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    void ChangeJump()
    {
        isJumping = !isJumping;

        if (rb.bodyType == RigidbodyType2D.Dynamic)
            rb.bodyType = RigidbodyType2D.Kinematic;
        else
            rb.bodyType = RigidbodyType2D.Dynamic;
    }

    void ClimbJumpForwardChange()
    {
        isGrounded = true;
        isClimbing = false;
        animator.SetBool("Grip", false);
    }

    void CheckStep()
    {
        if (slowWalkCheck.collider == null)
        {
            //Debug.Log("Slow Walk Check");
            if (isForward)
            {
                if (feet.collider != null)
                    rb.position = new Vector2(feet.collider.transform.position.x - 0.3f, rb.transform.position.y);

                animator.Play("Step", -1, 0.65f);
            }
            else if (!isForward)
            {
                if (feet.collider != null)
                    rb.position = new Vector2(feet.collider.transform.position.x - 0.3f, rb.transform.position.y);

                animator.Play("StepLeft", -1, 0.65f);
            }

            audioSource.clip = footsteps[Random.Range(0, footsteps.Length)];
            audioSource.pitch = Random.Range(0.85f, 0.95f);
            audioSource.volume = 0.35f;
            audioSource.PlayOneShot(audioSource.clip);
        }

        if (bodyTrap.collider != null && !oneStep)
        {
            //Debug.Log("Body Trap Check");
            if (isForward)
            {
                rb.position = new Vector2(bodyTrap.collider.transform.position.x - 0.5f, rb.transform.position.y);

                animator.Play("Step", -1, 0.65f);
            }
            else if (!isForward)
            {
                rb.position = new Vector2(bodyTrap.collider.transform.position.x + 0.75f, rb.transform.position.y);

                animator.Play("StepLeft", -1, 0.65f);
            }

            audioSource.clip = footsteps[Random.Range(0, footsteps.Length)];
            audioSource.pitch = Random.Range(0.85f, 0.95f);
            audioSource.volume = 0.35f;
            audioSource.PlayOneShot(audioSource.clip);

            oneStep = true;
        }
        else if (bodyTrap.collider != null && oneStep)
        {
            oneStep = false;
        }
    }

    void ResetOneStep()
    {
        if (oneStep)
            oneStep = false;
    }

    void CheckFallShort()
    {
        if (feetGroundShort.collider == null)
        {
            if (rb.bodyType == RigidbodyType2D.Kinematic)
                rb.bodyType = RigidbodyType2D.Dynamic;
            isGrounded = false;
            isJumping = false;
            rb.gravityScale = 1;
            if (body.collider == null && isForward)
                transform.position = new Vector3(transform.position.x + shortFall, transform.position.y, transform.position.z);
            else if (body.collider == null && !isForward)
                transform.position = new Vector3(transform.position.x - shortFall, transform.position.y, transform.position.z);
            mainFall = shortFall;
            animator.SetBool("Falling", true);
            animator.Play("ToFalling");
        }
        /*else if (isRunning /*|| !isRunning)
            {
            Debug.Log("test");
            isGrounded = false;
            animator.SetBool("Falling", true);
            transform.position = new Vector3(transform.position.x + 0.025f, transform.position.y, transform.position.z);
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("ToFalling") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Falling"))
            {
                animator.Play("ToFalling");
            }

            if (beforeFall.y == 0)
                beforeFall = transform.position;
        }
            else
        {
            isGrounded = false;
            animator.SetBool("Falling", true);
            if (body.collider == null)
                transform.position = new Vector3(transform.position.x + 0.025f, transform.position.y, transform.position.z);
            //animator.Play("ToFalling");

            if (beforeFall.y == 0)
                beforeFall = transform.position;
        }*/
    }

    void CheckFallLong()
    {
        if (feetGroundLong.collider == null)
        {
            if (rb.bodyType == RigidbodyType2D.Kinematic)
                rb.bodyType = RigidbodyType2D.Dynamic;
            isGrounded = false;
            isJumping = false;
            rb.gravityScale = 1;
            if (body.collider == null && isForward)
                transform.position = new Vector3(transform.position.x + longFall, transform.position.y, transform.position.z);
            else if (body.collider == null && !isForward)
                transform.position = new Vector3(transform.position.x - longFall, transform.position.y, transform.position.z);
            mainFall = longFall;
            animator.SetBool("Falling", true);
            animator.Play("ToFalling");
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
        //rb.velocity = new Vector2(0.5f, rb.velocity.y);
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
        if (isForward) ;
        //rb.velocity = new Vector2(crouchSpeed, rb.velocity.y);
        else;
            //rb.velocity = new Vector2(-crouchSpeed, rb.velocity.y);
    }

    void JumpStart()
    {
        isJumping = true;
        rb.gravityScale = 0;
    }

    void JumpStop()
    {
        isJumping = false;
        rb.gravityScale = 1;
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
                animator.Play("StandingJumpLeft");
            }
        }
    }

    void BeforeHanging()
    {
        rb.gravityScale = 0;
        //rb.velocity = new Vector2(0, 0);
    }

    void StopDescending()
    {
        isDescending = false;
        //Debug.Log(animator.GetBool("Swing"));
    }

    void AfterHanging()
    {
        rb.gravityScale = 1;
    }

    void LevelFinish()
    {
        //rb.velocity = new Vector2(0.16f, 0.36f);
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void LevelFinishStop()
    {
        //rb.velocity = new Vector2(0, 0);
        spriteRenderer.enabled = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
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
