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

        private bool isAnimationPlaying = false;
        private Slider currentSlider = null;
        private float beginTime = 0;
        private float endTime = 0;



        private void Start()
        {
            foreach (Slider g in transform.GetComponentsInChildren<Slider>())
            {
                dashSlot.Add(g);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                RemoveOneDash();
            }

            if (isAnimationPlaying)
            {
                if (currentSlider == null) { return; }
                currentSlider.value = Mathf.InverseLerp(beginTime, endTime, Time.realtimeSinceStartup);

                if (currentSlider.value == 1 && dashNumber < 3)
                {
                    dashNumber ++;
                    currentSlider = dashSlot[dashNumber - 1];
                    MakeDashReloadAnimation();
                }
            }
        }

        public void RemoveOneDash()
        {
            if (currentSlider != null) { currentSlider.value = 0; }

            currentSlider = dashSlot[dashNumber - 1];

            currentSlider.value = 0;
            dashNumber --;
            MakeDashReloadAnimation();
        }

        private void MakeDashReloadAnimation()
        {
            if (isAnimationPlaying && dashNumber == 3) { isAnimationPlaying = false; }

            beginTime = Time.realtimeSinceStartup;
            endTime = Time.realtimeSinceStartup + dashCoolDown;
            isAnimationPlaying = true;
        }
    }
}
