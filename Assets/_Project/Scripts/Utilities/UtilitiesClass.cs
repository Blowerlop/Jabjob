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

namespace Project
{
    namespace Utilities
    {
        public class UtilitiesClass : MonoBehaviour
        {
            #region Variables

            public static UtilitiesClass instance;
        
            #endregion


            #region Updates

            private void Awake()
            {
                instance = this;
            }


            #endregion


            #region Methods

        
            public Coroutine CoroutineStart(IEnumerator coroutine) => StartCoroutine(coroutine);
            public Coroutine CoroutineStart(string methodName) => StartCoroutine(methodName);
            public Coroutine CoroutineStart(string methodName, [DefaultValue("null")] object value) => StartCoroutine(methodName, value);
        
            public void CoroutineStop(Coroutine coroutine) => StopCoroutine(coroutine);
            public void CoroutineStop(IEnumerator coroutine) => StopCoroutine(coroutine);
            public void CoroutineStop(string methodName) => StopCoroutine(methodName);

            #endregion
        }
        
        [System.Serializable]
        public class Timer
        {
            [field: SerializeField] public float timer { get; set; }
            private bool _hasATimerStarted = false;
            private Coroutine _coroutine;

            public void StartSimpleTimer(float timeInSeconds, bool forceStart = false)
            {
                if (_hasATimerStarted && forceStart == false)
                {
                    Debug.Log("A timer is already in progress");
                    return;
                }

                if (_hasATimerStarted)
                {
                    UtilitiesClass.instance.StopCoroutine(_coroutine);
                }

                _coroutine = UtilitiesClass.instance.CoroutineStart(SimpleTimer(timeInSeconds));
            }

            public void StartTimerWithCallback(float timeInSeconds, Action callback, bool forceStart = false)
            {
                if (_hasATimerStarted && forceStart == false)
                {
                    Debug.Log("A timer is already in progress");
                    return;
                }

                if (_hasATimerStarted)
                {
                    UtilitiesClass.instance.StopCoroutine(_coroutine);
                }

                _coroutine = UtilitiesClass.instance.CoroutineStart(TimerWithCallback(timeInSeconds, callback));
            }


            private IEnumerator SimpleTimer(float timeInSeconds)
            {
                _hasATimerStarted = true;
                timer = timeInSeconds;
                while (timer > 0.0f)
                {
                    timer -= Time.deltaTime;
                    yield return null;
                }

                _hasATimerStarted = false;
            }

            private IEnumerator TimerWithCallback(float timeInSeconds, Action callback)
            {
                _hasATimerStarted = true;
                timer = timeInSeconds;
                while (timer > 0.0f)
                {
                    timer -= Time.deltaTime;
                    yield return null;
                }

                callback?.Invoke();
                _hasATimerStarted = false;
            }

            public float GetElapsedTime() => timer;
            

            public static async void StartTimerWithCallback(float timeInSeconds, Action callback)
            {
                int timeInMilliseconds = (int)(timeInSeconds * 1000);
                await Task.Delay(timeInMilliseconds);
                callback.Invoke();
            }
        }

        public static class Extensions
        {
            public static List<Transform> GetChildren(this Transform transform, List<Transform> children = null)
            {
                return GetChildren<Transform>(transform);
            }
            public static List<T> GetChildren<T>(this Transform transform, List<T> children = null) where T : Component
            {
                if (children == null) children = new List<T>();
                
                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform child = transform.GetChild(i);

                    child.GetChildren(children);
                    if (child.TryGetComponent(out T TChild))
                    {
                        children.Add(TChild);                        
                    }
                }
                
                return children;
            }

            public static void DestroyChildren(this Transform transform)
            {
                for (int i = transform.childCount - 1; i >= 0 ; i--)
                {
                    if (Application.isEditor)
                    {
                        Object.DestroyImmediate(transform.GetChild(i).gameObject);
                    }
                    else
                    {
                        Object.Destroy(transform.GetChild(i).gameObject);
                    }
                }

                List<Transform> a = new List<Transform>();
            }

            public static void ResetVelocities(this Rigidbody rigidbody)
            {
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
            }

            public static void ForInRange<T>(this IList<T> target, Action<T> action)
            {
                for (int i = 0; i < target.Count; i++)
                {
                    action.Invoke(target[i]);
                }
            }
            
            public static void ForEach<T>(this IList<T> target, Action<T> action)
            {
                foreach (T t in target)
                {
                    action.Invoke(t);
                }
            }
        }
    }
}




