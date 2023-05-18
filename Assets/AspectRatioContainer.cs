using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    public class AspectRatioContainer : MonoBehaviour
    {
        public EAspectRation aspectRation;

        
        private void Awake()
        {
            Settings.onAspectRatioNotSupportedEvent.Subscribe(IsAspectRatioNotSupported, this);
        }

        private void OnDestroy()
        {
            Settings.onAspectRatioNotSupportedEvent.Unsubscribe(IsAspectRatioNotSupported);
        }

        private void IsAspectRatioNotSupported(EAspectRation aspectRation)
        {
            if (this.aspectRation == aspectRation)
            {
                Destroy(this.gameObject);
            }
            else if (aspectRation == EAspectRation.None)
            {
                Settings.onAspectRatioNotSupportedEvent.Unsubscribe(IsAspectRatioNotSupported);
            }
        }
    }
}
