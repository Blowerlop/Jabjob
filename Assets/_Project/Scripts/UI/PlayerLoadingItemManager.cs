using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using SceneManager = _Project.Scripts.Managers.SceneManager;

namespace Project
{
    public class PlayerLoadingItemManager : MonoBehaviour
    {

        public static PlayerLoadingItemManager Instance { get; private set; }
        [SerializeField] PlayerLoadingItem[] PlayerList;
        [SerializeField] Image MapBackground;
        [SerializeField] Image MiniMapBackground; 
        [SerializeField] TextMeshProUGUI mapTitle;
        [SerializeField] Slider loadSlider; 

        public GameMapsUI gameMapsUI;
        private void Awake()
        {
            Instance = this;
        }

        public void OnEnable()
        {
            SceneManager.onSceneLoadEvent.Subscribe(UpdateLoadingSLider, this);
        }
        public void OnDisable()
        {
            SceneManager.onSceneLoadEvent.Unsubscribe(UpdateLoadingSLider);
        }

        public void UpdateLoadingScreen(Lobby lobby) 
        {
            List<Unity.Services.Lobbies.Models.Player> lobbyPlayers = lobby.Players; 

            int i = 0;
            while (i < lobbyPlayers.Count)
            {
                PlayerLoadingItem loadingItemUI = PlayerList[i];
                Unity.Services.Lobbies.Models.Player player = lobbyPlayers[i];
                loadingItemUI.SetName(player.Data[LobbyManager.KEY_PLAYER_NAME].Value);
                loadingItemUI.SetModel(player.Data[LobbyManager.KEY_PLAYER_CHARACTER].Value);
                Color color;
                if (ColorUtility.TryParseHtmlString("#" + player.Data[LobbyManager.KEY_PLAYER_COLOR].Value, out color)) loadingItemUI.SetColor(color);
                loadingItemUI.gameObject.SetActive(true);
                i++;
            }
            while (i < PlayerList.Length)
            {
                PlayerList[i].gameObject.SetActive(false);
                i++;
            }

            mapTitle.text =  gameMapsUI.GetMapName(lobby.Data[LobbyManager.KEY_GAMEMAP_NAME].Value);
            MapBackground.sprite = gameMapsUI.GetMapSprite(lobby.Data[LobbyManager.KEY_GAMEMAP_NAME].Value);
            MiniMapBackground.sprite = gameMapsUI.GetMiniMapSprite(lobby.Data[LobbyManager.KEY_GAMEMAP_NAME].Value);
        }

        private async void UpdateLoadingSLider(ulong clientId, string sceneName, LoadSceneMode loadSceneMode, AsyncOperation asyncOperation)
        {
            asyncOperation.allowSceneActivation = false;
            do
            {
                await Task.Delay(100);
                float loadingPercentage = asyncOperation.progress / 0.9f * 100.0f;
                if (loadSlider != null) loadSlider.value = loadingPercentage / 100f;
            } while (asyncOperation.progress < 0.9f);

            await Task.Delay(100);
            asyncOperation.allowSceneActivation = true;
        }
    }
}
