using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    public class DropDownItemsSeter_FullScreenModeContainer : MonoBehaviour
    {
        private DropDownExtended _dropDownExtended;
        


        private void Awake()
        {
            _dropDownExtended = GetComponent<DropDownExtended>();
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
        }


        private void SetFullScreenModeContainer(RectTransform rectTransform, int index)
        {
            if (index == 0)
            {
                rectTransform.GetComponent<FullScreenModeContainer>().fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            }
            else if (index == 1)
            {
                rectTransform.GetComponent<FullScreenModeContainer>().fullScreenMode = FullScreenMode.FullScreenWindow;
            }
            else
            {
                Debug.Log("ERRORRRRRRR ITEMS PAS BON NOMBRE");
            }
        }
    }
}
