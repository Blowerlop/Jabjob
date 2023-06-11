using UnityEngine;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using SceneManager = _Project.Scripts.Managers.SceneManager;

public class RelayWithLobby : MonoBehaviour
{
    string _inputText;
    [SerializeField] int _maximumPlayer = 4;
    public static RelayWithLobby Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    #region Relay Method
    public async Task<string> CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(_maximumPlayer - 1); //Nombre de client en + de l'host
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log(joinCode);
            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();
            return joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return null;
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
    public async void JoinRelay(string joinCode)
    {
        try
        {
            SceneManager.onSceneLoadEvent.Invoke(this, true, 0, "", LoadSceneMode.Additive, null);

            Debug.Log("Joining Relay with " + joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
            NetworkManager.Singleton.SceneManager.OnLoadComplete += InvokeOnSceneLoadComplete;
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
    
    
    private void InvokeOnSceneLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        SceneManager.onSceneLoadCompleteEvent.Invoke(nameof(SceneManager), true, clientId, sceneName, loadSceneMode);
        NetworkManager.Singleton.SceneManager.OnLoadComplete -= InvokeOnSceneLoadComplete;
    }
    #endregion
}