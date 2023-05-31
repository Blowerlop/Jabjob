using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project
{
    public enum EButtonState
    {
        None,
        Normal,
        Highlighted,
        Clicked,
        Selected,
        Disabled,
    }
    
    [RequireComponent(typeof(ButtonUIBehaviourConfigurator))]
    public class ButtonUIVisualConfigurator : MonoBehaviour
    {
        // [SerializeField] private RectTransform _normalContent;
        // [SerializeField] private RectTransform _highlightContent;
        // [SerializeField] private RectTransform _clickedContent;

        [Header("References")] 
        [SerializeField] private GameObject _imagesContainer;
        [SerializeField] private Image _baseImage;
        [SerializeField] private Image _outlineImage;
        [SerializeField] private TMPro.TMP_Text _text;
        [SerializeField] private RectTransform _spacer;
        
        [Space(15)]
        [SerializeField] private float _spaceBetweenImagesAndText = 0.0f;
        [SerializeField] private bool _invertTextAndImage = false;
        [SerializeField] private float _imageSize = 30.0f;

        [Header("Base Image")] 
        [SerializeField] private bool _useImage = true;
        [SerializeField] private Sprite _baseNormalSprite;
        [SerializeField] private Color _baseNormalColor = new Color(255.0f, 255.0f, 255.0f, 1.0f);
        [Space(10)]
        [SerializeField] private Sprite _baseHighlightedSprite;
        [SerializeField] private Color _baseHighlightedColor = new Color(255.0f, 255.0f, 255.0f, 1.0f);
        [Space(10)]
        [SerializeField] private Sprite _baseSelectedSprite;
        [SerializeField] private Color _baseSelectedColor = new Color(255.0f, 255.0f, 255.0f, 1.0f);
        
        [Header("Outline Image")] [Space(30)]
        [SerializeField] private bool _useOutline = true;
        [SerializeField] private Sprite _outlineNormalSprite;
        [SerializeField] private Color _outlineNormalColor = new Color(255.0f, 255.0f, 255.0f, 1.0f);
        [Space(10)]
        [SerializeField] private Sprite _outlineHighlightedSprite;
        [SerializeField] private Color _outlineHighlightedColor = new Color(255.0f, 255.0f, 255.0f, 1.0f);
        [Space(10)]
        [SerializeField] private Sprite _outlineSelectedSprite;
        [SerializeField] private Color _outlineSelectedColor = new Color(255.0f, 255.0f, 255.0f, 1.0f);

        [Space(30)] [Header("Text")] 
        [SerializeField] private bool _useText = true;
        [SerializeField] private string _textContent;
        [SerializeField] private Color _textNormalColor = new Color(255.0f, 255.0f, 255.0f, 1.0f);
        [SerializeField] private Color _textHighLightedColor = new Color(255.0f, 255.0f, 255.0f, 1.0f);
        [SerializeField] private Color _textSelectedColor = new Color(255.0f, 255.0f, 255.0f, 1.0f);

        [Space(30)] [Header("States")] 
        [SerializeField] private bool _hasHighlightedState;
        [SerializeField] private bool _hasClickedState;
        [SerializeField] private EButtonState _previewState = EButtonState.Normal;

        
        private void OnValidate()
        {
            if (gameObject.activeSelf == false) return;
            bool needToReturned = false;

            if (_imagesContainer == null)
            {
                Debug.LogWarning($"{name} : ImagesParent is null !");
                needToReturned = true;
            }
            if (_baseImage == null)
            {
                Debug.LogError($"{name} : BaseImage is null !");
                needToReturned = true;
            }
            if (_outlineImage == null)
            {
                Debug.LogError($"{name} : OutlineImage is null !");
                needToReturned = true;
            }

            if (_text == null)
            {
                Debug.LogError($"{name} : Text is null !");
                needToReturned = true;
            }

            if (_spacer == null)
            {
                Debug.LogError($"{name} : Spacer is null !");
                needToReturned = true;
            }

            if (needToReturned)
            {
                Debug.Log("C'est ce gameObject le coupable : " + gameObject.name);
                return;
            }
            if (Application.isPlaying) return;
            
            // Set active state of image / outline / text
            _imagesContainer.SetActive(_useImage);
            _outlineImage.gameObject.SetActive(_useOutline);
            _text.gameObject.SetActive(_useText);

            // Set the distance between the image and the text
            _spacer.sizeDelta = new Vector2(_spaceBetweenImagesAndText, _spacer.sizeDelta.y);

            _imagesContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(_imageSize, _imagesContainer.GetComponent<RectTransform>().sizeDelta.y);
            _text.text = _textContent;

            if (_invertTextAndImage)
            {
                _imagesContainer.transform.SetAsLastSibling();
                _text.transform.SetAsFirstSibling();
            }
            else
            {
                _imagesContainer.transform.SetAsFirstSibling();
                _text.transform.SetAsLastSibling();
            }
            
            // Preview the visual depending on the state
            switch (_previewState)
            {
                // None,
                // Normal,
                // Highlighted,
                // Clicked,
                // Selected,
                // Disabled,

                case EButtonState.Normal: 
                    SetNormalVisual();
                    break;   
                    
                case EButtonState.Highlighted: 
                    SetHighlightedVisual();
                    break;
                    
                case EButtonState.Clicked:
                    SetClickedVisual();
                    break;  
                        
                case EButtonState.Selected:
                    
                    break;
                    
                case EButtonState.Disabled: 
                    break;  
                    
                default:
                    break;
;            }
        }



        public void SetVisual(EButtonState buttonState)
        {
            switch (buttonState)
            {
                case EButtonState.Normal: 
                    SetNormalVisual();
                    break;   
                    
                case EButtonState.Highlighted: 
                    SetHighlightedVisual();
                    break;
                    
                case EButtonState.Clicked:
                    SetClickedVisual();
                    break;  
                        
                case EButtonState.Selected:
                    break;
                    
                case EButtonState.Disabled: 
                    break;  
                    
                default:
                    break;
            }
        }
        
        public void SetNormalVisual()
        {
            _previewState = EButtonState.Normal;
            
            _baseImage.sprite = _baseNormalSprite;
            _baseImage.color = _baseNormalColor;
            
            _outlineImage.sprite = _outlineNormalSprite;
            _outlineImage.color = _outlineNormalColor;
            
            _text.color = _textNormalColor;
        }

        public void SetHighlightedVisual()
        {
            if (_hasHighlightedState == false || _previewState == EButtonState.Selected) return;
            _previewState = EButtonState.Highlighted;

            _baseImage.sprite = _baseHighlightedSprite;
            _baseImage.color = _baseHighlightedColor;
            
            _outlineImage.sprite = _outlineHighlightedSprite;
            _outlineImage.color = _outlineHighlightedColor;
            
            _text.color = _textHighLightedColor;
        }
        
        public void SetClickedVisual()
        {
            if (_hasClickedState == false) return;
            _previewState = EButtonState.Selected;

            _baseImage.sprite = _baseSelectedSprite;
            _baseImage.color = _baseSelectedColor;
            
            _outlineImage.sprite = _outlineSelectedSprite;
            _outlineImage.color = _outlineSelectedColor;
            
            _text.color = _textSelectedColor;
        }
    }
}
