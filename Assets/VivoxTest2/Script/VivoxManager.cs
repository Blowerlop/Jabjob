using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine;
using VivoxUnity;
using Unity.Services.Vivox;
using System;
using UnityEngine.Events;
using System.Linq;
using Project;
using System.Globalization;
using System.Threading.Tasks;

public class VivoxManager : MonoBehaviour
{
    private const string ChannelName = "My_super_channel";
    private ILoginSession LoginSession;
    private Client _client => VivoxService.Instance.Client;
    private IChannelSession _currentChannelSession;
    private string displayName;
    private LobbyManager _lobbyManager;
    public bool isConnected = false;

    public event Action OnUserLoggedIn;
    public Action<string, IChannelTextMessage, string> OnTextMessageLogReceived;
    public static VivoxManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        SubscribeLobbyEvent();
        GameEvent.onGameFinishedEvent.Subscribe(VivoxLogOut, this); 

    }
    public void SubscribeLobbyEvent()
    {
        _lobbyManager = FindObjectOfType<LobbyManager>();
        if (_lobbyManager == null) return;


        _lobbyManager.VivoxOnAuthenticate += InitAndLoginVivox; 
        _lobbyManager.VivoxOnCreateLobby += VivoxOnCreateLobby;
        _lobbyManager.VivoxOnJoinLobby += VivoxOnJoinLobby;
        _lobbyManager.VivoxOnLeaveLobby += VivoxOnLobbyLeave;
        _lobbyManager.VivoxOnLoginOnly += LogInViVoxOnly; 
    }
    private void VivoxOnLobbyLeave()
    {
        LeaveChannel();
    }


    private void VivoxOnJoinLobby(string channelName)
    {
        JoinChannel(channelName);
    }

    private void VivoxOnCreateLobby(string channelName)
    {
        JoinChannel(channelName);
    }

    private void InitAndLoginVivox(string playerName)
    {
        VivoxService.Instance.Initialize();
        Login(playerName);
    }

    private void LogInViVoxOnly(string playerName)
    {
        Login(playerName);
    }

   


    public void Login(string displayName)
    {
        
        Account account = new Account(displayName);
        LoginSession = _client.GetLoginSession(account);
        LoginSession.PropertyChanged += LoginSession_PropertyChanged;
        this.displayName = displayName;
        LoginSession.BeginLogin(LoginSession.GetLoginToken(), SubscriptionMode.Accept, null, null, null, ar =>
        {
            try
            {
                LoginSession.EndLogin(ar);
                isConnected = true; 
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return;
            }
        });

    }
    public void VivoxLogOut()
    {
        LeaveChannel();
        LoginSession.Logout();
        isConnected = false;
    }
    private void LoginSession_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {

        var logginSession = (ILoginSession)sender;
        switch (logginSession.State)
        {
            case LoginState.LoggedOut:
                VivoxLog("Logged Out");
                isConnected = false;
                break;
            case LoginState.LoggedIn:
                VivoxLog("Logged in");
                OnUserLoggedIn?.Invoke();
                //JoinChannel(ChannelName);
                break;
            case LoginState.LoggingIn:
                VivoxLog("Loggin in");
                break;
            case LoginState.LoggingOut:
                break;
            default:
                VivoxLog("Default");
                break;
        }
    }




    public void SendMessageVivox(string message)
    {
        if (string.IsNullOrEmpty(message))
            return;

        //var channelId = LoginSession?.ChannelSessions.FirstOrDefault(ac => ac.Channel.Name == ChannelName).Key;
        //var channelSession = LoginSession.GetChannelSession(channelId);


        VivoxLog(name);
        _currentChannelSession.BeginSendText(message, ar =>
        {
            try
            {
                _currentChannelSession.EndSendText(ar);

            }
            catch (Exception)
            {
                throw;
            }
        });

    }


    public void JoinChannel(string channelName)
    {
        if (LoginSession.State == LoginState.LoggedIn)
        {
            Channel channel = new Channel(channelName);

            _currentChannelSession = LoginSession.GetChannelSession(channel);
            _currentChannelSession.MessageLog.AfterItemAdded += OnMessageLogReceive;

            _currentChannelSession.BeginConnect(false, true, true, _currentChannelSession.GetConnectToken(), ar =>
            {
                try
                {
                    _currentChannelSession.EndConnect(ar);
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                    throw;
                }
            });
            VivoxLog("ChannelJoined : " + channelName);
        }
    }

    public void LeaveChannel()
    {
        if (_currentChannelSession == null)
            return;

        _currentChannelSession.Disconnect();
        _currentChannelSession = null;
        VivoxLog("Channel quit");
    }

    private void OnMessageLogReceive(object sender, QueueItemAddedEventArgs<IChannelTextMessage> textMessage)
    {

        IChannelTextMessage message = textMessage.Value;

        //GetColor
        string playerName = message.Sender.DisplayName;
        string color = string.Empty;

        foreach (var player in _lobbyManager.joinedLobby.Players)
        {
            if (playerName == player.Data[LobbyManager.KEY_PLAYER_NAME].Value)
            {
                color = player.Data[LobbyManager.KEY_PLAYER_COLOR].Value;
                break;
            }
        }

        OnTextMessageLogReceived?.Invoke(message.Sender.DisplayName, message, color);
    }

    private void VivoxLog(object message)
    {
        Debug.Log("<color=yellow>Vivox : " + message + "</color>");
    }


}