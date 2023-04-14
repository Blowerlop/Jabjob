using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    public class Console : MonoBehaviour
    {
        [Header("Console Component")]
        [SerializeField] TMPro.TMP_Text consoleText;
        [SerializeField] TMPro.TMP_InputField consoleIF;
        [SerializeField] RectTransform content;
        [SerializeField] ScrollRect scroll;

        [HideInInspector] public static Console Instance;

        bool follow = true;
        bool unityMessages = false;

        List<string> blackList = new List<string> 
        {
            "[Netcode] Syncing Time To Clients",
            "Painting",
            "Physics Projectile (Project.OnTriggerEnterEventClass) invoked event",
            "Init",
            "Shoot",
            "Method - OnTriggerEnter - has subscribed",
        };

        // Start is called before the first frame update
        void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            Application.logMessageReceived += NewLog;
        }

        public void OpenConsole()
        {
            gameObject.SetActive(!gameObject.activeSelf);
            if (gameObject.activeSelf) InputManager.instance.SwitchPlayerInputMap("UI");
            else InputManager.instance.SwitchPlayerInputMap("Player"); 
        }

        public void ExecuteCommand(string command)
        {
            if (command == "")
                return;
            consoleIF.text = "";
            Scripting.instance.CallCommand(command.Split(" ")[0], command.Split(" "));
        }

        void NewLog(string logString, string stackTrace, LogType type)
        {
            if (!unityMessages) return;
            if (CheckIfInList(logString)) return;
            string balise = "";
            switch (type)
            {
                case LogType.Log:
                    balise = "<color=green>";
                    break;
                case LogType.Warning:
                    balise = "<color=yellow>";
                    break;
                case LogType.Error:
                    balise = "<color=red>";
                    break;
                default:
                    balise = "<color=purple>";
                    break;
            }
            consoleText.text += balise + logString.Split("\n")[0] + (stackTrace.Split("\n").Length >= 2 ? "\n" + stackTrace.Split("\n")[1] + "\n" : "") + "</color>";
            //if (content != null) content.sizeDelta = new Vector2(content.sizeDelta.x, content.sizeDelta.y + 29 * (stackTrace.Split("\n").Length >= 2 ? 2 : 1));
            if (stackTrace.Split("\n").Length == 1)
            {
                consoleText.text += "\n";
            }
            UpdateBarPos();
        }
        public void OnApplicationQuit()
        {
            Application.logMessageReceived -= NewLog;
        }

        public void OnGUI()
        {
            UpdateBarPos();
        }

        void UpdateBarPos()
        {
            if (follow)
            {
                Canvas.ForceUpdateCanvases();
                if (scroll != null) scroll.verticalNormalizedPosition = 0;
            }
        }

        public void ToggleFollow(bool valueFollow)
        {
            follow = valueFollow;
        }

        public void ToggleUnityDebug(bool value)
        {
            unityMessages = value;
        }

        public bool CheckIfInList(string logToCheck)
        {
            return blackList.Where(x => logToCheck.Contains(x)).Count() != 0;
        }
    }
}
