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
            VideoSettings.onAspectRatioNotSupportedEvent.Subscribe(IsAspectRatioNotSupported, this);
        }

        private void OnDestroy()
        {
            VideoSettings.onAspectRatioNotSupportedEvent.Unsubscribe(IsAspectRatioNotSupported);
        }

        private void IsAspectRatioNotSupported(EAspectRation aspectRation)
        {
            if (this.aspectRation == aspectRation)
            {
                Destroy(this.gameObject);
            }
            else if (aspectRation == EAspectRation.None)
            {
                VideoSettings.onAspectRatioNotSupportedEvent.Unsubscribe(IsAspectRatioNotSupported);
            }
        }
    }
}
