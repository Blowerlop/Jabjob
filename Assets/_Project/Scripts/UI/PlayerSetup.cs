using System.Collections;
using System.Collections.Generic;
using Project.Utilities;
using Unity.Netcode;
using UnityEngine;

namespace Project
{
    public class PlayerSetup : NetworkBehaviour
    {
        [SerializeField] private Behaviour[] _componentsToDisableIfNotTheOwner;
        [SerializeField] private GameObject[] _gameObjectToDestroyIfNotTheOwner;
        [SerializeField] private LayerMask _layerIfNotTheOwner;

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
            
            UtilitiesClass.SwitchLayerInChildren(gameObject, _layerIfNotTheOwner);

        }
    }
}
