using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    public class PlayerAnimation : MonoBehaviour
    {
        private Animator mouvementAnimator;

        private int x = 0, y = 0;

        // Start is called before the first frame update
        void Awake()
        {
            mouvementAnimator = GetComponentInChildren<Animator>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
