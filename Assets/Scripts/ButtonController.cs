using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    Animator animator;
    GameObject player;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");

        //player.transform.position = new Vector2(this.transform.position.x - 0.6f, this.transform.position.y);
    }

    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && PlayerController.pressButton && PlayerController.almostPressButton)
            animator.SetBool("Pressed", true);
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && PlayerController.almostPressButton)
        {
            if (player.transform.position.x >= this.transform.position.x - 0.325f && player.transform.position.x <= this.transform.position.x - 0.275f)
                animator.SetBool("Pressed", true);
            else if (collision.gameObject.tag == "Player" && PlayerController.pressButton)
                animator.SetBool("Pressed", true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            animator.SetBool("Pressed", false);
    }
}
