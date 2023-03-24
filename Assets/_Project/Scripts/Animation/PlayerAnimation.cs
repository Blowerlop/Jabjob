using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    public class PlayerAnimation : MonoBehaviour
    {
        private float speed = 8f;
        private Animator movementAnimator;

        // Start is called before the first frame update
        void Awake()
        {
            movementAnimator = GetComponentInChildren<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector3 movemet = new Vector3(horizontal, 0f, vertical);

            if (movemet.magnitude > 0f)
            {
                movemet.Normalize();
                movemet *= speed * Time.deltaTime;
            }

            float velocityZ = Vector3.Dot(movemet.normalized, transform.forward);
            float velocityX = Vector3.Dot(movemet.normalized, transform.right);

            if (Input.GetKey(KeyCode.LeftShift))
            {
                velocityZ *= 2;
                velocityX *= 2;
            }

            movementAnimator.SetFloat("VelocityZ", velocityZ, 0.1f, Time.deltaTime);
            movementAnimator.SetFloat("VelocityX", velocityX, 0.1f, Time.deltaTime);
        }
    }
}
