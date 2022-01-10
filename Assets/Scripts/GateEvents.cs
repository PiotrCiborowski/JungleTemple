using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateEvents : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Animator animator;

    Vector2 startLocation;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        //startLocation = GetComponent<Transform>().localPosition;
    }

    void GateOnMain()
    {
        spriteRenderer.sortingLayerName = "Main";
        animator.speed = 1;

        //this.transform.position = startLocation;
    }
}
