using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


namespace Project
{
    public class ButtonHelperUI : MonoBehaviour
    {
        public TextMeshProUGUI buttonText;
        public float sizeIncrease;
        public Color baseColor, enterColor, clickColor;
        public GameObject ToolTip;
        bool isUpscaled = false;
        RectTransform thisTranform;
        Animator animator;

        public void Start()
        {
            thisTranform = GetComponent<RectTransform>();
            animator = GetComponent<Animator>();
        }
        public void MoveUI()
        {
            if (animator == null) return;
            animator.SetBool("isHover", true);
        }
        public void unMoveUI()
        {
            if (animator == null) return;
            animator.SetBool("isHover", false);
        }
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

        public void ShowToolTip()
        {
            ToolTip.SetActive(true);
        }
        public void HideToolTip()
        {
            ToolTip.SetActive(false);
        }
        public void ChangeFont(string Event)
        {
            switch (Event)
            {
                case "Enter":
                    buttonText.color = enterColor;
                    Upscale();
                    MoveUI();
                    break;
                case "Click":
                    buttonText.color = clickColor;
                    break;
                case "Exit":
                    buttonText.color = baseColor;
                    Downscale();
                    unMoveUI();
                    break;
                default:
                    break;
            }
        }

    }
}
