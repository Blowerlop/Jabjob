using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class GunScript : NetworkBehaviour
{
    public WeaponSO actualWeapon;

    [SerializeField]
    GameObject gun, projectilePrefab, playerCamera;

    [SerializeField]
    int range, maxAmmo;

    public int ammo;

    float nextShoot, shootRate;

    [SerializeField]
    LayerMask layerToAim;

    bool canShoot = true;

    [SerializeField] private Camera _camera;

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
        if (IsOwner == false) return;

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
                LocalShoot();
                ShootServerRpc();
            }
        }
        if (!InputManager.instance.shoot)
        {
            canShoot = true;
        }    
    }

    [ServerRpc]
    private void ShootServerRpc()
    {
        ShootClientRpc();
    }


    [ClientRpc]
    private void ShootClientRpc()
    {
        if (IsOwner == false) LocalShoot();
    }

    /// <summary>
    /// Shoot and instanciate the projectile in term of the actual weapon 
    /// </summary>
    public void LocalShoot()
    {
        RaycastHit hit;
        Ray ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f));
        if (Physics.Raycast(ray, out hit, range, layerToAim))
        {
            if (actualWeapon.spray)
            {
                for (int i = 0; i < actualWeapon.bulletNumber; i++)
                {
                    GameObject go = ObjectPoolingManager.instance.GetObject();
                    go.transform.position = gun.transform.position;
                    go.transform.rotation = Quaternion.identity;
                    go.GetComponent<ProjectileScript>().Init(hit.point, actualWeapon.dispertion);
                }
            }
            else
            {
                GameObject go = ObjectPoolingManager.instance.GetObject();
                go.transform.position = gun.transform.position;
                go.transform.rotation = Quaternion.identity;
                go.GetComponent<ProjectileScript>().Init(hit.point, actualWeapon.dispertion);
            }
        }
        else 
        {
            if (actualWeapon.spray)
            {
                for (int i = 0; i < actualWeapon.bulletNumber; i++)
                {
                    GameObject go = ObjectPoolingManager.instance.GetObject();
                    go.transform.position = gun.transform.position;
                    go.transform.rotation = gun.transform.rotation;
                    go.GetComponent<ProjectileScript>().Init(actualWeapon.dispertion);
                }
            }
            else
            {
                GameObject go = ObjectPoolingManager.instance.GetObject();
                go.transform.position = gun.transform.position;
                go.transform.rotation = gun.transform.rotation;
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
