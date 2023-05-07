using System;
using UnityEngine;

namespace Project
{
    public class OnTriggerEnterEventClass : MonoBehaviour
    {
        private Rigidbody rb;
        public Collider[] a;
        public LayerMask layerMask;

        
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        public readonly Event<Collider> @event = new Event<Collider>(nameof(@event));
        
        private void OnTriggerEnter(Collider other)
        {
            @event.Invoke(this, false, other);
        }


        private void FixedUpdate()
        {
            HighSpeedCollision();
        }

        private void HighSpeedCollision()
        {
            a = Physics.OverlapSphere(rb.position, 0.25f, layerMask);
            if (a.Length != 0)
            {
                Debug.Log("OnCollision Overlap : " + a[0].name);
                OnTriggerEnter(a[0]);
            }
        }
    } 
}
