using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Project
{
    [CreateAssetMenu(fileName = "Game Map", menuName = "Assets/SO/GameMap")]
    public class SOGameMaps : ScriptableObject
    {
        public string sceneName;
        public string mapName;
        public Sprite sceneWallpaper;
    }
}
