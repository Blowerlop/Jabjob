using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine.UI;
using System.Collections.Generic;

public class RelayManager : MonoBehaviour
{

    string _inputText;
    [SerializeField] int _maximumPlayer = 4;
    [SerializeField] Text _codeRelay;
    [SerializeField] private List<ulong> _players;

    async void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += AddPlayer;
        NetworkManager.Singleton.OnClientDisconnectCallback += RemovePlayer;
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);

        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        
    }
    private void AddPlayer(ulong playerId)
    {
        _players.Add(playerId);
    }
    private void RemovePlayer(ulong playerId)
    {
        _players.Remove(playerId);
    }

    #region Relay Method
    public async void CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(_maximumPlayer-1); //Nombre de client en + de l'host
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log(joinCode);
            _codeRelay.text = string.Concat("Code : ", joinCode);
            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }
    public async void JoinRelay()
    {
        try
        {
            Debug.Log("Joining Relay with code " + _inputText);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(_inputText);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }
    private async void JoinRelay(string joinCode)
    {
        try
        {
            Debug.Log("Joining Relay with " + joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }


    }

    public void ReadStringInput(string s)
    {
        _inputText = s;
    }
    #endregion
}