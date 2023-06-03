using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Project
{
    public class DashVisualManager : MonoBehaviour
    {
        private List<Slider> dashSlot = new List<Slider>();

        private int dashNumber = 3;
        private int dashCoolDown = 3;

        private Slider currentSlider = null;
        private float beginTime = 0;
        private float endTime = 0;
        private float currentProgression = 0;

        private bool playAnimation = false;

        private void Start()
        {
            foreach (Slider g in transform.GetComponentsInChildren<Slider>())
            {
                dashSlot.Add(g);
            }
        }

        private void Update()
        {
            if (playAnimation)
            {
                currentSlider.value = Mathf.InverseLerp(beginTime, endTime, Time.realtimeSinceStartup) + currentProgression;

                if (currentSlider.value == 1)
                {
                    AnimationFinish();
                }
            }
        }

        public void RemoveOneDash()
        {
            if (currentSlider != null) 
            {
                currentProgression = currentSlider.value;
                currentSlider.value = 0;
            }

            currentSlider = dashSlot[dashNumber - 1];

            currentSlider.value = 0;
            dashNumber -= 1;
            MakeDashReloadAnimation();
        }

        private void MakeDashReloadAnimation()
        {
            beginTime = Time.realtimeSinceStartup;
            endTime = Time.realtimeSinceStartup + dashCoolDown;
            playAnimation = true;
        }

        private void AnimationFinish()
        {
            dashNumber += 1;
            playAnimation = false;
            currentSlider = null;
            currentProgression = 0;

            if (dashNumber < 3)
            {
                currentSlider = dashSlot[dashNumber];
                MakeDashReloadAnimation();
            }
        }
    }
}