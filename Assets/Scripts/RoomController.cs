using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RoomController : MonoBehaviour
{
    CinemachineVirtualCamera vcam;

    void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Hero")
            vcam.Priority = 1;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "Hero")
            vcam.Priority = 0;
    }
}
