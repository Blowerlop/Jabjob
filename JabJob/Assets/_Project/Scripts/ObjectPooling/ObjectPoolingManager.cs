using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingManager : MonoBehaviour
{
    public static ObjectPoolingManager instance;
    List<GameObject> objects = new();
    public int baseNumber;
    public GameObject objectToInstanciate;

    /// <summary>
    /// Initialize all bullets
    /// </summary>
    void Awake()
    {
        instance = this;
        for(int i = 0; i < baseNumber; i++)
        {
            GameObject go = Instantiate(objectToInstanciate,transform);
            objects.Add(go);
            go.SetActive(false);
        }
        GetObject();
        GetObject();
        ReturnGameObject(GetObject());
        GetObject();
    }

    /// <summary>
    /// Return a gameobject into the ObjectPoolingManager
    /// </summary>
    public void ReturnGameObject(GameObject go)
    {
        go.SetActive(false);
        objects.Add(go);
    }

    /// <summary>
    /// Get a gameobject from ObjectPoolingManager.objects
    /// </summary>
    public GameObject GetObject()
    {
        GameObject go;
        if (objects.Count > 0)
        {
            go = objects[0];
            objects.Remove(objects[0]);
            go.SetActive(true);
        }
        else
        {
            go = Instantiate(objectToInstanciate, transform);
        }
        return go;
    }
}
