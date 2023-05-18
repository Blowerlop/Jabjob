using System;
using System.Collections;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T: MonoBehaviour{
    
    public static bool verbose = false;
    public static bool keepAlive = true;
    public static bool isApplicationQuitting = false;

    private static T _instance = null;
    public static T instance {
        get { 
            if(_instance == null){
                _instance = GameObject.FindObjectOfType<T>();
                if(_instance == null && isApplicationQuitting == false){
                    var singletonObj = new GameObject();
                    singletonObj.name = typeof(T).ToString();
                    _instance = singletonObj.AddComponent<T>();
                }
            }
            return _instance;
        }
    }

    static public bool isInstanceAlive{
        get { return _instance != null; }
    }

    public virtual void Awake(){
        if (_instance != null){
            if(verbose)
                Debug.Log("SingleAccessPoint, Destroy duplicate instance " + name + " of " + instance.name);
            Destroy(gameObject);
            return;
        }

        _instance = GetComponent<T>();
        
        if(keepAlive){
            DontDestroyOnLoad(gameObject);
        }
        
        if (_instance == null){
            if(verbose)
                Debug.LogError("SingleAccessPoint<" + typeof(T).Name + "> Instance null in Awake");
            return;
        }

        if(verbose)
            Debug.Log("SingleAccessPoint instance found " + instance.GetType().Name);
    }

    public void OnApplicationQuit()
    {
        isApplicationQuitting = true;
        StartCoroutine(DestroyInstance());
    }

    private IEnumerator DestroyInstance()
    {
        yield return null;
        if (isInstanceAlive)
        {
            StartCoroutine(DestroyInstance());
        }
        else
        {
            Destroy(gameObject);
        }
    }
}