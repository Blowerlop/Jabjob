using UnityEngine;
using Unity.Netcode;
using Unity.Collections;


public class PlayerNetwork : NetworkBehaviour
{
    #region Variables
    
    #endregion

    private NetworkVariable<MyCustomData> randomNumber = new NetworkVariable<MyCustomData>(new MyCustomData
    {
        _int = 56,
        _bool = true,
    }
        , NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public struct MyCustomData : INetworkSerializable
    {
        public int _int;
        public bool _bool;
        public FixedString128Bytes message;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _bool);
            serializer.SerializeValue(ref message);
        }
    }
    public override void OnNetworkSpawn()
    {
        randomNumber.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) => {
            Debug.Log(OwnerClientId + "; " + newValue._int + "; " + newValue._bool + "; " + newValue.message);
        };
    }

    private void Update()
    {
        if (!IsOwner) return;
    }

    [ServerRpc]
    private void FireServerRpc()
    {
        FireClientRpc();
    }

    [ClientRpc]
    private void FireClientRpc()
    {
        if (!IsOwner) localFire();
    }


    private void PerformFire()
    {
        /*
        if (InputManager.instance.isFiring)
        {
            InputManager.instance.isFiring = false;
            FireServerRpc();
            localFire();
        }
        */
    }
    private void localFire()
    {
        /*
        Transform spawnedObjectTransform = Instantiate(spawnedObjectPrefab);
        spawnedObjectTransform.position = transform.position;
        spawnedObjectTransform.rotation = transform.rotation;
        */
    }


    #region Methods
    
    #endregion
}