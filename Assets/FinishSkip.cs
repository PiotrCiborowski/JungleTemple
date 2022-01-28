using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishSkip : MonoBehaviour
{
    void Update()
    {
        if (Input.anyKeyDown)
        {
            Application.Quit();
            Debug.Log("Quit");
        }
    }
}
