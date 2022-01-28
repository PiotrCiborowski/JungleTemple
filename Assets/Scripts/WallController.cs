using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour
{
    [SerializeField] GameObject sideWall;

    public void ChangeLayer(float time)
    {
        if (sideWall != null && GetComponent<SpriteRenderer>().sortingLayerName != "Foreground")
        {
            GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
            sideWall.GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
            StartCoroutine(SetToBackground(time));
        }
        else if (sideWall == null && GetComponent<SpriteRenderer>().sortingLayerName != "Foreground")
        {
            GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
            //sideWall.GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
            StartCoroutine(SetToBackgroundWithout(time));
        }
    }

    public void ChangeLayer(bool yes)
    {
        if (yes && sideWall != null)
        {
            GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
            sideWall.GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
        }
        else if (yes && sideWall == null)
        {
            GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
        }
        else if (!yes && sideWall != null)
        {
            GetComponent<SpriteRenderer>().sortingLayerName = "Background";
            sideWall.GetComponent<SpriteRenderer>().sortingLayerName = "Background";
        }
        else if (!yes && sideWall == null)
        {
            GetComponent<SpriteRenderer>().sortingLayerName = "Background";
        }
    }

    IEnumerator SetToBackground(float time)
    {
        yield return new WaitForSeconds(time);
        GetComponent<SpriteRenderer>().sortingLayerName = "Background";
        sideWall.GetComponent<SpriteRenderer>().sortingLayerName = "Background";
    }

    IEnumerator SetToBackgroundWithout(float time)
    {
        yield return new WaitForSeconds(time);
        GetComponent<SpriteRenderer>().sortingLayerName = "Background";
        //sideWall.GetComponent<SpriteRenderer>().sortingLayerName = "Background";
    }
}
