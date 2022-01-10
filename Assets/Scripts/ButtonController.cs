using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    Animator animator;
    GameObject player;

    bool inUse = false;

    [SerializeField] GameObject gate;
    [SerializeField] bool canOpen;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");

        //player.transform.position = new Vector2(this.transform.position.x - 0.6f, this.transform.position.y);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && PlayerController.pressButton && PlayerController.almostPressButton)
        {
            animator.SetBool("Pressed", true);

            if (canOpen && !inUse)
            {
                //gate.GetComponent<Animator>().SetBool("isOpen", true);
                gate.GetComponent<Animator>().SetTrigger("testOpen");
                gate.GetComponent<SpriteRenderer>().sortingLayerName = "Background";

                //Invoke("CloseGate", 5);

                Debug.Log("Opens the gate");
            }
            else if (!canOpen && !inUse && !gate.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("CloseGate") && !gate.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("OpeningGate"))
            {
                //CancelInvoke("CloseGate");
                //CloseGate();

                gate.GetComponent<Animator>().speed = 7;
                gate.GetComponent<Animator>().Play("ClosingGate");

                Debug.Log("Closes the gate");
            }

            inUse = true;
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && PlayerController.almostPressButton)
        {
            if (player.transform.position.x >= this.transform.position.x - 0.325f && player.transform.position.x <= this.transform.position.x - 0.275f)
            {
                animator.SetBool("Pressed", true);

                if (canOpen && !inUse)
                {
                    gate.GetComponent<Animator>().SetBool("isOpen", true);
                    gate.GetComponent<SpriteRenderer>().sortingLayerName = "Background";

                    Debug.Log("Opens the gate");
                }
                else if (!canOpen && !inUse)
                {
                    Debug.Log("Closes the gate");
                }
            }
            /*else if (collision.gameObject.tag == "Player" && PlayerController.pressButton)
            {
                animator.SetBool("Pressed", true);

                if (canOpen && !inUse)
                {
                    gate.GetComponent<Animator>().SetBool("isOpen", true);
                    gate.GetComponent<SpriteRenderer>().sortingLayerName = "Background";

                    Debug.Log("Opens the gate");
                }
                else if (!canOpen && !inUse)
                {
                    Debug.Log("Closes the gate");
                }
            }*/

            inUse = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            animator.SetBool("Pressed", false);
            inUse = false;
        }
    }

    void CloseGate()
    {
        gate.GetComponent<Animator>().SetBool("isOpen", false);
        //gate.GetComponent<SpriteRenderer>().sortingLayerName = "Main";

        //inUse = false;
        Debug.Log(inUse);
        Debug.Log("CloseGate");
    }
}
