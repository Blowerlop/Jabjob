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
            
            [SerializeField] private NetworkVariable<float> _networkTimer = new NetworkVariable<float>();
            private float _timer;
            [SerializeField] private float _tickRate = 1.0f;
            private readonly NetworkVariable<bool> _hasATimerStarted = new NetworkVariable<bool>(false);
            private Coroutine _coroutine;
            


            private void Awake()
            {
                instance = this; 
            }

            public override void OnNetworkSpawn()
            {
                base.OnNetworkSpawn();

                _networkTimer.OnValueChanged += Invoke;
            }

            public override void OnNetworkDespawn()
            {
                base.OnNetworkDespawn();

                _networkTimer.OnValueChanged -= Invoke;
            }


            public void StartSimpleTimer(float timeInSeconds, bool forceStart = false) => StartTimerWithCallback(timeInSeconds, null, true);
            
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

                _coroutine = Utilities.UtilitiesClass.instance.StartCoroutine(TimerWithCallback(timeInSeconds, callback));
            }
            
            private IEnumerator TimerWithCallback(float timeInSeconds, Action callback)
            {
                _hasATimerStarted.Value = true;
                _timer = timeInSeconds;
                float tickRateTimer = _tickRate;
                
                while (_timer > 0.0f)
                {
                    _timer -= Time.deltaTime;
                    tickRateTimer -= Time.deltaTime;
                    
                    if (tickRateTimer <= 0)
                    {
                        tickRateTimer = _tickRate;
                        _networkTimer.Value = _timer;
                    }
                    
                    yield return null;
                }
                
                callback?.Invoke();
                _hasATimerStarted.Value = false;
                
                
                
                // _hasATimerStarted.Value = true;
                // _networkTimer.Value = timeInSeconds;
                // while (_networkTimer.Value > 0.0f)
                // {
                //     _networkTimer.Value -= Time.deltaTime;
                //     yield return null;
                // }
                //
                // callback?.Invoke();
                // _hasATimerStarted.Value = false;
            }

            public float GetElapsedTime() => _networkTimer.Value;
            

            // public static async void StartTimerWithCallback(float timeInSeconds, Action callback)
            // {
            //     int timeInMilliseconds = (int)(timeInSeconds * 1000);
            //     await Task.Delay(timeInMilliseconds);
            //     callback.Invoke();
            // }
            //
            // // public static async void StartTimerWithCallback<T>(float timeInSeconds, Action<T> callback, T arg)
            // // {
            // //     int timeInMilliseconds = (int)(timeInSeconds * 1000);
            // //     await Task.Delay(timeInMilliseconds);
            // //     callback.Invoke(arg);
            // // }
            
            public void Invoke(float previousValue, float nextValue) => GameEvent.onGameTimerUpdated.Invoke(this, false, nextValue);
        }
}