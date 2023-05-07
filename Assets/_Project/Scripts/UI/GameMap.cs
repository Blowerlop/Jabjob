using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Project
{
    public class GameMap : MonoBehaviour
    {
        public SOGameMaps _gameMap;
        
        [SerializeField] private Button _button;
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Image _inactiveFilter, _selectedBorder;
        [SerializeField] private GameObject Tooltip;
        public Event<GameMap> call = new Event<GameMap>(nameof(call));
        
        private void Start()
        {
            InitializeUI();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(Invoke);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(Invoke);
        }


        public void InitializeUI()
        {
            if (_gameMap == null) return; 

            if (_image != null)
            {
                _image.sprite = _gameMap.sceneWallpaper;
            }

            if (_text != null)
            {
                _text.text = _gameMap.mapName;
            }

            if (!_gameMap.isAvailable)
            {
                _inactiveFilter.gameObject.SetActive(true);
                _button.onClick.RemoveListener(Invoke);
            }

        }
        private void Invoke()
        {
            call.Invoke(this, true, this);
        }
        public void ShowSelectedBorder()
        {
            _selectedBorder.enabled = true;
        }
        public void HideSelectedBorder()
        {
            _selectedBorder.enabled = false;
        }
        public void ShowToolTip()
        {
            if (_gameMap.isAvailable) return;
            Tooltip.SetActive(true);
        }
        public void HideToolTip()
        { 
            if (_gameMap.isAvailable) return;
            Tooltip.SetActive(false);
        }
    }
}
