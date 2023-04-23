using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


namespace Project
{
    public class FontModifier : MonoBehaviour
    {
        public TextMeshProUGUI buttonText;
        public float sizeIncrease;
        public Color baseColor, enterColor, clickColor;

        bool isUpscaled = false;

        public void Upscale()
        {
            if (!isUpscaled)
            {
                buttonText.fontSize *= sizeIncrease;
                isUpscaled = true;
            }
        }

        public void Downscale()
        {
            if(isUpscaled)
            {
                buttonText.fontSize /= sizeIncrease;
                isUpscaled = false;
            }
        }

        public void ChangeColor(string Event)
        {
            if (Event == "Enter") buttonText.color = enterColor;
            if (Event == "Click") buttonText.color = clickColor;
            if (Event == "Exit") buttonText.color = baseColor;
        }

    }
}
