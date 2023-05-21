using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingManager : MonoBehaviour
{
    public static ObjectPoolingManager instance;
    List<GameObject> projectileList = new();
    public int baseNumber;
    public GameObject projectilePrefab;
    

    /// <summary>
    /// Initialize all bullets
    /// </summary>
    void Awake()
    {
        instance = this;
        for(int i = 0; i < baseNumber; i++)
        {
            GameObject go = Instantiate(projectilePrefab,transform);
            projectileList.Add(go);
            go.SetActive(false);
        }
    }

    /// <summary>
    /// Return a gameobject into the ObjectPoolingManager
    /// </summary>
    public void ReturnGameObject(GameObject go)
    {
        go.SetActive(false);
        projectileList.Add(go); 
    }

    /// <summary>
    /// Get a gameobject from ObjectPoolingManager.objects
    /// </summary>
    public GameObject GetObject()
    {
        GameObject go;
        if (projectileList.Count > 0)
        {
            go = projectileList[0];
            projectileList.Remove(projectileList[0]);
            go.SetActive(true);

        }
        else
        {
            go = Instantiate(projectilePrefab, transform);
        }
        return go;
    }
}
