using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float playerHealth = 100;
    [SerializeField] Slider slider;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Damage(5);
        }
    }

    public void Damage(float damage)
    {
        playerHealth -= damage;
        slider.value = playerHealth / 100;
    }
}
