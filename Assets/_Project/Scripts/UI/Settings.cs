using System;
using System.Collections;
using System.Collections.Generic;
using Project.Utilities;
using TMPro;
using UnityEditor;
using UnityEngine;
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


    public class Settings : MonoBehaviour
    {
        [SerializeField] private DropDownExtended _screenSizeDropdown;
        [SerializeField] private DropDownExtended _screenDisplayDropdown;
        public List<Resolution> screenResolutions = new List<Resolution>();
        public string clickedResolutionString;
        public Resolution clickedResolution;

        public FullScreenMode currentFullScreenMode;
        public FullScreenMode selectedFullScreenMode;



        private Resolution _currentResolution;
        public EAspectRation _currentAspectRation;

        private Resolution _selectedResolution;


        public EAspectRation _previousSelectedAspectRation;
        public EAspectRation _currentSelectedAspectRatio;

        private bool _optionsRefreshed = true;


        public static Event<EAspectRation> onAspectRatioNotSupportedEvent =
            new Event<EAspectRation>(nameof(onAspectRatioNotSupportedEvent));

        private void Start()
        {
            CheckAllAspectRatioSupportedState();
            
            // _screenSizeDropdown.onBeforeDropdownShowEvent.Subscribe(RefreshResolutionOptions, this);
            _screenSizeDropdown.onDropdownItemCreatedEvent.Subscribe(OnDropdownItemCreated, this);
            _screenSizeDropdown.onAfterDropdownShowEvent.Subscribe(AAAA, this);

            _screenDisplayDropdown.onDropdownItemCreatedEvent.Subscribe(SetFullScreenModeContainer, this);


            // _screenSizeDropdown.captionText.text = Screen.currentResolution.ToString();
            _screenSizeDropdown.captionText.text = ResolutionToString(Screen.currentResolution);

            Debug.Log(Screen.currentResolution.height);
            Debug.Log(Screen.currentResolution.width);

            _currentResolution = Screen.currentResolution;
            _currentAspectRation = CalculateScreenAspectRationApproximation(_currentResolution);

            _selectedResolution = _currentResolution;

            _previousSelectedAspectRation = EAspectRation.None;
            _currentSelectedAspectRatio = _currentAspectRation;

            SelectAspectRatio(_currentAspectRation);
        }


        private void RefreshResolutionOptions()
        {
            // if (_currentAspectRation == _selectedAspectRation) return;

            // if (_currentAspectRation != _currentSelectedAspectRation) _screenSizeDropdown.ClearOptions();
            // screenResolutions = GetAllResolutionOfTheSelectedAspectRatio();
            // if (_currentAspectRation != _currentSelectedAspectRation || _screenSizeDropdown.options.Count == 0)
            // {
            //     _screenSizeDropdown.AddOptions(ResolutionsToString(screenResolutions));
            //     _newOptions = true;
            // }
            // else
            // {
            //     _newOptions = false;
            // }

            if (_optionsRefreshed == false) return;

            _screenSizeDropdown.ClearOptions();
            screenResolutions = GetAllResolutionOfTheSelectedAspectRatio();
            _screenSizeDropdown.AddOptions(ResolutionsToString(screenResolutions));
            // _optionsRefreshed = true;

            if (_currentAspectRation == _currentSelectedAspectRatio)
            {
                _screenSizeDropdown.captionText.text = ResolutionToString(_currentResolution);
                SelectResolution(_currentResolution);

            }
            else
            {
                SelectResolution(screenResolutions[0]);
            }
        }

        private void OnDropdownItemCreated(RectTransform rectTransform, int index)
        {
            // if (_previousSelectedAspectRation == _currentSelectedAspectRation && _optionsRefreshed == false) return;
            // if (_optionsRefreshed == false) return;

            rectTransform.GetComponent<ResolutionContainer>().resolution = screenResolutions[index];
            // There is an Event set in the editor for the item;
        }

        private void AAAA()
        {
            if (_optionsRefreshed == false) return;


            for (int i = 0; i < _screenSizeDropdown.dropdownItems.Count; i++)
            {
                if (AreResolutionsEqual(
                        _screenSizeDropdown.dropdownItems[i].GetComponent<ResolutionContainer>().resolution,
                        Screen.currentResolution))
                {
                    Debug.Log("ici ca passe");
                    _screenSizeDropdown.dropdownItems[0].GetComponent<Toggle>().SetIsOnWithoutNotify(false);
                    ;
                    Debug.Log("Resolution : " + _screenSizeDropdown.dropdownItems[i].GetComponent<ResolutionContainer>()
                        .resolution);
                    // _screenSizeDropdown.captionText.text = Screen.currentResolution.ToString();
                    _screenSizeDropdown.dropdownItems[i].GetComponent<Toggle>().isOn = true;
                    // _screenSizeDropdown.dropdownItems[i].GetComponent<Toggle>().Select();

                    StartCoroutine(UtilitiesClass.WaitForFramesAndDoActionCoroutine(1,
                        () => _screenSizeDropdown.OnPointerClick(null)));
                    break;
                }
            }

            if (_screenSizeDropdown.dropdownItems[0].GetComponent<Toggle>().isOn)
            {
                _screenSizeDropdown.dropdownItems[0].GetComponent<Toggle>().Select();
            }

            _optionsRefreshed = false;
        }

        private List<Resolution> GetAllResolutionOfTheSelectedAspectRatio()
        {
            List<Resolution> resolutions = new List<Resolution>();

            Screen.resolutions.ForEach(resolution =>
            {
                if (CalculateScreenAspectRationApproximation(resolution) == _currentSelectedAspectRatio)
                {
                    bool hasBreak = false;
                    for (int i = 0; i < resolutions.Count; i++)
                    {
                        if (ResolutionToString(resolutions[i]) == ResolutionToString(resolution))
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

            return resolutions;
        }

        private List<string> ResolutionsToString(List<Resolution> resolutions)
        {
            List<string> resolutionStrings = new List<string>();

            resolutions.ForEach(resolution => resolutionStrings.Add(ResolutionToString(resolution)));

            return resolutionStrings;
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

        public void SelectResolution(ResolutionContainer resolutionContainer) =>
            SelectResolution(resolutionContainer.resolution);

        public void SelectResolution(Resolution resolution)
        {
            clickedResolutionString = resolution.ToString();
            clickedResolution = resolution;
        }

        public void SelectDisplayMode(FullScreenModeContainer fullScreenMode)
        {
            selectedFullScreenMode = fullScreenMode.fullScreenMode;
        }

        public void SetResolution()
        {
            if (AreResolutionsEqual(_currentResolution, clickedResolution))
            {
                Screen.fullScreenMode = selectedFullScreenMode;
            }
            else
            {
                Screen.SetResolution(clickedResolution.width, clickedResolution.height, selectedFullScreenMode,
                    0);
            }

            _currentResolution = clickedResolution;
            currentFullScreenMode = selectedFullScreenMode;
            _currentAspectRation = _currentSelectedAspectRatio;
        }

        public bool AreResolutionsEqual(Resolution resolution1, Resolution resolution2) =>
            resolution1.height == resolution2.height && resolution1.width == resolution2.width &&
            resolution1.refreshRate == resolution2.refreshRate;

        public void SelectAspectRatio(AspectRatioContainer aspectRationContainer)
        {
            SelectAspectRatio(aspectRationContainer.aspectRation);
        }

        public void SelectAspectRatio(EAspectRation aspectRation)
        {
            _currentSelectedAspectRatio = aspectRation;

            if (_previousSelectedAspectRation != _currentSelectedAspectRatio)
            {
                _optionsRefreshed = true;
                RefreshResolutionOptions();
                _previousSelectedAspectRation = _currentSelectedAspectRatio;

            }
        }


        private void SetFullScreenModeContainer(RectTransform rectTransform, int index)
        {
            if (index == 0)
            {
                rectTransform.GetComponent<FullScreenModeContainer>().fullScreenMode =
                    FullScreenMode.ExclusiveFullScreen;
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

        private string ResolutionToString(Resolution resolution)
        {
            return $"{(object)resolution.width} x {(object)resolution.height}";
        }
    }
}
