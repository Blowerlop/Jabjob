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
        bool follow = true;
        // Start is called before the first frame update
        void Awake()
        {
            Application.logMessageReceived += NewLog;
            content.sizeDelta = new Vector2(content.sizeDelta.x, 0);
            NewLog("AAAA", "", LogType.Exception);
            Debug.Log("eee");
            Debug.Log("eee");
            NewLog("AAAA", "", LogType.Exception);
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void OpenConsole()
        {
            gameObject.SetActive(!gameObject.activeSelf);
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
            consoleText.text += balise + logString.Split("\n")[0] + (stackTrace.Split("\n").Length >= 2? "\n" + stackTrace.Split("\n")[1] + "\n" : "")+ "</color>";
            content.sizeDelta = new Vector2(content.sizeDelta.x, content.sizeDelta.y + 29 * (stackTrace.Split("\n").Length >= 2? 2: 1));
            if(stackTrace.Split("\n").Length == 1)
            {
                consoleText.text += "\n";
            }
            UpdateBarPos();
        }
        public void OnApplicationQuit()
        {
            Application.logMessageReceived -= NewLog;
        }

        void UpdateBarPos()
        {
            if (follow)
            {
                Canvas.ForceUpdateCanvases();
                scroll.verticalNormalizedPosition = 0;
            }
        }

        public void ToggleFollow(bool valueFollow)
        {
            follow = valueFollow;
        }
    }
}
