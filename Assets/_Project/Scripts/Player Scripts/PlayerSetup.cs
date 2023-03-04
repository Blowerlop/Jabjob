using Cinemachine;
using Unity.Netcode;
using UnityEngine;

namespace Project
{
    public class PlayerSetup : NetworkBehaviour
    {
        [SerializeField] private Behaviour[] _componentsToDisabledIfNotOwner;
        [SerializeField] private CinemachineVirtualCamera _cinemachineCamera;

        public override void OnNetworkSpawn()
        {
            if (IsOwner) return;
            
            for (int i = 0; i < _componentsToDisabledIfNotOwner.Length; i++)
            {
                _componentsToDisabledIfNotOwner[i].enabled = false;
            }

            _cinemachineCamera.Priority = -1;
        }
    }
}
