using System;
using _Project.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

public class OnSceneStartBehaviour : MonoBehaviour
{
    [SerializeField] private Slider _loadingSlider;

    private void Awake()
    {
        SceneManager.loadSlider = _loadingSlider;
    }
}