using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChomperSound : MonoBehaviour
{
    AudioSource audioSource;
    Animator animator;

    [SerializeField] Sprite[] bloodyChompers;

    int whichCollider = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (whichCollider == 1 && animator.GetBool("inZone") == false)
        {
            animator.SetBool("inZone", true);
        }

        if (whichCollider == 0 && animator.GetBool("inZone") == true)
        {
            animator.SetBool("inZone", false);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        whichCollider++;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        whichCollider--;
    }

    void Chomp()
    {
        audioSource.PlayOneShot(audioSource.clip);

        if (whichCollider == 2 && animator.GetBool("Killed") == false)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().ChomperKill(gameObject);
            animator.Play("ChompBloody", -1, animator.GetCurrentAnimatorStateInfo(0).normalizedTime + 0.05f);
            animator.SetBool("Killed", true);
            //GetComponent<SpriteRenderer>().sprite = bloodyChompers[0];
        }
    }
}
