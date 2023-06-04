using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Internal;
using Object = UnityEngine.Object;

namespace Project.Utilities
{
    public enum ETime
    {
        Milliseconds,
        Seconds,
        Minutes
    }
    
    public class UtilitiesClass : MonoBehaviour
    {
        public static UtilitiesClass instance;
        
        
        private void Awake()
        {
            instance = this;
            
            Debug.Log("Convert : " + ConvertSecondsToMinutes(1));
        }
        

        #region Methods
        

        public static IEnumerator LerpInTimeCoroutine(float timeInSeconds, float from, float to, Action<float> callback, Action onFinishCallback = null)
        {
            float timer = 0.0f;
            while (timer < timeInSeconds)
            {
                timer += Time.deltaTime;
                callback.Invoke(Mathf.Lerp(from, to, timer / timeInSeconds));
                yield return null;
            }
            
            onFinishCallback?.Invoke();
        }
        
        public static IEnumerator WaitForSecondsAndDoActionCoroutine(float timeInSeconds, Action action)
        {
            yield return new WaitForSeconds(timeInSeconds);
            action.Invoke();
        }
        
        public static IEnumerator WaitForFramesAndDoActionCoroutine(int frames, Action action)
        {
            for (int i = 0; i < frames; i++)
            {
                yield return null;
            }
            
            action.Invoke();
        }
        
        public static IEnumerator WaitForEndOfFrameAndDoActionCoroutine(Action action)
        {
            yield return new WaitForEndOfFrame()
                ;            
            action.Invoke();
        }

        /// <summary>
        /// This script only works if there is only one layer selected
        /// </summary>
        /// <param name="layerMask"></param>
        /// <returns></returns>
        public static int LayerToInt(LayerMask layerMask)
        {
            return Mathf.RoundToInt(Mathf.Log(layerMask.value, 2));
        }
        
            
        public static void SwitchLayerInChildren(GameObject gameObject, LayerMask layerMask)
        {
            Transform[] children = gameObject.GetComponentsInChildren<Transform>();
            for (int i = 0; i < children.Length; i++)
            {
                children[i].gameObject.layer = LayerToInt(layerMask);
            }
        }


        public static float ConvertSecondsToMinutes(float timeInSeconds) => Mathf.FloorToInt(timeInSeconds * 0.0166f);
        // public float ConvertTime(float time, ETime from, ETime to)
        // {
        //     if (from == to) return time;
        //     
        //     if (from == ETime.Milliseconds)
        //     {
        //         if (to == ETime.Seconds)
        //         {
        //             return time * 0.001f;
        //         }
        //         else if (to == ETime.Minutes)
        //         {
        //             return time * 0.000016f;
        //         }
        //     }
        //
        //     if (from == ETime.Seconds)
        //     {
        //         if (to == ETime.Milliseconds)
        //         {
        //         
        //         }
        //         else if (to == ETime.Minutes)
        //         {
        //         
        //         }
        //     }
        //     
        //     if (from == ETime.Minutes)
        //     {
        //         if (to == ETime.Milliseconds)
        //         {
        //         
        //         }
        //         else if (to == ETime.Seconds)
        //         {
        //         
        //         }
        //     }
        // }

        #endregion
    }
}




