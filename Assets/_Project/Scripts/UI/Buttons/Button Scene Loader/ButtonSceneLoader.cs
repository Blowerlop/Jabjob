using _Project.Scripts.Managers;
using UnityEngine;

public class ButtonSceneLoader : ButtonListener
{
    [SerializeField] private SceneManager.EScene _sceneToLoad;
    
    private void Start()
    {
        button.onClick.AddListener(() => SceneManager.LoadSceneAsync(_sceneToLoad));
    }
}