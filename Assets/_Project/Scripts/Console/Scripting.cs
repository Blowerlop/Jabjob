using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Project;
using Project.Utilities;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using SceneManager = _Project.Scripts.Managers.SceneManager;

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
            RegisterCommand(ShowPlayer, "show_player");
            RegisterCommand(SetKills, "set_kills");
            RegisterCommand(ChangeLevel, "change_level");
            RegisterCommand(StartGame, "startGame");
            RegisterCommand(ShowUI, "show_ui");
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
        
        private static readonly Action<string[]> ShowPlayer = (args) =>
        {
            float alpha = args.Length < 2 ? 0.45f :Convert.ToInt32(args[2]) / 100f;
            try
            {
                Player pla = GameManager.instance.GetPlayers().First(x => x.playerName == args[1]);
                pla.gameObject.GetComponentInChildren<Paintable>().SetAlpha(alpha);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        };
        
        private static readonly Action<string[]> SetKills = (args) =>
        {
            try
            {
                Player pla = GameManager.instance.GetPlayers().First(x => x.playerName == args[1]);
                if (int.TryParse(args[2], out int result))
                {
                    pla.SetKills(result);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        };

        private static readonly Action<string[]> ChangeLevel = (args) =>
        {
            try
            {
                if (int.TryParse(args[1], out int result))
                {
                    // Local level change
                    if (result == 0)
                    {
                        NetworkManager.Singleton.Shutdown(); 
                        SceneManager.LoadSceneAsyncLocal(args[2]);
                    }
                    else if (result == 1)
                    {
                        NetworkManager.Singleton.SceneManager.LoadScene(args[2], LoadSceneMode.Single);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        };
        
        private static readonly Action<string[]> StartGame = (args) =>
        {
            try
            {
                GameManager.instance.StartGameClientRpc();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        };
        
        private static readonly Action<string[]> ShowUI = (args) =>
        {
            try
            {
                if (int.TryParse(args[1], out int result))
                {
                    var uiToToggle = GameObject.FindObjectsOfType<UIToggle>(true);
                    
                    if (result == 0)
                    {
                        uiToToggle.ForEach(x => x.gameObject.SetActive(false));
                    }
                    else if (result == 1)
                    {
                        uiToToggle.ForEach(x => x.gameObject.SetActive(true));
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        };
    }
}
