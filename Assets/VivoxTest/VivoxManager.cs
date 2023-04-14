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

    public event UnityAction OnUserLoggedIn;
    public event UnityAction<string, IChannelTextMessage> OnTextMessageLogReceived;
    private async void Start()
    {
        InitializationOptions options = new InitializationOptions();
        options.SetProfile("Getet" + UnityEngine.Random.Range(0, 650));
        await UnityServices.InitializeAsync(options);
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        VivoxService.Instance.Initialize();
    }

    public void Login(string displayName)
    {
        Account account = new Account(displayName);
        LoginSession = _client.GetLoginSession(account);
        LoginSession.PropertyChanged += LoginSession_PropertyChanged;
        LoginSession.BeginLogin(LoginSession.GetLoginToken(), SubscriptionMode.Accept, null, null, null, ar =>
        {
            try
            {
                LoginSession.EndLogin(ar);
            }
            catch (Exception e)
            {
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
                break;
            case LoginState.LoggedIn:
                VivoxLog("Logged in");
                OnUserLoggedIn?.Invoke();
                JoinChannel(ChannelName);
                break;
            case LoginState.LoggingIn:
                VivoxLog("Loggin in");
                break;
            case LoginState.LoggingOut:
                break;
            default:
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

            IChannelSession channelSession = LoginSession.GetChannelSession(channel);
            channelSession.MessageLog.AfterItemAdded += OnMessageLogReceive;

            channelSession.BeginConnect(false, true, true, channelSession.GetConnectToken(), ar =>
            {
                try
                {
                    channelSession.EndConnect(ar);
                }
                catch (Exception e )
                {
                    Debug.Log(e.Message);
                    throw;
                }
            });
        }
    }

    private void OnMessageLogReceive(object sender, QueueItemAddedEventArgs<IChannelTextMessage> textMessage)
    {
        Debug.Log("Message received");
        IChannelTextMessage message = textMessage.Value;
        OnTextMessageLogReceived?.Invoke(message.Sender.DisplayName, message);
    }

    private void VivoxLog(string msg = null) { 
    
        Debug.Log(msg);
    }

}
