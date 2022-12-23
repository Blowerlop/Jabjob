using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    GameObject player;
    PlayerStats playerStats;

    TextMeshProUGUI name;
    TextMeshProUGUI kill;
    TextMeshProUGUI death;
    TextMeshProUGUI score;
    TextMeshProUGUI health;
    TextMeshProUGUI ping;

    private void OnEnable()
    {
        //Instantiation
        player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();

        name = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        kill = gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        death = gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        score = gameObject.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        health = gameObject.transform.GetChild(4).GetComponent<TextMeshProUGUI>();
        ping = gameObject.transform.GetChild(5).GetComponent<TextMeshProUGUI>();

        name.text = playerStats.name;
        kill.text = playerStats.kill.ToString();
        death.text = playerStats.death.ToString();
        score.text = playerStats.score.ToString();
        health.text = playerStats.health.ToString();
        ping.text = playerStats.ping.ToString();
    }
}
