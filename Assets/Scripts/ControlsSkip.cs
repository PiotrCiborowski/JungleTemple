using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ControlsSkip : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI story;
    [SerializeField] TextMeshProUGUI controls;

    private bool canSkip = false;

    void Start()
    {
        Invoke("ChangeToStory", 5f);
    }

    void ChangeToStory()
    {
        Debug.Log("Now story");

        title.enabled = false;
        story.enabled = true;

        Invoke("ChangeToControls", 15f);
    }

    void ChangeToControls()
    {
        Debug.Log("Now controls");

        story.enabled = false;
        controls.enabled = true;

        canSkip = true;
    }

    void Update()
    {
        if (canSkip && Input.anyKeyDown)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
