using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    public class PlayerMovementForTest : MonoBehaviour
    {
        [SerializeField] CharacterController controller;

        [SerializeField] float speed = 8f;
        [SerializeField] float gravity = -9.81f;
        [SerializeField] float jumpHeight = 3f;

        [SerializeField] Transform groundCheck;
        [SerializeField] float groundDistance = 0.4f;
        [SerializeField] LayerMask groundMask;

        Vector3 velocity;
        public bool isGrounded;

        private void Update()
        {
            //Regarde si le joueur est au sol
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                speed = 12f;
            }
            else
            {
                speed = 8f;
            }

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            //On effectue les mouvement selon les touche
            Vector3 move = transform.right * x + transform.forward * z;

            controller.Move(move * speed * Time.deltaTime);

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            velocity.y += gravity * Time.deltaTime;

            controller.Move(velocity * Time.deltaTime);
        }
    }
}
