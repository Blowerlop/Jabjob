using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    public class CameraMovementForTest : MonoBehaviour
    {
        [SerializeField] float mouseSensitivity = 100f;

        [SerializeField] Transform player;

        float xRotation = 0f;

        // Start is called before the first frame update
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked; //Bloque le curseur au centre
        }

        // Update is called once per frame
        void Update()
        {
            //Permet de faire regarder la caméra en fonction des mouvement de la souris
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -50f, 50f);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            player.Rotate(Vector3.up * mouseX);
        }
    }
}
