using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignOrder : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    GameObject hero;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hero = GameObject.FindGameObjectWithTag("Player");

        if (gameObject.layer == 9)
            spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.x - 0.9f) + Mathf.RoundToInt(transform.position.y - 0.9f);
        else if (gameObject.tag == "Gate")
        {
            spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.x - 0.9f) + Mathf.RoundToInt(transform.position.y - 0.9f);
        }
        else
            spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.x + 0.1f) + Mathf.RoundToInt(transform.position.y + 0.1f);
    }

    void Update()
    {
        if (spriteRenderer.sprite.name == "EndFloorTop")
        {
            if (spriteRenderer.gameObject.transform.position.y < hero.transform.position.y)
                spriteRenderer.sortingLayerName = "Background";
            else if (spriteRenderer.gameObject.transform.position.y >= hero.transform.position.y)
                spriteRenderer.sortingLayerName = "Foreground";
        }

        if (gameObject.tag == "Gate")
        {
            if (spriteRenderer.gameObject.transform.position.x < hero.transform.position.x - 0.2f)
                spriteRenderer.sortingLayerName = "GateInBack";
            else if (spriteRenderer.gameObject.transform.position.x >= hero.transform.position.x - 0.2f)
                spriteRenderer.sortingLayerName = "GateInFront";
        }
    }
}
