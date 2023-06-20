using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UtilitiesClass = Project.Utilities.UtilitiesClass;

namespace Project.Utilities
{
    [System.Serializable]
    public class Timer
    {
        [field: SerializeField] public float timer { get; set; }
        public bool hasATimerStarted => _coroutine != null;
        private Coroutine _coroutine;

        public void StartSimpleTimer(float timeInSeconds, bool forceStart = false)
        {
            if (hasATimerStarted && forceStart == false)
            {
                Debug.Log("A timer is already in progress");
                return;
            }

            if (hasATimerStarted)
            {
                UtilitiesClass.instance.StopCoroutine(_coroutine);
            }

            _coroutine = UtilitiesClass.instance.StartCoroutine(SimpleTimer(timeInSeconds));
        }

        public void StartTimerWithCallbackScaledTime(float timeInSeconds, Action callback, bool forceStart = false)
        {
            if (hasATimerStarted && forceStart == false)
            {
                Debug.Log("A timer is already in progress");
                return;
            }

            if (hasATimerStarted)
            {
                StopTimer();
            }

            _coroutine = UtilitiesClass.instance.StartCoroutine(TimerWithCallback(timeInSeconds, callback));
        }


        private IEnumerator SimpleTimer(float timeInSeconds)
        {
            timer = timeInSeconds;
            while (timer > 0.0f)
            {
                timer -= Time.deltaTime;
                yield return null;
            }

        }

        private IEnumerator TimerWithCallback(float timeInSeconds, Action callback)
        {
            timer = timeInSeconds;
            while (timer > 0.0f)
            {
                timer -= Time.deltaTime;
                yield return null;
            }

            callback?.Invoke();
        }

        private void StopTimer()
        {
            UtilitiesClass.instance.StopCoroutine(_coroutine);
            _coroutine = null;
        }

        public float GetTimeRemaining() => timer;


        public static Coroutine StartTimerWithCallbackScaledTime(float timeInSeconds, Action callback)
        {
            // int timeInMilliseconds = (int)(timeInSeconds * 1000);
            // await Task.Delay(timeInMilliseconds);
            // callback.Invoke();
            return UtilitiesClass.instance.StartCoroutine(TimerWithCallbackStaticScaledTime(timeInSeconds, callback));
        }
        
        public static Coroutine StartTimerWithCallbackRealTime(float timeInSeconds, Action callback)
        {
            // int timeInMilliseconds = (int)(timeInSeconds * 1000);
            // await Task.Delay(timeInMilliseconds);
            // callback.Invoke();
            return UtilitiesClass.instance.StartCoroutine(TimerWithCallbackStaticRealTime(timeInSeconds, callback));
        }

        private static IEnumerator TimerWithCallbackStaticScaledTime(float timeInSeconds, Action callback)
        {
            yield return new WaitForSeconds(timeInSeconds);
            callback.Invoke();
        }
        
        private static IEnumerator TimerWithCallbackStaticRealTime(float timeInSeconds, Action callback)
        {
            yield return new WaitForSecondsRealtime(timeInSeconds);
            callback.Invoke();
        }
    }
}
