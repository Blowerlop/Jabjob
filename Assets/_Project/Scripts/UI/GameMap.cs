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
        }

        private void Invoke()
        {
            call.Invoke(this, true, this);
        }
    }
}
