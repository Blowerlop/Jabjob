using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    public float partyDuration = 300; //Durée de la partie en seconde

    private void Update()
    {
        float t = Time.time - partyDuration;

        string minutes = Mathf.Abs(((int)t / 60)).ToString();

        string seconds;
        //Permet un affichage 0 : 00 et non pas 0 : 5 si les secondes sont inferieur à 10
        if (Mathf.Abs(t % 60) <= 9.5) { seconds = "0" + Mathf.Abs((t % 60)).ToString("f0"); }
        else if (Mathf.Abs(t % 60) >= 59.3) { seconds = "59"; }
        else { seconds = Mathf.Abs((t % 60)).ToString("f0"); }

        timerText.text = minutes + ":" + seconds;
    }
}
