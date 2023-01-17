using TMPro;
using UnityEngine;
using Unity.Netcode;

public class ScoreBoard : MonoBehaviour
{

    private PlayerStats _playerStats;

    TextMeshProUGUI _name;
    TextMeshProUGUI _kill;
    TextMeshProUGUI _death;
    TextMeshProUGUI _score;
    TextMeshProUGUI _health;
    TextMeshProUGUI _ping;

    private void OnEnable()
    {
        _name.text = _playerStats.name;
        _kill.text = _playerStats.kill.ToString();
        _death.text = _playerStats.death.ToString();
        _score.text = _playerStats.score.ToString();
        _health.text = _playerStats.health.ToString();
        _ping.text = _playerStats.ping.ToString();
    
    }
    private void Awake()
    {
        //Instantiation
        _playerStats = transform.root.GetComponent<PlayerStats>();

        _name = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _kill = gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        _death = gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        _score = gameObject.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        _health = gameObject.transform.GetChild(4).GetComponent<TextMeshProUGUI>();
        _ping = gameObject.transform.GetChild(5).GetComponent<TextMeshProUGUI>();   
    }   


}
