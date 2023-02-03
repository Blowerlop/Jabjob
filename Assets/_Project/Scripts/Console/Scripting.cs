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

        void Awake()
        {
            if(instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(this);
                instance = this;
            }
        }

        void Start()
        {
            RegisterCommand(Example, "example");
        }

        void RegisterCommand(Action<string[]> newCommandAction, string commandName)
        {
            commands.Add(commandName, newCommandAction);
        }

        public void CallCommand(string command, string[] args)
        {
            commands[command].Invoke(args);
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
    }
}
