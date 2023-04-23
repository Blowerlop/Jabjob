using System;
using System.Collections;
using System.Collections.Generic;
using Project.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Project
{
    public class GameMapsUI : MonoBehaviour
    {
        [SerializeField] private RectTransform _repo;
        [SerializeField] private bool _generateUIForAllTheMaps = true;
        [SerializeField] private SOGameMaps[] _gameMaps;
        [SerializeField] private GameMap _template;

        public List<GameMap> gameMaps;

        private void Awake()
        {
            // Apply();
            
        }
        

        private void InitializeUI()
        {
            if (_generateUIForAllTheMaps)
            {
                ////
            }
            else
            {
                for (int i = 0; i < _gameMaps.Length; i++)
                {
                    GameMap instance = Instantiate(_template, _repo);
                    instance._gameMap = _gameMaps[i];
                    instance.InitializeUI();
                }
            }
        }
        
        public void Apply()
        {
            _repo.DestroyChildren();

            InitializeUI();
        }

        public void AddListeners(Action<GameMap> call)
        {
            Apply();
            gameMaps = _repo.GetChildren<GameMap>();

            for (int i = 0; i < gameMaps.Count; i++)
            {
                gameMaps[i].call.Subscribe(call, this);
            }
        }
        
        public void RemoveListeners(Action<GameMap> call)
        {
            for (int i = 0; i < gameMaps.Count; i++)
            {
                gameMaps[i].call.Unsubscribe(call);
            }
        }
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(GameMapsUI))]
    public class GameMapUIEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GameMapsUI t = target as GameMapsUI;
            if (t == null) return;
            
            if (GUILayout.Button("Apply"))
            {
                 t.Apply();
            }
        }
    }
    #endif
}
