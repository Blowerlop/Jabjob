using System;
using UnityEngine;

namespace Project
{
    public class OnTriggerEnterEventClass : MonoBehaviour
    {
        public readonly Event<Collider> @event = new Event<Collider>(nameof(@event));
        
        private void OnTriggerEnter(Collider other)
        {
            @event.Invoke(this, false, other);
        }
    }
}
