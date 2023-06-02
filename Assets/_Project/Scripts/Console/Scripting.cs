using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Project;
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
            RegisterCommand(Damage, "damage");
            RegisterCommand(KillPlayer, "kill_player");
            RegisterCommand(KickPlayer, "kick_player");
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
        
        private static readonly Action<string[]> Damage = (args) =>
        {
            try
            {
                Player pla = GameManager.instance.GetPlayers().First(x => x.playerName == args[1]);
                if (pla != null)
                {
                    pla.Damage(Convert.ToInt32(args[2]),pla.OwnerClientId);
                }
                else
                {
                    Debug.LogError("No such player " + args[1]);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        };
        
        private static readonly Action<string[]> KillPlayer = (args) =>
        {
            try
            {
                Player pla = GameManager.instance.GetPlayers().First(x => x.playerName == args[1]);
                if (pla != null)
                {
                    pla.Damage(1000,pla.OwnerClientId);
                }
                else
                {
                    Debug.LogError("No such player " + args[1]);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        };
        
        private static readonly Action<string[]> KickPlayer = (args) =>
        {
            try
            {
                Player pla = GameManager.instance.GetPlayers().First(x => x.playerName == args[1]);
                pla.Kick();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        };
    }
}
