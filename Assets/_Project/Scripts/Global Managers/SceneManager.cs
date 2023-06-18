using System.Threading.Tasks;
using Project;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Event = Project.Event;

namespace _Project.Scripts.Managers
{
    public static class SceneManager
    {
        #region Variables

        public enum EScene
        {
            MenuScene,
            LoadingScene,
            GameScene,
            Multi_Lobby
        }

        public static Slider loadSlider;
        public static readonly Event<ulong, string, LoadSceneMode, AsyncOperation> onSceneLoadEvent = new Event<ulong, string, LoadSceneMode, AsyncOperation>(nameof(onSceneLoadEvent));
        public static readonly Event<ulong, string, LoadSceneMode> onSceneLoadCompleteEvent = new Event<ulong, string, LoadSceneMode>(nameof(onSceneLoadCompleteEvent));

        #endregion

        #region Methods

        // public static async void LoadSceneAsync(EScene sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        // {
        //     if (loadSceneMode == LoadSceneMode.Additive)
        //     {
        //         UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName.ToString(), loadSceneMode);
        //     }
        //     else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != EScene.LoadingScene.ToString())
        //     {
        //         AsyncOperation loadingScene =
        //             UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(EScene.LoadingScene.ToString());
        //         loadingScene.completed += (e) => LoadSceneAsync(sceneName, loadSceneMode);
        //     }
        //     else
        //     {
        //         AsyncOperation newScene =
        //             UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName.ToString(), loadSceneMode);
        //         
        //         newScene.allowSceneActivation = false;
        //         do
        //         {
        //             await Task.Delay(100);
        //             float loadingPercentage = newScene.progress / 0.9f * 100.0f;
        //             //loadText.text = loadingPercentage + "%";
        //             
        //             
        //             
        //         } while (newScene.progress < 0.9f);
        //         
        //         await Task.Delay(100);
        //         newScene.allowSceneActivation = true;
        //     }
        //}
 

        public static async void LoadSceneAsyncLocal(EScene sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            LoadSceneAsyncLocal(sceneName.ToString(), loadSceneMode);
        }
        
        public static async void LoadSceneAsyncLocal(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            if (loadSceneMode == LoadSceneMode.Additive)
            {
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName.ToString(), loadSceneMode);
            }
            else
            {
                

                AsyncOperation newScene =
                    UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName.ToString(), loadSceneMode);

                InvokeOnSceneLoad(0, sceneName.ToString(), loadSceneMode, newScene); 

                newScene.allowSceneActivation = false;
                do
                {
                    await Task.Delay(100);
                    float loadingPercentage = newScene.progress / 0.9f * 100.0f;
                    if (loadSlider != null) loadSlider.value = loadingPercentage / 100f;
                } while (newScene.progress < 0.9f);

