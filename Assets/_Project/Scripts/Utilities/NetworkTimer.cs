using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace Project
{
    public class NetworkTimer : NetworkBehaviour
    {
            public static NetworkTimer instance;
            
            private readonly NetworkVariable<float> _timer = new NetworkVariable<float>();
            private readonly NetworkVariable<bool> _hasATimerStarted = new NetworkVariable<bool>(false);
            private Coroutine _coroutine;


            private void Awake()
            {
                instance = this; 
            }

            public void StartSimpleTimer(float timeInSeconds, bool forceStart = false)
            {
                if (_hasATimerStarted.Value && forceStart == false)
                {
                    Debug.Log("A timer is already in progress");
                    return;
                }

                if (_hasATimerStarted.Value)
                {
                    Utilities.UtilitiesClass.instance.StopCoroutine(_coroutine);
                }

                _coroutine = Utilities.UtilitiesClass.instance.CoroutineStart(SimpleTimer(timeInSeconds));
            }

            public void StartTimerWithCallback(float timeInSeconds, Action callback, bool forceStart = false)
            {
                if (_hasATimerStarted.Value && forceStart == false)
                {
                    Debug.Log("A timer is already in progress");
                    return;
                }

                if (_hasATimerStarted.Value)
                {
                    Utilities.UtilitiesClass.instance.StopCoroutine(_coroutine);
                }

                _coroutine = Utilities.UtilitiesClass.instance.CoroutineStart(TimerWithCallback(timeInSeconds, callback));
            }


            private IEnumerator SimpleTimer(float timeInSeconds)
            {
                _hasATimerStarted.Value = true;
                
                _timer.Value = timeInSeconds;
                while (_timer.Value > 0.0f)
                {
                    _timer.Value -= Time.deltaTime;
                    yield return null;
                }

                _hasATimerStarted.Value = false;
            }

            private IEnumerator TimerWithCallback(float timeInSeconds, Action callback)
            {
                _hasATimerStarted.Value = true;
                _timer.Value = timeInSeconds;
                while (_timer.Value > 0.0f)
                {
                    _timer.Value -= Time.deltaTime;
                    yield return null;
                }

                callback?.Invoke();
                _hasATimerStarted.Value = false;
            }

            public float GetElapsedTime() => _timer.Value;
            

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