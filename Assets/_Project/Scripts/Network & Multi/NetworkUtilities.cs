using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;

namespace Project
{
    public class NetworkUtilities : NetworkBehaviour
    {
        public static List<ulong> GetAllNonLocalConnectedClientsIds(ServerRpcParams serverRpcParams)
        {
            var clientId = serverRpcParams.Receive.SenderClientId;
            List<ulong> clientsIds = NetworkManager.Singleton.ConnectedClientsIds.ToList();
            clientsIds.Remove(clientId);
            return clientsIds;
        }
        

        public static ClientRpcParams GetNewClientRpcSenderParams(List<ulong> targetClientIds)
        {
            return new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = targetClientIds }
            };
        }
    }
    
    #region Custom struct

    [Serializable]
    public struct StringNetwork : INetworkSerializable, IEquatable<StringNetwork>
    {
        [ReadOnlyField] public string value;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsReader)
            {
                var reader = serializer.GetFastBufferReader();
                reader.ReadValueSafe(out value);
            }
            else
            {
                var writer = serializer.GetFastBufferWriter();
                writer.WriteValueSafe(value);
            }
        }

        public bool Equals(StringNetwork other) =>
            String.Equals(other.value, value, StringComparison.CurrentCultureIgnoreCase);
    }

    #endregion
}