using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> uiToSpawn = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject go in uiToSpawn)
        {
            Instantiate(go);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
