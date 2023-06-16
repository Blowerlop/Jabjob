using System.Collections;
using System.Collections.Generic;
using Project;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using SceneManager = _Project.Scripts.Managers.SceneManager;

public class MenuUI : MonoBehaviour
{
    public void QuitGame()
    {
        if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
        {
            GameManager.instance.AskToBeDisconnected(NetworkManager.Singleton.LocalClientId);
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += (sceneName, mode, completed, @out) =>
            {
                VivoxManager.Instance.VivoxLogOut();
                NetworkManager.Singleton.Shutdown();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                SoundManager2D.instance.PlayBackgroundMusic("Start Scene Background Music");
                VivoxManager.Instance.SubscribeLobbyEvent();
            };

            SceneManager.LoadSceneNetwork("MenuScene");
            //SceneManager.LoadSceneAsyncLocal("MenuScene", LoadSceneMode.Single);
            Debug.Log("Leave Game");
        }
        else
        {
            Application.Quit();
            Debug.Log("Quit application");
        }
        
    }
}
