using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateEvents : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Animator animator;

    Vector2 aboveGate;
    RaycastHit2D aboveGateCheck;

    public AudioClip gateOpening;
    public AudioClip gateClosing;
    public AudioClip gateShut;

    Vector2 startLocation;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        //startLocation = GetComponent<Transform>().localPosition;
    }

    void Update()
    {
        aboveGate = new Vector2(0, 1.2f);

        aboveGateCheck = Physics2D.Raycast(transform.position, Vector2.up, Mathf.Abs(aboveGate.y), 1 << 9);

        Debug.DrawRay(transform.position, aboveGate, Color.yellow);
    }

    void ToForeground()
    {
        if (aboveGateCheck.collider.tag == "Above Wall")
        {
            //StopAllCoroutines();
            aboveGateCheck.collider.GetComponent<WallController>().ChangeLayer(true);
        }
    }

    void ToBackground()
    {
        if (aboveGateCheck.collider.tag == "Above Wall")
            aboveGateCheck.collider.GetComponent<WallController>().ChangeLayer(false);
    }

    void ClosingAudio()
    {
        if (animator.speed == 1)
        {
            GetComponent<AudioSource>().clip = gateClosing;
            GetComponent<AudioSource>().Play();
        }
    }

    void NormalSpeed()
    {
        if (animator.speed != 1)
        {
            GetComponent<AudioSource>().clip = gateShut;
            GetComponent<AudioSource>().Play();
        }
        animator.speed = 1;
    }

    void ReverseGate()
    {
        //animator.applyRootMotion = false;
    }
}
