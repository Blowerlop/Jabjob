using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    [SerializeField]
    GameObject gun, projectilePrefab;

    [SerializeField]
    int range, ammo;

    [SerializeField]
    LayerMask layerToAim;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray,out hit, range, layerToAim))
        {
            GameObject go = Instantiate(projectilePrefab, gun.transform.position, Quaternion.identity);
            go.GetComponent<ProjectileScript>().Init(hit.point);
        }
    }
}
