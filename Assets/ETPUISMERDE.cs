using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    public class ETPUISMERDE : MonoBehaviour
    {
        private void OnDisable()
        {
            CursorManager.instance.Revert();
        }
    }
}
