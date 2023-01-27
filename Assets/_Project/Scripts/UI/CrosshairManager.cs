using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairManager : MonoBehaviour
{
    [SerializeField] private GameObject CrossLeft;
    [SerializeField] private GameObject CrossUp;
    [SerializeField] private GameObject CrossRight;
    [SerializeField] private GameObject CrossDown;
    [SerializeField] private GameObject CrossPoint;

    private RectTransform CrossLeftRect;
    private RectTransform CrossUpRect;
    private RectTransform CrossRightRect;
    private RectTransform CrossDownRect;

    private Image CrossLeftImage;
    private Image CrossUpImage;
    private Image CrossRightImage;
    private Image CrossDownImage;
    private Image CrossPointImage;

    private void Start()
    {
        CrossLeftRect = CrossLeft.GetComponent<RectTransform>();
        CrossUpRect = CrossUp.GetComponent<RectTransform>();
        CrossRightRect = CrossRight.GetComponent<RectTransform>();
        CrossDownRect = CrossDown.GetComponent<RectTransform>();

        CrossLeftImage = CrossLeft.GetComponent<Image>();
        CrossUpImage = CrossUp.GetComponent<Image>();
        CrossRightImage = CrossRight.GetComponent<Image>();
        CrossDownImage= CrossDown.GetComponent<Image>();
        CrossPointImage = CrossPoint.GetComponent<Image>();
    }

    public void SetLenght(float lenght)
    {
        CrossLeftRect.sizeDelta = new Vector2(lenght, CrossLeftRect.sizeDelta.y);
        CrossUpRect.sizeDelta = new Vector2(CrossUpRect.sizeDelta.x, lenght);
        CrossRightRect.sizeDelta = new Vector2(lenght, CrossRightRect.sizeDelta.y);
        CrossDownRect.sizeDelta = new Vector2(CrossDownRect.sizeDelta.x, lenght);
    }

    public void SetThick(float thick)
    {
        CrossLeftRect.sizeDelta = new Vector2(CrossLeftRect.sizeDelta.x, thick);
        CrossUpRect.sizeDelta = new Vector2(thick, CrossUpRect.sizeDelta.y);
        CrossRightRect.sizeDelta = new Vector2(CrossRightRect.sizeDelta.x, thick);
        CrossDownRect.sizeDelta = new Vector2(thick, CrossDownRect.sizeDelta.y);
    }

    public void SetGap(float gap)
    {
        CrossLeftRect.anchoredPosition = new Vector2(gap, CrossLeftRect.anchoredPosition.y);
        CrossUpRect.anchoredPosition = new Vector2(CrossUpRect.anchoredPosition.x, -gap);
        CrossRightRect.anchoredPosition = new Vector2(-gap, CrossRightRect.anchoredPosition.y);
        CrossDownRect.anchoredPosition = new Vector2(CrossDownRect.anchoredPosition.x, gap);
    }

    public void SetRedColor(float red)
    {
        CrossLeftImage.color = new Color(red, CrossLeftImage.color.g, CrossLeftImage.color.b);
        CrossUpImage.color = new Color(red, CrossUpImage.color.g, CrossUpImage.color.b);
        CrossRightImage.color = new Color(red, CrossRightImage.color.g, CrossRightImage.color.b);
        CrossDownImage.color = new Color(red, CrossDownImage.color.g, CrossDownImage.color.b);
        CrossPointImage.color = new Color(red, CrossPointImage.color.g, CrossPointImage.color.b);
    }

    public void SetGreenColor(float green)
    {
        CrossLeftImage.color = new Color(CrossLeftImage.color.r, green, CrossLeftImage.color.b);
        CrossUpImage.color = new Color(CrossUpImage.color.r, green, CrossUpImage.color.b);
        CrossRightImage.color = new Color(CrossRightImage.color.r, green, CrossRightImage.color.b);
        CrossDownImage.color = new Color(CrossDownImage.color.r, green, CrossDownImage.color.b);
        CrossPointImage.color = new Color(CrossPointImage.color.r, green, CrossPointImage.color.b);
    }

    public void SetBlueColor(float blue)
    {
        CrossLeftImage.color = new Color(CrossLeftImage.color.r, CrossLeftImage.color.g, blue);
        CrossUpImage.color = new Color(CrossUpImage.color.r, CrossUpImage.color.g, blue);
        CrossRightImage.color = new Color(CrossRightImage.color.r, CrossRightImage.color.g, blue);
        CrossDownImage.color = new Color(CrossDownImage.color.r, CrossDownImage.color.g, blue);
        CrossPointImage.color = new Color(CrossPointImage.color.r, CrossPointImage.color.g, blue);
    }
}
