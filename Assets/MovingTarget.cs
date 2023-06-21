using System;
using System.Collections;
using System.Collections.Generic;
using Project.Utilities;
using UnityEngine;
using Random = System.Random;

namespace Project
{
    public class MovingTarget : MonoBehaviour
    {
        [SerializeField] private Vector2 _movingAreaX;
        [SerializeField] private Vector2 _movingAreaZ;
        private Rigidbody _rigidbody;
        [SerializeField] private float _pauseTime;
        [SerializeField] private Vector2 _speedRange;
        private Coroutine _movementCoroutine = null;
        [SerializeField] [ReadOnlyField] private bool _hasArrived = false;
        
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _movementCoroutine = StartCoroutine(MovementCoroutine());
        }
        
        private void OnDestroy()
        {
            StopCoroutine(_movementCoroutine);
        }


        private IEnumerator MovementCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(_pauseTime);
                _hasArrived = false;

                
                float axis1 = UnityEngine.Random.Range(_movingAreaX.x, _movingAreaX.y + 1);
                float axis2 = UnityEngine.Random.Range(_movingAreaZ.x, _movingAreaZ.y + 1);
                
                yield return MoveTo(new Vector3(axis1, _rigidbody.position.y, axis2));
            }
        }

        private IEnumerator MoveTo(Vector3 position)
        {
            while ((_rigidbody.position - position).sqrMagnitude > 0.1f * 0.1f)
            {
                Vector3 direction = (position - _rigidbody.position).normalized;
            
                _rigidbody.velocity = direction * UnityEngine.Random.Range(_speedRange.x, _speedRange.y + 1);
                yield return new WaitForFixedUpdate();
            }

            _rigidbody.velocity = Vector3.zero;
            _hasArrived = true;

        }
        
        
    }
}
