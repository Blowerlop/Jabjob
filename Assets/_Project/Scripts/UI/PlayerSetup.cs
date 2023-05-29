using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Project
{
    public class PlayerSetup : NetworkBehaviour
    {
        [SerializeField] private Behaviour[] _componentsToDisableIfNotTheOwner;
        [SerializeField] private GameObject[] _gameObjectToDestroyIfNotTheOwner;

        public override void OnNetworkSpawn()
        {
            if (IsOwner) return;
            
            foreach (var componentToDestroy in _componentsToDisableIfNotTheOwner)
            {
                componentToDestroy.enabled = false;
            }

            foreach (var gameObjectToDestroy in _gameObjectToDestroyIfNotTheOwner)
            {
                Destroy(gameObjectToDestroy);
            }
        }
    }
}
