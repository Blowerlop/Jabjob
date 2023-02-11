using System;
using System.Collections;
using System.Collections.Generic;
using Project;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponProjectile : NetworkBehaviour
{
    Vector3 target;

    [SerializeField] private int damage;
    [SerializeField] float speed;
    private bool _isOwner = false;


    /// <summary>
    /// Initialize bullet
    /// </summary>
    public void Init(Vector3 _target, float disp, bool isOwner)
    {
        this._isOwner = isOwner;
        target = _target;
        transform.LookAt(target);
        RandomizeRotation(disp);
        StartCoroutine(DestroyCD());
    }
    public void Init(float disp, bool isOwner)
    {
        this._isOwner = isOwner;
        RandomizeRotation(disp);
        StartCoroutine(DestroyCD());
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isOwner == false) return;
        
        if (other.TryGetComponent(out IHealthManagement healthManagement))
        {
            healthManagement.Damage(damage);
        }
    }

    /// <summary>
    /// Randomize the bullet rotation in term of the <paramref name="disp" />
    /// </summary>
    public void RandomizeRotation(float disp)
    {
        transform.Rotate(new Vector3(Random.Range(-disp, disp), Random.Range(-disp, disp), 0), Space.Self);
    }

    /// <summary>
    /// Destroy the GameObject after 10sec
    /// </summary>
    public IEnumerator DestroyCD()
    {
        yield return new WaitForSeconds(10);
        ObjectPoolingManager.instance.ReturnGameObject(gameObject);
    }
}
