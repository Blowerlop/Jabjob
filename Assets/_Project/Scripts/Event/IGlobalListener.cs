using System;
using UnityEngine;

namespace Project
{
    public interface IGlobalListener
    {
        public Action action { get; set; }
    }
}