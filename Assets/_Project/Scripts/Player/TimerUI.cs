using System.Collections;
using System.Collections.Generic;
using Project;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    private GameObject UI;
    [SerializeField] private TextMeshProUGUI timerText;

    private void Start()
    {
        UI = GameObject.FindGameObjectWithTag("UI Player");
        timerText = UI.FindComponentInChildWithTag<TextMeshProUGUI>("Timer");
    }

    //private void Update()
    //{
    //    float t = NetworkTimer.instance.GetElapsedTime();


    //    string minutes = Mathf.Abs(((int)t / 60)).ToString();

    //    string seconds;
    //    //Permet un affichage 0 : 00 et non pas 0 : 5 si les secondes sont inferieur � 10
    //    if (Mathf.Abs(t % 60) <= 9.5) { seconds = "0" + Mathf.Abs((t % 60)).ToString("f0"); }
    //    else if (Mathf.Abs(t % 60) >= 59.3) { seconds = "59"; }
    //    else { seconds = Mathf.Abs((t % 60)).ToString("f0"); }

    //    timerText.text = minutes + ":" + seconds;
    //}
}

public static class Helper
{
    public static T FindComponentInChildWithTag<T>(this GameObject parent, string tag) where T : Component
    {
        Transform t = parent.transform;
        foreach (Transform tr in t)
        {
            if (tr.tag == tag)
            {
                return tr.GetComponent<T>();
            }
        }
        return null;
    }
}
