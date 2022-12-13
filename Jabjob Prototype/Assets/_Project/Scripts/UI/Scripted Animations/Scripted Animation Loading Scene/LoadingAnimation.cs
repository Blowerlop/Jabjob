using System;
using UnityEngine;

namespace _Project.Scripts.Scripted_Animations.Scripted_Animation_Loading_Scene
{
    public class LoadingAnimation : MonoBehaviour
    {
        [SerializeField] private float _animationSpeed = 100;
    
        private void Update()
        {
            transform.Rotate(Vector3.forward * (-_animationSpeed * Time.deltaTime));
        }
    }
}