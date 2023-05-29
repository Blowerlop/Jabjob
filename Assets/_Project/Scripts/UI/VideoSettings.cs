using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Project.Utilities;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project
{
    public enum EAspectRation
    {
        None,
        AspectOther,
        Aspect4by3,
        Aspect16by9,
        Aspect21by9
    }

    public enum EFullScreenMode
    {
        None,
        ExclusiveFullScreen,
        BorderlessWindow,
        Windowed
    }


    public class VideoSettings : MonoBehaviour
    {
        #region Variables
        
        #region Resolution Relative
        [SerializeField] private DropDownExtended _screenSizeDropdown;
        [SerializeField] private DropDownExtended _screenDisplayDropdown;

        
        private List<Resolution> _screenResolutions = new List<Resolution>();
        private Resolution _currentResolution => new Resolution() { width = Screen.width, height = Screen.height };
        private Resolution _selectedResolution;

        private FullScreenMode _currentFullScreenMode => Screen.fullScreenMode;
        [SerializeField] [ReadOnlyField] private FullScreenMode _selectedFullScreenMode;

        [SerializeField] [ReadOnlyField] private EAspectRation _currentAspectRation;
        [SerializeField] [ReadOnlyField] private EAspectRation _currentSelectedAspectRatio;
        private EAspectRation _previousSelectedAspectRation;

        private bool _optionsRefreshed = true;


        public static Event<EAspectRation> onAspectRatioNotSupportedEvent =
            new Event<EAspectRation>(nameof(onAspectRatioNotSupportedEvent));

        public UnityEvent _onUseCurrentResolution = new UnityEvent();
        public UnityEvent _onUseAdvancedSettings = new UnityEvent();
        
        #endregion
        
        #region Rendering
        [SerializeField] private UnityEvent _onLowQualityEvent = new UnityEvent();
        [SerializeField] private UnityEvent _onMediumQualityEvent = new UnityEvent();
        [SerializeField] private UnityEvent _onHighQualityEvent = new UnityEvent();
        
        [SerializeField] private TMP_InputField _maxFrameRateInputField;
        [SerializeField] private Slider _maxFrameRateSlider;
        #endregion
        
        
#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] [ReadOnlyField] private string selectedResolutionString;
#endif
        #endregion
        
        
        #region Updates
        private IEnumerator Start()
        {
            yield return null;
            
            CheckAllAspectRatioSupportedState();
            
            
            
            _screenSizeDropdown.captionText.text = ResolutionToString(_currentResolution);
            _screenDisplayDropdown.captionText.text = ConvertBuiltInFullScreenModeToMine(_currentFullScreenMode).ToString().SeparateContent();

            

            _currentAspectRation = CalculateScreenAspectRationApproximation(_currentResolution);
            
            _previousSelectedAspectRation = EAspectRation.None;
            _currentSelectedAspectRatio = _currentAspectRation;

            _selectedFullScreenMode = _currentFullScreenMode;


            switch (VideoManager.instance.currentQualityIndex)
            {
                case 0:
                    SetLowQuality();
                    Debug.Log("aaaaaaaaaaaaaaaaa");

                    break;
                    
                case 1:
                    SetMediumQuality();
                    Debug.Log("aaaaaaaaaaaaaaaaa");

                    break;
                
                case 2:
                    SetHighQuality();
                    Debug.Log("aaaaaaaaaaaaaaaaa");
                    break;
                
                case 3:
                    break;
            }

            SetMaximumFrameRate(VideoManager.instance.GetFrameRate);
            SetMaximumFrameRateSlider(VideoManager.instance.GetFrameRate.ToString());
            
            SelectAspectRatio(_currentAspectRation);


            if (AreResolutionsEqual(_currentResolution.height, Display.main.systemHeight, _currentResolution.width,
                    Display.main.systemWidth) && _currentFullScreenMode == FullScreenMode.ExclusiveFullScreen)
            {
                _onUseCurrentResolution.Invoke();
            }
            else
            {
                _onUseAdvancedSettings.Invoke();
            }
            
            #endregion
        }


        private void OnEnable()
        {
            _screenSizeDropdown.onDropdownItemCreatedEvent.Subscribe(SetDropdownItemResolutionContainer, this);
            _screenSizeDropdown.onAfterDropdownShowEvent.Subscribe(SelectCurrentResolutionDropdownItem, this);

            _screenDisplayDropdown.onDropdownItemCreatedEvent.Subscribe(SetDropdownItemFullScreenModeContainer, this);
            _screenDisplayDropdown.onAfterDropdownShowEvent.Subscribe(SelectCurrentFullScreenModeDropdownItem, this);
            
            _maxFrameRateInputField.onDeselect.AddListenerExtended(SetMaximumFrameRate);
            _maxFrameRateInputField.onEndEdit.AddListenerExtended(SetMaximumFrameRateSlider);
        }
        
        private void OnDisable()
        {
            _screenSizeDropdown.onDropdownItemCreatedEvent.Unsubscribe(SetDropdownItemResolutionContainer);
            _screenSizeDropdown.onAfterDropdownShowEvent.Unsubscribe(SelectCurrentResolutionDropdownItem);

            _screenDisplayDropdown.onDropdownItemCreatedEvent.Unsubscribe(SetDropdownItemFullScreenModeContainer);
            _screenDisplayDropdown.onAfterDropdownShowEvent.Unsubscribe(SelectCurrentFullScreenModeDropdownItem);
            
            _maxFrameRateInputField.onDeselect.RemoveListenerExtended(SetMaximumFrameRate);
            _maxFrameRateInputField.onEndEdit.RemoveListenerExtended(SetMaximumFrameRateSlider);
        }

        
        
        #region Methods
        #region Resolutions Relative
        public void ApplyResolutionRelativeSettings()
        {
            Screen.SetResolution(_selectedResolution.width, _selectedResolution.height, _selectedFullScreenMode);
            
            _currentAspectRation = _currentSelectedAspectRatio;
            
            Debug.Log("Apply resolution !");
        }

        #region Resolution
        private void RefreshResolutionsOptions()
        {
            // if (_optionsRefreshed == false) return;

            _screenSizeDropdown.ClearOptions();
            _screenResolutions = GetAllResolutionOfTheSelectedAspectRatio(true);
            _screenSizeDropdown.AddOptions(ResolutionsToString(_screenResolutions));
            
            if (_currentAspectRation == _currentSelectedAspectRatio)
            {
                _screenSizeDropdown.captionText.text = ResolutionToString(_currentResolution);
                SelectResolution(_currentResolution);
            }
            else
            {
                SelectResolution(_screenResolutions[0]);
            }
        }
        
        private void SetDropdownItemResolutionContainer(RectTransform rectTransform, int index)
        {
            rectTransform.GetComponent<ResolutionContainer>().resolution = _screenResolutions[index];
        }
        
        private void SelectCurrentResolutionDropdownItem()
        {
            if (_optionsRefreshed == false) return;

            
            foreach (var dropDownItem in _screenSizeDropdown.dropdownItems)
            {
                if (AreResolutionsEqual(dropDownItem.GetComponent<ResolutionContainer>().resolution, _currentResolution))
                {
                    _screenSizeDropdown.dropdownItems[0].GetComponent<Toggle>().SetIsOnWithoutNotify(false);
                    dropDownItem.GetComponent<Toggle>().isOn = true;

                    // Opened automatically the dropdown after being closed when setting the toggle on 'isOn';
                    StartCoroutine(UtilitiesClass.WaitForFramesAndDoActionCoroutine(1,
                        () => _screenSizeDropdown.OnPointerClick(null)));
                    break;
                }
            }

            _optionsRefreshed = false;
        }
        
        public void SelectResolution(ResolutionContainer resolutionContainer) =>
            SelectResolution(resolutionContainer.resolution);

        private void SelectResolution(Resolution resolution)
        {
            _selectedResolution = resolution;
            
#if UNITY_EDITOR
            selectedResolutionString = resolution.ToString();
#endif
        }
        
        private List<Resolution> GetAllResolutionOfTheSelectedAspectRatio(bool revert)
        {
            List<Resolution> resolutions = new List<Resolution>();

            // Screen.resolutions.ForEach(resolution =>
            // {
            //     if (CalculateScreenAspectRationApproximation(resolution) == _currentSelectedAspectRatio)
            //     {
            //         bool hasBreak = false;
            //         for (int i = 0; i < resolutions.Count; i++)
            //         {
            //             if (ResolutionToString(resolutions[i]) == ResolutionToString(resolution))
            //             {
            //                 hasBreak = true;
            //                 break;
            //             }
            //         }
            //
            //         if (hasBreak == false)
            //         {
            //             resolutions.Add(resolution);
            //         }
            //     }
            // });
            
            Screen.resolutions.ForEach(resolution =>
            {
                if (CalculateScreenAspectRationApproximation(resolution) == _currentSelectedAspectRatio)
                {
                    bool hasBreak = false;
                    for (int i = 0; i < resolutions.Count; i++)
                    {
                        if (AreResolutionsEqual(resolutions[i], resolution) || resolution.height > Display.main.systemHeight ||
                            resolution.width > Display.main.systemWidth)
                        {
                            hasBreak = true;
                            break;
                        }
                    }
            
                    if (hasBreak == false)
                    {
                        resolutions.Add(resolution);
                    }
                }
            });

            if (revert)
            {
                resolutions.Reverse();
            }
            
            return resolutions;
        }

        private bool AreResolutionsEqual(Resolution resolution1, Resolution resolution2) 
            => AreResolutionsEqual(resolution1.height, resolution2.height, resolution1.width, resolution2.width);
        
        private bool AreResolutionsEqual(int height1, int height2, int width1, int width2) 
            => height1 == height2 && width1 == width2;
            
        
        private List<string> ResolutionsToString(List<Resolution> resolutions)
        {
            List<string> resolutionStrings = new List<string>();

            resolutions.ForEach(resolution => resolutionStrings.Add(ResolutionToString(resolution)));

            return resolutionStrings;
        }
        
        private string ResolutionToString(Resolution resolution)
        {
            return $"{(object)resolution.width} x {(object)resolution.height}";
        }


        public void UseNativeResolution()
        {
            Debug.Log($"Native Resolution : {Display.main.systemWidth} x {Display.main.systemHeight}");
            
            SelectResolution(GetResolution(Display.main.systemWidth, Display.main.systemHeight, false));
            _screenSizeDropdown.captionText.text = ResolutionToString(_selectedResolution);

            SelectAspectRatio(CalculateScreenAspectRationApproximation(_selectedResolution));
            
            SelectFullScreenMode(FullScreenMode.ExclusiveFullScreen);
            // _screenDisplayDropdown.captionText.text = ConvertBuiltInFullScreenModeToMine(_selectedFullScreenMode).ToString().SeparateContent();
            _screenDisplayDropdown.captionText.text = "Exclusive FullScreen";
        }

        private Resolution GetResolution(int width, int height, bool onlyFromTheCurrentResolutions = true)
        {
            if (onlyFromTheCurrentResolutions)
            {
                foreach (var resolution in _screenResolutions)
                {
                    if (resolution.width == width && resolution.height == height)
                    {
                        return resolution;
                    }
                }
            }
            else
            {
                foreach (var resolution in Screen.resolutions)
                {
                    if (resolution.width == width && resolution.height == height)
                    {
                        return resolution;
                    }
                }
            }
            

            Debug.Log("This screen does not support the selected width and height");
            return new Resolution();
        }
        #endregion
        
        
        #region FullScreenMode
        
        private void SetDropdownItemFullScreenModeContainer(RectTransform rectTransform, int index)
        {
            if (index == 0)
            {
                rectTransform.GetComponent<DisplayModeContainer>().fullScreenMode =
                    FullScreenMode.ExclusiveFullScreen;
            }
            else if (index == 1)
            {
                rectTransform.GetComponent<DisplayModeContainer>().fullScreenMode = FullScreenMode.FullScreenWindow;
            }
            else if (index == 2)
            {
                rectTransform.GetComponent<DisplayModeContainer>().fullScreenMode = FullScreenMode.Windowed;
            }
            else
            {
                Debug.Log("ERRORRRRRRR ITEMS PAS BON NOMBRE");
            }
        }
        
        private void SelectCurrentFullScreenModeDropdownItem()
        {
            for (int i = 0; i < _screenDisplayDropdown.dropdownItems.Count; i++)
            {
                if (_screenDisplayDropdown.dropdownItems[i].GetComponent<DisplayModeContainer>().fullScreenMode ==
                    _currentFullScreenMode)
                {
                    _screenDisplayDropdown.dropdownItems[0].GetComponent<Toggle>().SetIsOnWithoutNotify(false);
                    _screenDisplayDropdown.dropdownItems[i].GetComponent<Toggle>().isOn = true;

                    // Opened automatically the dropdown after being closed when setting the toggle on 'isOn';
                    StartCoroutine(UtilitiesClass.WaitForFramesAndDoActionCoroutine(1,
                        () => _screenDisplayDropdown.OnPointerClick(null)));
                    break;
                }
            }
            
            _screenDisplayDropdown.onAfterDropdownShowEvent.Unsubscribe(SelectCurrentFullScreenModeDropdownItem);
        }
        
        public void SelectFullScreenMode(DisplayModeContainer displayModeContainer)
        {
            SelectFullScreenMode(displayModeContainer.fullScreenMode);
        }
        public void SelectFullScreenMode(FullScreenMode fullScreenMode)
        {
            _selectedFullScreenMode = fullScreenMode;
        }
        
        private EFullScreenMode ConvertBuiltInFullScreenModeToMine(FullScreenMode fullScreenMode)
        {
            switch (fullScreenMode)
            {
                case FullScreenMode.ExclusiveFullScreen:
                    return EFullScreenMode.ExclusiveFullScreen;
                
                case FullScreenMode.FullScreenWindow:
                    return EFullScreenMode.BorderlessWindow;
                
                case FullScreenMode.Windowed:
                    return EFullScreenMode.Windowed;
                
                default:
                    return EFullScreenMode.None;
            }
        }
        
        private FullScreenMode ConvertMineFullScreenToBuiltIn(EFullScreenMode fullScreenMode)
        {
            switch (fullScreenMode)
            {
                case EFullScreenMode.ExclusiveFullScreen:
                    return FullScreenMode.ExclusiveFullScreen;
                
                case EFullScreenMode.BorderlessWindow:
                    return FullScreenMode.FullScreenWindow;
                
                case EFullScreenMode.Windowed:
                    return FullScreenMode.Windowed;
                
                default:
                    Debug.LogError("Ca sent pas bon");
                    return default;
            }
        }
        #endregion
        
        
        #region AspectRation
        public void SelectAspectRatio(AspectRatioContainer aspectRationContainer)
        {
            SelectAspectRatio(aspectRationContainer.aspectRation);
        }
        
        private void SelectAspectRatio(EAspectRation aspectRation)
        {
            _currentSelectedAspectRatio = aspectRation;

            if (_previousSelectedAspectRation != _currentSelectedAspectRatio)
            {
                _optionsRefreshed = true;
                RefreshResolutionsOptions();
                _previousSelectedAspectRation = _currentSelectedAspectRatio;

            }
        }
        private void CheckAllAspectRatioSupportedState()
        {
            bool screenSupport4By3 = false;
            bool screenSupport16By9 = false;
            bool screenSupport21By9 = false;
            
            Screen.resolutions.ForEach(x =>
            {
                EAspectRation aspectRation = CalculateScreenAspectRationApproximation(x);
                if (aspectRation == EAspectRation.Aspect4by3)
                {
                    screenSupport4By3 = true;
                }
                else if (aspectRation == EAspectRation.Aspect16by9)
                {
                    screenSupport16By9 = true;
                }
                else if (aspectRation == EAspectRation.Aspect21by9)
                {
                    screenSupport21By9 = true;
                }
            });
            
            if (screenSupport4By3 == false)
            {
                onAspectRatioNotSupportedEvent.Invoke(this, true, EAspectRation.Aspect4by3);
            }
            if (screenSupport16By9 == false)
            {
                onAspectRatioNotSupportedEvent.Invoke(this, true, EAspectRation.Aspect16by9);
            }
            if (screenSupport21By9 == false)
            {
                onAspectRatioNotSupportedEvent.Invoke(this, true, EAspectRation.Aspect21by9);
            }
            
            onAspectRatioNotSupportedEvent.Invoke(this, true, EAspectRation.None);
        }
        
        private EAspectRation CalculateScreenAspectRationApproximation(Resolution resolution) =>
            CalculateScreenAspectRationApproximation(resolution.width, resolution.height);
        
        private EAspectRation CalculateScreenAspectRationApproximation(float width, float height)
        {
            if (width / height >= 2.37f)
            {
                return EAspectRation.Aspect21by9;
            }
            else if (width / height >= 1.7f)
            {
                return EAspectRation.Aspect16by9;
            }
            else if (width / height >= 1.3f)
                return EAspectRation.Aspect4by3;
            else
            {
                return EAspectRation.AspectOther;
            }
        }
        #endregion
        #endregion

        #region  Rendering

        public void SetLowQuality()
        {
            _onLowQualityEvent.Invoke();
            VideoManager.instance.SetLowQuality();
        }

        public void SetMediumQuality()
        {
            VideoManager.instance.SetMediumQuality();
            _onMediumQualityEvent.Invoke();
        }

        public void SetHighQuality()
        {
            VideoManager.instance.SetHighQuality();
            _onHighQualityEvent.Invoke();
        }

        public void SetCustomQuality() => VideoManager.instance.SetCustomQuality();

        public void SetMaximumFrameRateSlider(string targetFrameRate)
        {
            _maxFrameRateSlider.value = float.Parse(targetFrameRate);
        }
        
        private void SetMaximumFrameRate(string targetFrameRate)
        {
            SetMaximumFrameRate(float.Parse(targetFrameRate));
        }

        private void SetMaximumFrameRate(float targetFrameRate)
        {
            VideoManager.instance.SetMaximumFrameRate((int)targetFrameRate);
            _maxFrameRateInputField.text = targetFrameRate.ToString(CultureInfo.InvariantCulture);
        }

        #endregion

        #endregion
    }
}
