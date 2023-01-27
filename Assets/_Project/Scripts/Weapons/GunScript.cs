using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class GunScript : NetworkBehaviour
{
    public WeaponSO actualWeapon;

    [SerializeField]
    GameObject gun, projectilePrefab, playerCamera;

    public
    int range, ammo, maxAmmo;

    float nextShoot, shootRate;

    [SerializeField]
    LayerMask layerToAim;

    bool canShoot = true;

    public bool ShowDebug = true;
    Vector3 hitpointClient;
    [SerializeField] private Camera _camera;
    [Header("Multiplayer")]
    [SerializeField] private bool isMultiplayer = true;
    void Start()
    {
        if (isMultiplayer && IsOwner == false) this.enabled = false;
        //Init values
        ChangeWeapon(actualWeapon);
        InputManager.instance.reload.AddListener(Reload);
        
        //Set Cursor
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
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
                LocalShoot(gun.transform.position, gun.transform.rotation);
                ShootServerRpc(gun.transform.position, gun.transform.rotation, hitpointClient);
            }
        }
        if (!InputManager.instance.shoot)
        {
            canShoot = true;
        }    
    }

    [ServerRpc]
    private void ShootServerRpc(Vector3 position, Quaternion rotation, Vector3 hitpoint)
    {
        ShootClientRpc(position, rotation,hitpoint);
    }


    [ClientRpc]
    private void ShootClientRpc(Vector3 position, Quaternion rotation,Vector3 hitpoint)
    {
        if (IsOwner == false) LocalShoot(position, rotation,hitpoint);
    }

    /// <summary>
    /// Shoot and instanciate the projectile in term of the actual weapon 
    /// </summary>
    public void LocalShoot(Vector3 position, Quaternion rotation)
    {
        RaycastHit hit;
        Ray ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f));
        if (Physics.Raycast(ray, out hit, range, layerToAim))
        {
            hitpointClient = hit.point;
            if (actualWeapon.spray)
            {
                for (int i = 0; i < actualWeapon.bulletNumber; i++)
                {
                    GameObject go = ObjectPoolingManager.instance.GetObject();
                    go.transform.position = position;
                    go.transform.rotation = rotation;
                    go.GetComponent<ProjectileScript>().Init(hit.point, actualWeapon.dispertion);
                }
            }
            else
            {
                GameObject go = ObjectPoolingManager.instance.GetObject();
                go.transform.position = position;
                go.transform.rotation = rotation;
                go.GetComponent<ProjectileScript>().Init(hit.point, actualWeapon.dispertion);
            }
            Debug.Log(GetComponent<NetworkObject>().OwnerClientId + " : shoot at " + hit.point);
        }
        else  
        {
            hitpointClient = Vector3.zero;
            if (actualWeapon.spray)
            {
                for (int i = 0; i < actualWeapon.bulletNumber; i++)
                {
                    GameObject go = ObjectPoolingManager.instance.GetObject();
                    go.transform.position = position;
                    go.transform.rotation = rotation;
                    go.GetComponent<ProjectileScript>().Init(actualWeapon.dispertion);
                }
            }
            else
            {
                GameObject go = ObjectPoolingManager.instance.GetObject();
                go.transform.position = position;
                go.transform.rotation = rotation;
                go.GetComponent<ProjectileScript>().Init(actualWeapon.dispertion);
            }
        }
    }
    public void LocalShoot(Vector3 position, Quaternion rotation, Vector3 hitpoint)
    {
        if (hitpoint != Vector3.zero)
        {
            if (actualWeapon.spray)
            {
                for (int i = 0; i < actualWeapon.bulletNumber; i++)
                {
                    GameObject go = ObjectPoolingManager.instance.GetObject();
                    go.transform.position = position;
                    go.transform.rotation = rotation;
                    go.GetComponent<ProjectileScript>().Init(hitpoint, actualWeapon.dispertion);
                }
            }
            else
            {
                GameObject go = ObjectPoolingManager.instance.GetObject();
                go.transform.position = position;
                go.transform.rotation = rotation;
                go.GetComponent<ProjectileScript>().Init(hitpoint, actualWeapon.dispertion);
            }
        }
        else
        {
            if (actualWeapon.spray)
            {
                for (int i = 0; i < actualWeapon.bulletNumber; i++)
                {
                    GameObject go = ObjectPoolingManager.instance.GetObject();
                    go.transform.position = position;
                    go.transform.rotation = rotation;
                    go.GetComponent<ProjectileScript>().Init(actualWeapon.dispertion);
                }
            }
            else
            {
                GameObject go = ObjectPoolingManager.instance.GetObject();
                go.transform.position = position;
                go.transform.rotation = rotation;
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


    void OnDrawGizmos()
    {
        if(ShowDebug)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(gun.transform.position, _camera.transform.forward * 100);
        }
    }
}
