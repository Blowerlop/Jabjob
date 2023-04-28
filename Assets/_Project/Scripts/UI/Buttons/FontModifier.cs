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

        public void ChangeFont(string Event)
        {
            switch (Event)
            {
                case "Enter":
                    buttonText.color = enterColor;
                    Upscale();
                    break;
                case "Click":
                    buttonText.color = clickColor;
                    break;
                case "Exit":
                    buttonText.color = baseColor;
                    Downscale();
                    break;
                default:
                    break;
            }
        }

    }
}
