using System;
using _Project.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

public class OnSceneStartBehaviour : MonoBehaviour
{
    [SerializeField] private Text _loadingText;

    private void Awake()
    {
        SceneManager.loadText = _loadingText;
    }
}