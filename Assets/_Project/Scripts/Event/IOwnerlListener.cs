using System;
using UnityEngine;

namespace Project
{
    public interface IOwnerlListener
    {
        public Action action { get; set; }
        public bool IsPlayerOwner();
    }
}