using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    Image image;

    new string name;
    char number;

    [SerializeField] Sprite heart;
    [SerializeField] Sprite grayHeart;

    void Start()
    {
        image = this.GetComponent<Image>();

        name = this.gameObject.name;
        number = name[name.Length - 1];
    }

    void Update()
    {
        if (int.Parse(number.ToString()) <= PlayerController.heroHP)
            image.sprite = heart;
        else
            image.sprite = grayHeart;
    }
}
