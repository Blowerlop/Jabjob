using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class UISpawner : NetworkBehaviour
{
    [SerializeField] private List<GameObject> _hudList = new List<GameObject>();
    [SerializeField] private Transform _hudTransform;
    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner) return;
        foreach (GameObject go in _hudList)
        {
            Instantiate(go, _hudTransform);
        }
    }

}
