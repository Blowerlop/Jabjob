using System.Threading.Tasks;
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
            GameScene
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


        public static async void LoadSceneAsync(EScene sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
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
        #endregion
    }
}