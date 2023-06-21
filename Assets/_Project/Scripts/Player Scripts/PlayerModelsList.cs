using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    [System.Serializable]
    public class PlayerModelsList
    {
        public string name;
        public Mesh meshModel;
        public Material materialModel;
        public Mesh meshHands;
        public Material materialHands;
        public Sprite portrait;
        public bool isMale;
    }
}
