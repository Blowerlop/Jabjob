using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Project
{
    public class Weapon : MonoBehaviour
    {
        
    }
    
    /*
    public struct WeaponStruct : INetworkSerializable
    {
        public SOWeapon weaponData;
        public GameObject origin;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref weaponData);
        }
    }
    
    public struct SOWeaponStruct : INetworkSerializable
    {
        public int ID;
        public string name;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ID);
        }
    }*/
}



