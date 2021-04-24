using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayedCardContainer : MonoBehaviour
{
    private bool hasCardOver = false;
    public bool HasCardOver
    {
        get { return hasCardOver; }
    }
    [SerializeField]
    private Image emptyImage;
    public Image EmptyImage
    {
        get { return emptyImage; }
    }
    private Transform placement;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Card")
        {
            hasCardOver = true;
            emptyImage.gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Card")
        {
            emptyImage.gameObject.SetActive(true);
            hasCardOver = false;
        }
    }

}
