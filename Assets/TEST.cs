using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Project
{
    public class TEST : NetworkBehaviour
    {
        public void Update()
        {
            if (IsServer && Input.GetKeyDown(KeyCode.C)) 
            {
                Debug.Log("OnNetworkSpawn : " + NetworkManager.Singleton.ConnectedClientsIds.Count);
            }
        }
    }
}
