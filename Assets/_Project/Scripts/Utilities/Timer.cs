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

            _coroutine = UtilitiesClass.instance.StartCoroutine(SimpleTimer(timeInSeconds));
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

            _coroutine = UtilitiesClass.instance.StartCoroutine(TimerWithCallback(timeInSeconds, callback));
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

        public float GetTimeRemaining() => timer;


        public static void StartTimerWithCallback(float timeInSeconds, Action callback)
        {
            // int timeInMilliseconds = (int)(timeInSeconds * 1000);
            // await Task.Delay(timeInMilliseconds);
            // callback.Invoke();
            UtilitiesClass.instance.StartCoroutine(TimerWithCallbackStatic(timeInSeconds, callback));
        }

        private static IEnumerator TimerWithCallbackStatic(float timeInSeconds, Action callback)
        {
            yield return new WaitForSeconds(timeInSeconds);
            callback.Invoke();
        }
    }
}