using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Project
{
    public class NetworkUtilities : NetworkBehaviour
    {
        public static List<ulong> GetAllNonLocalConnectedClientsIds(ServerRpcParams serverRpcParams)
        {
            var clientId = serverRpcParams.Receive.SenderClientId;
            List<ulong> clientsIds = NetworkManager.Singleton.ConnectedClientsIds.ToList();
            clientsIds.Remove(serverRpcParams.Receive.SenderClientId);
            return clientsIds;
        }

        public static ClientRpcParams GetNewClientRpcSenderParams(List<ulong> allConnectedNoneLocalClientsIds)
        {
            return new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = allConnectedNoneLocalClientsIds }
            };
        }
    }
}