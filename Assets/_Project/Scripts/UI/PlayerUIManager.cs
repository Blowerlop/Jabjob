using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;

public class PlayerUIManager : NetworkBehaviour
{
    [SerializeField] private List<GameObject> _hudList = new List<GameObject>();
    [SerializeField] private Transform _hudTransform;

    private List<GameObject> playerHudList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner) return;
        foreach (GameObject go in _hudList)
        {
            playerHudList.Add(Instantiate(go, _hudTransform));
        }
    }

    public void HidePlayerUI()
    {
        foreach (GameObject go in playerHudList)
        {
            if (go.CompareTag("UI Player"))
            {
                Canvas canvas = go.GetComponent<Canvas>();
                canvas.enabled = false;
            }
        }
    }

    public void ShowPlayerUI()
    {
        foreach (GameObject go in playerHudList)
        {
            if (go.CompareTag("UI Player"))
            {
                Canvas canvas = go.GetComponent<Canvas>();
                canvas.enabled = true;
            }
        }
    }
}
