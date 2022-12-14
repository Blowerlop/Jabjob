using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    Vector3 target;
    [SerializeField]
    float speed;
    // Start is called before the first frame update
    public void Init(Vector3 _target)
    {
        target = _target;
        transform.LookAt(target);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }
}
