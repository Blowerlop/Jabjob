using System.Collections;
using System.Collections.Generic;
using Project;
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
            // Cursor.lockState = actualConsole.activeSelf? CursorLockMode.Locked: CursorLockMode.None;
            // Cursor.visible = !actualConsole.activeSelf;

            actualConsole.SetActive(!actualConsole.activeSelf);
            
            if (actualConsole.activeSelf)
            {
                CursorManager.instance.ApplyNewCursor(new CursorState(CursorLockMode.Confined, "UI"));
            }
            else
            {
                CursorManager.instance.Revert();
            }
            
            
            
        }
        void OnApplicationQuit()
        {
            SceneManager.sceneLoaded -= InstanciateConsole;
        }
    }
}
