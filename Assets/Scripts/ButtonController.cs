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

    Animator gateAnim;
    AudioSource gateAudio;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");

        gateAnim = gate.GetComponent<Animator>();
        gateAudio = gate.GetComponent<AudioSource>();

        //player.transform.position = new Vector2(this.transform.position.x - 0.6f, this.transform.position.y);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player"/* && PlayerController.pressButton && PlayerController.almostPressButton*/)
        {
            animator.SetBool("Pressed", true);
            GetComponent<AudioSource>().Play();

            if (canOpen && !inUse)
            {
                if (gateAnim.GetCurrentAnimatorStateInfo(0).IsName("CloseGate"))
                {
                    gateAnim.SetTrigger("Open");
                    gateAudio.clip = gate.GetComponent<GateEvents>().gateOpening;
                    gateAudio.Play();
                    Debug.Log("Opens the gate");
                }
                else if (gateAnim.GetCurrentAnimatorStateInfo(0).IsName("OpeningGate"))
                    Debug.Log("Gate is already opening");
                else if (gateAnim.GetCurrentAnimatorStateInfo(0).IsName("OpenGate"))
                {
                    gateAnim.Play("OpenGate", -1, 0);
                    Debug.Log("Reset open timer");
                }
                /*else if (gateAnim.GetCurrentAnimatorStateInfo(0).IsName("ClosingGate"))
                {
                    float currentNormalizedTime = 1f - gateAnim.GetCurrentAnimatorStateInfo(0).normalizedTime;
                    gateAnim.Play("OpeningGate", -1, currentNormalizedTime);
                    Debug.Log("Open closing gate");
                }*/
            }
            else if (!canOpen && !inUse)
            {
                gateAnim.speed = 20;

                if (gateAnim.GetCurrentAnimatorStateInfo(0).IsName("CloseGate"))
                {
                    gateAnim.speed = 1;
                    Debug.Log("Gate is already closed");
                }
                /*else if (gateAnim.GetCurrentAnimatorStateInfo(0).IsName("OpeningGate"))
                {
                    float currentTime = 1f - gateAnim.GetCurrentAnimatorStateInfo(0).normalizedTime;
                    gateAnim.Play("ClosingGate", -1, currentTime);
                    Debug.Log("Close opening gate");
                }*/
                else if (gateAnim.GetCurrentAnimatorStateInfo(0).IsName("OpenGate"))
                {
                    gateAnim.Play("ClosingGate", -1, 0);
                    gateAudio.Stop();
                    Debug.Log("Close gate");
                }
                else if (gateAnim.GetCurrentAnimatorStateInfo(0).IsName("ClosingGate"))
                {
                    gateAudio.Stop();
                    Debug.Log("Fast close gate");
                }
            }

            inUse = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //gateAnim.speed = 1;
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