                await Task.Delay(100);
                newScene.allowSceneActivation = true;
                InvokeOnSceneLoadComplete(0, sceneName.ToString(), loadSceneMode); 
            }
        }

        public static void LoadSceneNetwork(EScene scene)
        {
            var a = NetworkManager.Singleton.SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Single);
            // NetworkManager.Singleton.SceneManager.OnLoadComplete += (id, sceneName, mode) =>
                // UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(EScene.LoadingScene.ToString());
            // NetworkManager.Singleton.SceneManager.LoadScene(EScene.LoadingScene.ToString(), LoadSceneMode.Single);
        }
        public static void LoadSceneNetwork(string scene)
        {
            NetworkManager.Singleton.SceneManager.OnLoad += InvokeOnSceneLoad;
            NetworkManager.Singleton.SceneManager.OnLoadComplete += InvokeOnSceneLoadComplete;
            
            SceneEventProgressStatus sceneEventProgressStatus = NetworkManager.Singleton.SceneManager.LoadScene(scene, LoadSceneMode.Single);
            
            if (sceneEventProgressStatus == SceneEventProgressStatus.Started)
            {
                Debug.Log($"The {scene} has successfully loaded");
            }
            else
            {
                Debug.LogError($"Failed to load {scene} " +
                        $"with a {nameof(SceneEventProgressStatus)}: {sceneEventProgressStatus.ToString()}");
            }
            
            
            // NetworkManager.Singleton.SceneManager.OnLoad -= InvokeOnSceneLoadClientRpc;
            // NetworkManager.Singleton.SceneManager.OnLoadComplete -= InvokeOnSceneLoadCompleteClientRpc;
            
            
            // if (sceneEventProgressStatus == SceneEventProgressStatus.Started)
            // {
            //     // NetworkManager.Singleton.SceneManager.OnLoadComplete
            //     // Debug.Log($"The {scene} has successfully started loading");
            //     // NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += (sceneName, mode, completed, @out) =>
            //     //     Debug.Log("On Load Event Completed : " +
            //     //               $"sceneName : {sceneName}" +
            //     //               $"Mode : {mode}" +
            //     //               $"Completed : {completed}" +
            //     //               $"@out : {@out}");
            //     
            //     var eventProgressStatus = NetworkManager.Singleton.SceneManager.LoadScene("Multi_Lobby", LoadSceneMode.Additive);
            //     if (eventProgressStatus == SceneEventProgressStatus.SceneEventInProgress)
            //     {
            //         
            //         while (true)
            //         {
            //             var progress = NetworkManager.Singleton.SceneManager.LoadScene("Multi_Lobby", LoadSceneMode.Additive);
            //             Debug.Log($"Event Status : {progress.ToString()}");
            //
            //             if (progress == SceneEventProgressStatus.Started)
            //             {
            //                 break;
            //             }
            //             else if (progress == SceneEventProgressStatus.SceneEventInProgress)
            //             {
            //                 
            //             }
            //             else
            //             {
            //                 Debug.LogError($"Failed to load Multi_Lobby" +
            //                                $"with a {nameof(SceneEventProgressStatus)}: {progress.ToString()}");
            //             }
            //             // NetworkManager.Singleton.SceneManager.LoadScene("Multi_Lobby", LoadSceneMode.Single);
            //             await Task.Delay(100);
            //         }
            //     }
            //
            //     
            //     // NetworkManager.Singleton.SceneManager.LoadScene("Multi_Lobby", LoadSceneMode.Single);
            // }
            // else
            // {
            //     Debug.LogError($"Failed to load {scene} " +
            //                      $"with a {nameof(SceneEventProgressStatus)}: {sceneEventProgressStatus.ToString()}");
            // }
            // NetworkManager.Singleton.SceneManager.OnLoadComplete += (id, sceneName, mode) =>
            // UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(EScene.LoadingScene.ToString());
            // NetworkManager.Singleton.SceneManager.LoadScene(EScene.LoadingScene.ToString(), LoadSceneMode.Single);
        }
        
        [ServerRpc]
        public static void LoadSceneAsyncNetworkServerRpc(EScene sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            LoadSceneAsyncClientRpc(sceneName, loadSceneMode);
            // if (loadSceneMode == LoadSceneMode.Additive) 
            // {
            //     NetworkManager.Singleton.SceneManager.LoadScene(sceneName.ToString(), loadSceneMode);
            // }
            // else
            // {
            //     UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(EScene.LoadingScene.ToString());
            //
            //     // AsyncOperation newScene =
            //     //     NetworkManager.Singleton.SceneManager.LoadScene(sceneName.ToString(), loadSceneMode);
            //
            //     newScene.allowSceneActivation = false;
            //     do
            //     {
            //         await Task.Delay(100);
            //         float loadingPercentage = newScene.progress / 0.9f * 100.0f;
            //         loadSlider.value = loadingPercentage / 100f;
            //
            //     } while (newScene.progress < 0.9f);
            //
            //     await Task.Delay(100);
            //     newScene.allowSceneActivation = true;
            // }
        }

        [ClientRpc]
        private static void LoadSceneAsyncClientRpc(EScene sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            LoadSceneAsyncLocal(sceneName, loadSceneMode);
        }
        
        private static void InvokeOnSceneLoad(ulong clientId, string sceneName, LoadSceneMode loadSceneMode, AsyncOperation asyncOperation)
        {
            onSceneLoadEvent.Invoke(nameof(SceneManager), true, clientId, sceneName, loadSceneMode, asyncOperation);

            if (NetworkManager.Singleton != null && NetworkManager.Singleton.SceneManager != null)
            {
                NetworkManager.Singleton.SceneManager.OnLoad -= InvokeOnSceneLoad;
            }
        }
        
        
        private static void InvokeOnSceneLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
        {
            onSceneLoadCompleteEvent.Invoke(nameof(SceneManager), true, clientId, sceneName, loadSceneMode);

            if (NetworkManager.Singleton != null && NetworkManager.Singleton.SceneManager != null)
            {
                NetworkManager.Singleton.SceneManager.OnLoadComplete -= InvokeOnSceneLoadComplete;
            }
        }
        #endregion
    }
}