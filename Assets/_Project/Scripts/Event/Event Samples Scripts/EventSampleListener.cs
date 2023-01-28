using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Project
{
    public class EventSampleListener : NetworkBehaviour, IGameEventListener
    {
        public void OnEnable()
        {
            GameEvent.onGlobalEventSample.Subscribe(SpawnObjectLocal, this);
            //GameEvent.onGlobal.Subscribe(() => {SpawnObjectServerRpc();});
            GameEvent.onGlobalEventSample.Subscribe(SpawnObjectServerRpcV2, this);
            
            GameEvent.onLocalEventSample.Subscribe(SpawnObjectLocal, this);
        }

        public void OnDisable()
        {
            GameEvent.onGlobalEventSample.Unsubscribe(SpawnObjectLocal);
            //GameEvent.onGlobal.Unsubscribe(() => {SpawnObjectServerRpc();});
            GameEvent.onGlobalEventSample.Unsubscribe(SpawnObjectServerRpcV2);
            
            GameEvent.onLocalEventSample.Unsubscribe(SpawnObjectLocal);
        }



        private void SpawnObjectServerRpcV2()
        {
            SpawnObjectServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnObjectServerRpc(ServerRpcParams serverRpcParams = default)
        {
            Debug.Log("Server RPC");
            
            List<ulong> nonLocalClientsIds = NetworkUtilities.GetAllNonLocalConnectedClientsIds(serverRpcParams);
            SpawnObjectClientRpc(NetworkUtilities.GetNewClientRpcSenderParams(nonLocalClientsIds));
        }
        
        [ClientRpc]
        private void SpawnObjectClientRpc(ClientRpcParams clientRpcParams)
        {
            Debug.Log("Client RPC");
            SpawnObjectLocal();      
        }
        
        private void SpawnObjectLocal()
        {
            Debug.Log("Local RPC");
            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.transform.position = new Vector3(0,0, (int)NetworkManager.Singleton.LocalClientId);
        }
        
    }
}