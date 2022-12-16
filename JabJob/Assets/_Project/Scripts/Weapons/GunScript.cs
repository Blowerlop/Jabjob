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
    int range, ammo, maxAmmo;

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
<<<<<<< HEAD
                LocalShoot();
                ShootServerRpc();
=======
                LocalShoot(gun.transform.position, gun.transform.rotation);
                ShootServerRpc(gun.transform.position, gun.transform.rotation);
>>>>>>> Weapons-Guillaume
            }
        }
        if (!InputManager.instance.shoot)
        {
            canShoot = true;
        }    
    }

    [ServerRpc]
<<<<<<< HEAD
    private void ShootServerRpc()
    {
        ShootClientRpc();
=======
    private void ShootServerRpc(Vector3 position, Quaternion rotation)
    {
        ShootClientRpc(position, rotation);
>>>>>>> Weapons-Guillaume
    }


    [ClientRpc]
<<<<<<< HEAD
    private void ShootClientRpc()
    {
        if (IsOwner == false) LocalShoot();
=======
    private void ShootClientRpc(Vector3 position, Quaternion rotation)
    {
        if (IsOwner == false) LocalShoot(position, rotation);
>>>>>>> Weapons-Guillaume
    }

    /// <summary>
    /// Shoot and instanciate the projectile in term of the actual weapon 
    /// </summary>
<<<<<<< HEAD
    public void LocalShoot()
=======
    public void LocalShoot(Vector3 position, Quaternion rotation)
>>>>>>> Weapons-Guillaume
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
<<<<<<< HEAD
                    go.transform.position = gun.transform.position;
                    go.transform.rotation = Quaternion.identity;
=======
                    go.transform.position = position;
                    go.transform.rotation = rotation;
>>>>>>> Weapons-Guillaume
                    go.GetComponent<ProjectileScript>().Init(hit.point, actualWeapon.dispertion);
                }
            }
            else
            {
                GameObject go = ObjectPoolingManager.instance.GetObject();
<<<<<<< HEAD
                go.transform.position = gun.transform.position;
                go.transform.rotation = Quaternion.identity;
=======
                go.transform.position = position;
                go.transform.rotation = rotation;
>>>>>>> Weapons-Guillaume
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
<<<<<<< HEAD
                    go.transform.position = gun.transform.position;
                    go.transform.rotation = gun.transform.rotation;
=======
                    go.transform.position = position;
                    go.transform.rotation = rotation;
>>>>>>> Weapons-Guillaume
                    go.GetComponent<ProjectileScript>().Init(actualWeapon.dispertion);
                }
            }
            else
            {
                GameObject go = ObjectPoolingManager.instance.GetObject();
<<<<<<< HEAD
                go.transform.position = gun.transform.position;
                go.transform.rotation = gun.transform.rotation;
=======
                go.transform.position = position;
                go.transform.rotation = rotation;
>>>>>>> Weapons-Guillaume
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
