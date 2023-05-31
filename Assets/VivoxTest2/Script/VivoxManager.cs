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

public class VivoxManager : MonoBehaviour
{
    private const string ChannelName = "My_super_channel";
    public ILoginSession LoginSession;
    private Client _client => VivoxService.Instance.Client;
    private IChannelSession _currentChannelSession;
    public event UnityAction OnUserLoggedIn;
    public event UnityAction<string, IChannelTextMessage> OnTextMessageLogReceived;
    private string displayName;

    private LobbyManager _lobbyManager;

    private void Awake()
    {
        _lobbyManager = FindObjectOfType<LobbyManager>();
        _lobbyManager.VivoxOnAuthenticate += InitAndLoginVivox;
        _lobbyManager.VivoxOnCreateLobby += VivoxOnCreateLobby;
        _lobbyManager.VivoxOnJoinLobby += VivoxOnJoinLobby;
        _lobbyManager.VivoxOnLeaveLobby += VivoxOnLobbyLeave;
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


    //private async void Start()
    //{
    //    InitializationOptions options = new InitializationOptions();
    //    options.SetProfile("Getet" + UnityEngine.Random.Range(0, 650));
    //    await UnityServices.InitializeAsync(options);
    //    await AuthenticationService.Instance.SignInAnonymouslyAsync();
    //    VivoxService.Instance.Initialize();

    //}


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
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return;
            }
        });

    }

    private void LoginSession_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        

        var logginSession = (ILoginSession)sender;
        switch (logginSession.State)
        {
            case LoginState.LoggedOut:
                VivoxLog("Logged Out");
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

        var channelId = LoginSession?.ChannelSessions.FirstOrDefault(ac => ac.Channel.Name == ChannelName).Key;
        var channelSession = LoginSession.GetChannelSession(channelId);

        channelSession.BeginSendText(message, ar =>
        {
            try
            {
                channelSession.EndSendText(ar);
            }
            catch (Exception)
            {
                throw;
            }
        });

    }


    public void JoinChannel(string channelName)
    {
        if(LoginSession.State == LoginState.LoggedIn)
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
                catch (Exception e )
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
        Debug.Log("Message received");
        IChannelTextMessage message = textMessage.Value;
        OnTextMessageLogReceived?.Invoke(message.Sender.DisplayName, message);
    }

    private void VivoxLog(object message) { 
        Debug.Log("<color=yellow>Vivox : " + message+"</color>");
    }

  
}