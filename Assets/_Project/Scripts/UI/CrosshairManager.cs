using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairManager : MonoBehaviour
{
    [SerializeField] private RectTransform Cross1;
    [SerializeField] private RectTransform Cross2;
    [SerializeField] private RectTransform Cross3;
    [SerializeField] private RectTransform Cross4;

    public void SetLenght(float lenght)
    {
        Cross1.sizeDelta = new Vector2(lenght, Cross1.sizeDelta.y);
        Cross2.sizeDelta = new Vector2(Cross2.sizeDelta.x, lenght);
        Cross3.sizeDelta = new Vector2(lenght, Cross3.sizeDelta.y);
        Cross4.sizeDelta = new Vector2(Cross4.sizeDelta.x, lenght);
    }

    public void SetThick(float thick)
    {
        Cross1.sizeDelta = new Vector2(Cross1.sizeDelta.x, thick);
        Cross2.sizeDelta = new Vector2(thick, Cross2.sizeDelta.y);
        Cross3.sizeDelta = new Vector2(Cross3.sizeDelta.x, thick);
        Cross4.sizeDelta = new Vector2(thick, Cross4.sizeDelta.y);
    }

    public void SetGap(float gap)
    {
        Cross1.anchoredPosition = new Vector2(gap, Cross1.anchoredPosition.y);
        Cross2.anchoredPosition = new Vector2(Cross2.anchoredPosition.x, -gap);
        Cross3.anchoredPosition = new Vector2(-gap, Cross3.anchoredPosition.y);
        Cross4.anchoredPosition = new Vector2(Cross4.anchoredPosition.x, gap);
    }
}
