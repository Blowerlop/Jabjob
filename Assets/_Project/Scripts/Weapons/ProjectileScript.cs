using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileScript : NetworkBehaviour
{
    Vector3 target;
    [SerializeField]
    float speed;


    /// <summary>
    /// Initialize bullet
    /// </summary>
    public void Init(Vector3 _target, float disp)
    {
        target = _target;
        transform.LookAt(target);
        RandomizeRotation(disp);
        StartCoroutine(DestroyCD());
    }
    public void Init(float disp)
    {
        RandomizeRotation(disp);
        StartCoroutine(DestroyCD());
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
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
