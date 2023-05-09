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
    public class UtilitiesClass : MonoBehaviour
    {
        public static UtilitiesClass instance;
        
        
        private void Awake()
        {
            instance = this;
        }
        

        #region Methods

        
        public Coroutine CoroutineStart(IEnumerator coroutine) => StartCoroutine(coroutine);
        public Coroutine CoroutineStart(string methodName) => StartCoroutine(methodName);
        public Coroutine CoroutineStart(string methodName, [DefaultValue("null")] object value) => StartCoroutine(methodName, value);

        public void CoroutineStop(Coroutine coroutine) => StopCoroutine(coroutine);
        public void CoroutineStop(IEnumerator coroutine) => StopCoroutine(coroutine);
        public void CoroutineStop(string methodName) => StopCoroutine(methodName);



        /// <summary>
        /// This script only if there is only one layer selected
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

        #endregion
    }
}




