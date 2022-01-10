using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour
{
    [SerializeField] GameObject sideWall;

    public void ChangeLayer()
    {
        if (sideWall != null)
        {
            GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
            sideWall.GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
            StartCoroutine(SetToBackground(1.6f));
        }
    }

    IEnumerator SetToBackground(float time)
    {
        yield return new WaitForSeconds(time);
        GetComponent<SpriteRenderer>().sortingLayerName = "Background";
        sideWall.GetComponent<SpriteRenderer>().sortingLayerName = "Background";
    }
}
