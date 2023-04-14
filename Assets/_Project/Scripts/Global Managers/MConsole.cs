using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class MConsole : MonoBehaviour
    {
        [SerializeField] GameObject ConsolePrefab;
        GameObject actualConsole;
        // Start is called before the first frame update
        void Start()
        {
            SceneManager.sceneLoaded += InstanciateConsole;
            InstanciateConsole(SceneManager.GetActiveScene(),LoadSceneMode.Single);
            InputManager.instance.openCommand.AddListener(OpenConsole);
        }

        void InstanciateConsole(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "MenuScene")
            {
                Utils.Scripting.actualSceneType = Utils.Scripting.SceneType.UI;
            }
            if (scene.name == "GameScene")
            {
                Utils.Scripting.actualSceneType = Utils.Scripting.SceneType.GamePlay;
            }
            actualConsole = Instantiate(ConsolePrefab);
            actualConsole.SetActive(false);
        }

        void OpenConsole()
        {
            actualConsole.SetActive(!actualConsole.activeSelf);
            if (actualConsole.activeSelf) InputManager.instance.SwitchPlayerInputMap("UI");
            else InputManager.instance.SwitchPlayerInputMap("Player"); 
            
            
        }
        void OnApplicationQuit()
        {
            SceneManager.sceneLoaded -= InstanciateConsole;
        }
    }
}
