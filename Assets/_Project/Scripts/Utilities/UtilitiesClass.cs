using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Internal;

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
        
        public class Timer
        {
            private float _timer;
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
                _timer = timeInSeconds;
                while (_timer > 0.0f)
                {
                    _timer -= Time.deltaTime;
                    yield return null;
                }

                _hasATimerStarted = false;
            }

            private IEnumerator TimerWithCallback(float timeInSeconds, Action callback)
            {
                _hasATimerStarted = true;
                _timer = timeInSeconds;
                while (_timer > 0.0f)
                {
                    _timer -= Time.deltaTime;
                    yield return null;
                }

                callback?.Invoke();
                _hasATimerStarted = false;
            }

            public float GetElapsedTime() => _timer;
            

            public static async void StartTimerWithCallback(float timeInSeconds, Action callback)
            {
                int timeInMilliseconds = (int)(timeInSeconds * 1000);
                await Task.Delay(timeInMilliseconds);
                callback.Invoke();
            }

            public static async void StartTimerWithCallback<T>(float timeInSeconds, Action<T> callback, T arg)
            {
                int timeInMilliseconds = (int)(timeInSeconds * 1000);
                await Task.Delay(timeInMilliseconds);
                callback.Invoke(arg);
            }
        }
    }
}



