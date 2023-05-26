using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project
{
    public enum ESelectionState
    {
        None,
        Normal,
        Highlighted,
        Clicked,
        Selected,
        Disabled,
    }
    
    public class ButtonUIConfigurator : MonoBehaviour
    {
        // [SerializeField] private RectTransform _normalContent;
        // [SerializeField] private RectTransform _highlightContent;
        // [SerializeField] private RectTransform _clickedContent;

        [Header("References")]
        [SerializeField] private Image _baseImage;
        [SerializeField] private Image _outlineImage;
        [SerializeField] private TMPro.TMP_Text _text;
        
        [SerializeField] private bool _hasHighlightedState;
        [SerializeField] private bool _hasClickedState;
        
        [Header("Base Image")]
        [SerializeField] private Sprite _baseNormalSprite;
        [SerializeField] private Color _baseNormalColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        [SerializeField] private Sprite _baseHighlightedSprite;
        [SerializeField] private Color _baseHighlightedColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        [SerializeField] private Sprite _baseSelectedSprite;
        [SerializeField] private Color _baseSelectedColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);

        [Header("Outline Image")]
        [SerializeField] private Sprite _outlineNormalSprite;
        [SerializeField] private Color _outlineNormalColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        [SerializeField] private Sprite _outlineHighlightedSprite;
        [SerializeField] private Color _outlineHighlightedColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        [SerializeField] private Sprite _outlineSelectedSprite;
        [SerializeField] private Color _outlineSelectedColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);

        [Header("Text")] 
        [SerializeField] private Color _textNormalColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        [SerializeField] private Color _textHighLightedColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        [SerializeField] private Color _textSelectedColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);

        [Header("Preview State")] 
        [SerializeField] private ESelectionState _previewState;


        private void Start()
        {
            SetNormalBehaviour(); 
        }


        private void OnValidate()
        {
            if (gameObject.activeSelf == false) return;
            
            bool needToReturned = false;
            
            if (_baseImage == null)
            {
                Debug.LogWarning($"{name} : BaseImage is null !");
                needToReturned = true;
            }
            if (_outlineImage == null)
            {
                Debug.LogWarning($"{name} : OutlineImage is null !");
                needToReturned = true;
            }

            if (_text == null)
            {
                Debug.LogWarning($"{name} : Text is null !");
                needToReturned = true;
            }

            if (needToReturned) return;

            switch (_previewState)
            {
                // None,
                // Normal,
                // Highlighted,
                // Clicked,
                // Selected,
                // Disabled,

                case ESelectionState.Normal: 
                    SetNormalBehaviour();
                    break;   
                    
                case ESelectionState.Highlighted: 
                    SetHighlightedBehaviour();
                    break;
                    
                case ESelectionState.Clicked:
                    break;  
                        
                case ESelectionState.Selected:
                    SetSelectedBehaviour();
                    break;
                    
                case ESelectionState.Disabled: 
                    break;  
                    
                default:
                    break;
;            }
        }


        public void SetNormalBehaviour()
        {
            _previewState = ESelectionState.Normal;
            
            _baseImage.sprite = _baseNormalSprite;
            _baseImage.color = _baseNormalColor;
            
            _outlineImage.sprite = _outlineNormalSprite;
            _outlineImage.color = _outlineNormalColor;
            
            _text.color = _textNormalColor;
        }
        
        public void SetHighlightedBehaviour()
        {
            if (_hasHighlightedState == false || _previewState == ESelectionState.Selected) return;
            _previewState = ESelectionState.Highlighted;

            _baseImage.sprite = _baseHighlightedSprite;
            _baseImage.color = _baseHighlightedColor;
            
            _outlineImage.sprite = _outlineHighlightedSprite;
            _outlineImage.color = _outlineHighlightedColor;
            
            _text.color = _textHighLightedColor;
        }
        
        public void SetSelectedBehaviour()
        {
            if (_hasClickedState == false) return;
            _previewState = ESelectionState.Selected;

            _baseImage.sprite = _baseSelectedSprite;
            _baseImage.color = _baseSelectedColor;
            
            _outlineImage.sprite = _outlineSelectedSprite;
            _outlineImage.color = _outlineSelectedColor;
            
            _text.color = _textSelectedColor;
        }
    }
}
