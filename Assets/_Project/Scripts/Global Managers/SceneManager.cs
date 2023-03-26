using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Project.Scripts.Managers
{
    public class SceneManager : MonoBehaviour
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
            if (loadSceneMode == LoadSceneMode.Additive)
            {
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName.ToString(), loadSceneMode);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(EScene.LoadingScene.ToString());

                AsyncOperation newScene =
                    UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName.ToString(), loadSceneMode);

                newScene.allowSceneActivation = false;
                do
                {
                    await Task.Delay(100);
                    float loadingPercentage = newScene.progress / 0.9f * 100.0f;
                    loadSlider.value = loadingPercentage / 100f;

                } while (newScene.progress < 0.9f);

                await Task.Delay(100);
                newScene.allowSceneActivation = true;
            }
        }

        public static void LoadSceneNetwork(EScene scene)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Single);
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

        [ServerRpc]
        private static void LoadSceneAsyncClientRpc(EScene sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            LoadSceneAsyncLocal(sceneName, loadSceneMode);
        }
        #endregion
    }
}