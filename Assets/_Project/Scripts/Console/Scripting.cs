using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Utils
{
    public class Scripting : MonoBehaviour
    {
        public static Scripting instance;
        Dictionary<string, Action<string[]>> commands = new();

        public enum SceneType
        {
            UI,
            GamePlay
        }

        public static SceneType actualSceneType = SceneType.GamePlay;

        void Awake()
        {
            if(instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
            }
        }

        void Start()
        {
            RegisterCommand(Example, "example");
            RegisterCommand(ShowDebug, "show_debug");
        }

        void RegisterCommand(Action<string[]> newCommandAction, string commandName)
        {
            commands.Add(commandName, newCommandAction);
        }

        public void CallCommand(string command, string[] args)
        {
            try
            {
                commands[command].Invoke(args);
            }catch(Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        private static readonly Action<string[]> Example = (args) =>
        {
            string printValue = "Call: Example Args: ";
            foreach(string str in args)
            {
                printValue += str + "/";
            }
            Debug.Log(printValue);
        };

        private static readonly Action<string[]> ShowDebug = (args) =>
        {
            Console.Instance.ToggleUnityDebug(Convert.ToBoolean(args[1]));
        };
    }
}
