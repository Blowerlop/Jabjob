using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GunScript : MonoBehaviour
{
    public WeaponSO actualWeapon;

    [SerializeField]
    GameObject gun, projectilePrefab, playerCamera;

    [SerializeField]
    int range, ammo, maxAmmo;

    float nextShoot, shootRate;

    [SerializeField]
    LayerMask layerToAim;

    bool canShoot = true;

    void Start()
    {
        //Init values
        ChangeWeapon(actualWeapon);
        InputManager.instance.reload.AddListener(Reload);
        
        //Set Cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (InputManager.instance.shoot && (actualWeapon.riffle || canShoot))
        {
            if(Time.time >= nextShoot && ammo >0)
            {
                if (!actualWeapon.riffle)
                {
                    canShoot = false;
                }
                nextShoot = Time.time + shootRate;
                ammo--;
                Shoot();
            }
        }
        if (!InputManager.instance.shoot)
        {
            canShoot = true;
        }    
    }

    /// <summary>
    /// Shoot and instanciate the projectile in term of the actual weapon 
    /// </summary>
    public void Shoot()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f));
        if (Physics.Raycast(ray, out hit, range, layerToAim))
        {
            if (actualWeapon.spray)
            {
                for (int i = 0; i < actualWeapon.bulletNumber; i++)
                {
                    GameObject go = Instantiate(projectilePrefab, gun.transform.position, Quaternion.identity);
                    go.GetComponent<ProjectileScript>().Init(hit.point, actualWeapon.dispertion);
                }
            }
            else
            {
                GameObject go = Instantiate(projectilePrefab, gun.transform.position, Quaternion.identity);
                go.GetComponent<ProjectileScript>().Init(hit.point, actualWeapon.dispertion);
            }
        }
        else 
        {
            if (actualWeapon.spray)
            {
                for (int i = 0; i < actualWeapon.bulletNumber; i++)
                {
                    GameObject go = Instantiate(projectilePrefab, gun.transform.position, gun.transform.rotation);
                    go.GetComponent<ProjectileScript>().Init(actualWeapon.dispertion);
                }
            }
            else
            {
                GameObject go = Instantiate(projectilePrefab, gun.transform.position, gun.transform.rotation);
                go.GetComponent<ProjectileScript>().Init(actualWeapon.dispertion);
            }
        }
    }

    /// <summary>
    /// Reload the weapon 
    /// </summary>
    public void Reload()
    {
        if (ammo < maxAmmo)
        {
            ammo = maxAmmo;
        }
    }

    /// <summary>
    /// Change the actual weapon with <paramref name="newWeapon" />
    /// </summary>
    public void ChangeWeapon(WeaponSO newWeapon)
    {
        actualWeapon = newWeapon;
        gun = Instantiate(actualWeapon.model, playerCamera.transform).GetComponent<WeaponScript>().origin;
        maxAmmo = actualWeapon.maxAmmo;
        ammo = maxAmmo;
        shootRate = actualWeapon.shootRate;
    }
}
