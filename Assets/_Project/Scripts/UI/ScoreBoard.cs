using System;
using System.Collections.Generic;
using Project;
using TMPro;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Serialization;
using Event = Project.Event;

public class ScoreBoard : MonoBehaviour
{
    // private PlayerStats _playerStats;
    //
    TextMeshProUGUI _name;
    TextMeshProUGUI _kill;
    TextMeshProUGUI _assist;
    TextMeshProUGUI _death;
    TextMeshProUGUI _score;
    TextMeshProUGUI _health;
    TextMeshProUGUI _ping;

    private readonly Dictionary<ulong, PlayerScoreboardUI> playerScoreboard = new Dictionary<ulong, PlayerScoreboardUI>();
    
    [SerializeField] private Transform _bodyPlayersScoreboard; 
    [SerializeField] private PlayerScoreboardUI _playerScoreboardUiTemplate;
    [SerializeField] private GameObject scoreboard;
    bool isOpen; 

    // private void OnEnable()
    // {
    //     _name.text = _playerStats.name;
    //     _kill.text = _playerStats.kill.ToString();
    //     _death.text = _playerStats.death.ToString();
    //     _score.text = _playerStats.score.ToString();
    //     _health.text = _playerStats.health.ToString();
    //     _ping.text = _playerStats.ping.ToString();
    //
    // }
    // private void Awake()
    // {
    //     //Instantiation
    //     _playerStats = transform.root.GetComponent<PlayerStats>();
    //
    //     _name = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    //     _kill = gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    //     _death = gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
    //     _score = gameObject.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
    //     _health = gameObject.transform.GetChild(4).GetComponent<TextMeshProUGUI>();
    //     _ping = gameObject.transform.GetChild(5).GetComponent<TextMeshProUGUI>();   
    // }

    private void Awake()
    {
        GameEvent.onPlayerJoinGameEvent.Subscribe(AddPlayerToTheScoreboard, this);
        
        GameEvent.onPlayerUpdateNameEvent.Subscribe(UpdateNameText, this);
        GameEvent.onPlayerUpdateColorEvent.Subscribe(UpdateColorImage, this);
        GameEvent.onPlayerGetAKillEvent.Subscribe(UpdateKillText, this);
        GameEvent.onPlayerGetAssistEvent.Subscribe(UpdateAssistText, this);
        GameEvent.onPlayerDiedEvent.Subscribe(UpdateDeathText, this);
        GameEvent.onPlayerScoreEvent.Subscribe(UpdateScoreText, this);

        Player[] players = GameManager.instance.GetPlayers();
        for (int i = 0; i < players.Length; i++)
        {
            Player player = players[i];
            ulong playerId = player.OwnerClientId;
            AddPlayerToTheScoreboard(playerId);
            UpdateNameText(playerId, player.playerName);
            UpdateKillText(playerId, player.kills);
            UpdateAssistText(playerId, player.assists);
            UpdateDeathText(playerId, player.deaths);
            UpdateColorImage(playerId, player.playerColor);
            UpdateScoreText(playerId, player.score); 
        }
    }

    private void OnDestroy()
    {
        GameEvent.onPlayerJoinGameEvent.Unsubscribe(AddPlayerToTheScoreboard);
        
        GameEvent.onPlayerUpdateNameEvent.Unsubscribe(UpdateNameText);
        GameEvent.onPlayerUpdateColorEvent.Unsubscribe(UpdateColorImage);
        GameEvent.onPlayerGetAKillEvent.Unsubscribe(UpdateKillText);
        GameEvent.onPlayerGetAssistEvent.Unsubscribe(UpdateAssistText);
        GameEvent.onPlayerDiedEvent.Unsubscribe(UpdateDeathText);
        GameEvent.onPlayerScoreEvent.Unsubscribe(UpdateScoreText);
    }

    private void Update()
    {
        if (InputManager.instance.TabPressed)
        {
            scoreboard.SetActive(!isOpen);
            isOpen = !isOpen;
            InputManager.instance.TabPressed = false;
        }
    }
    public void AddPlayerToTheScoreboard(ulong playerId)
    {
        PlayerScoreboardUI playerScoreboardUi = Instantiate(_playerScoreboardUiTemplate, _bodyPlayersScoreboard);
        playerScoreboard.Add(playerId, playerScoreboardUi);
        playerScoreboardUi.gameObject.SetActive(true);
        
    }

    public void RemovePlayerToTheScoreboard(ulong playerId)
    {
        PlayerScoreboardUI playerScoreboardUI = playerScoreboard[playerId];
        Destroy(playerScoreboardUI);
        playerScoreboard.Remove(playerId);
    }

    private void UpdateNameText(ulong playerId, StringNetwork newValue)
    {
        Debug.Log("UpdateNameText playerId : " + playerId);
        Debug.Log("UpdateNameText newValue : " + newValue.value);
        playerScoreboard[playerId].UpdateNameText(newValue.value);
    }
    private void UpdateNameText(ulong playerId, string newValue)
    {
        Debug.Log("UpdateNameText playerId : " + playerId);
        Debug.Log("UpdateNameText newValue : " + newValue);
        playerScoreboard[playerId].UpdateNameText(newValue);
    }

    private void UpdateKillText(ulong playerId, int newValue) => playerScoreboard[playerId].UpdateKillText(newValue);
    private void UpdateDeathText(ulong playerId, int newValue) => playerScoreboard[playerId].UpdateDeathText(newValue);
    private void UpdateAssistText(ulong playerId, int newValue) => playerScoreboard[playerId].UpdateAssistText(newValue);
    private void UpdateColorImage(ulong playerId, Color newValue) => playerScoreboard[playerId].UpdateColorImage(newValue);
    private void UpdateScoreText(ulong playerId, int newValue) => playerScoreboard[playerId].UpdateScoreText(newValue);
}
