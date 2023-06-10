using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using SceneManager = _Project.Scripts.Managers.SceneManager;

namespace Project
{
    public class OnSceneLoad_LoadingEventListener : MonoBehaviour
    {
        public void Start()
        {
            SceneManager.onSceneLoadEvent.Subscribe(EnabledGameObject, this);
            SceneManager.onSceneLoadCompleteEvent.Subscribe(DisableGameObject, this);
        }

        

        public void OnDestroy()
        {
            SceneManager.onSceneLoadEvent.Unsubscribe(EnabledGameObject);
            SceneManager.onSceneLoadCompleteEvent.Unsubscribe(DisableGameObject);
        }

        private void EnabledGameObject(ulong clientId, string sceneName, LoadSceneMode loadSceneMode, AsyncOperation asyncOperation) { transform.GetChild(0).gameObject.SetActive(true); }

        private void DisableGameObject(ulong id, string sceneName, LoadSceneMode mode) =>
            transform.GetChild(0).gameObject.SetActive(false);
    }
}
