using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignOrder : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (gameObject.layer == 9 && spriteRenderer.sprite.name != "EndWall")
            spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.x - 0.9f) + Mathf.RoundToInt(transform.position.y - 0.9f);
        else if (gameObject.layer == 9 && spriteRenderer.sprite.name == "EndWall")
            spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.x + 1.1f) + Mathf.RoundToInt(transform.position.y + 1.1f);
        else
            spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.x + 0.1f) + Mathf.RoundToInt(transform.position.y + 0.1f);
    }
}
