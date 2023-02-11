// using System.Collections;
// using System.Collections.Generic;
// using Unity.Netcode;
// using UnityEngine;
//
//
// public class GunScript : NetworkBehaviour
// {
//     public SOWeapon _actualSoWeapon;
//
//     [SerializeField]
//     Transform gun, projectilePrefab, playerCamera;
//
//     public
//     int range, ammo, maxAmmo;
//
//     float nextShoot, shootRate;
//
//     [SerializeField]
//     LayerMask layerToAim;
//
//     bool canShoot = true;
//
//     public bool ShowDebug = true;
//     Vector3 hitpointClient;
//     [SerializeField] private Camera _camera;
//     [Header("Multiplayer")]
//     [SerializeField] private bool isMultiplayer = true;
//     void Start()
//     {
//         if (isMultiplayer && IsOwner == false) this.enabled = false;
//         //Init values
//         ChangeWeapon(_actualSoWeapon);
//         InputManager.instance.reload.AddListener(Reload);
//         
//         //Set Cursor
//         //Cursor.lockState = CursorLockMode.Locked;
//         //Cursor.visible = false;
//     }
//
//     void Update()
//     {
//         if (InputManager.instance.isShooting && (_actualSoWeapon.automatic || canShoot))
//         {
//             if(Time.time >= nextShoot && ammo >0)
//             {
//                 if (!_actualSoWeapon.automatic)
//                 {
//                     canShoot = false;
//                 }
//                 nextShoot = Time.time + shootRate;
//                 ammo--;
//                 LocalShoot(gun.transform.position, gun.transform.rotation);
//                 ShootServerRpc(gun.transform.position, gun.transform.rotation, hitpointClient);
//             }
//         }
//         if (!InputManager.instance.isShooting)
//         {
//             canShoot = true;
//         }    
//     }
//
//     [ServerRpc]
//     private void ShootServerRpc(Vector3 position, Quaternion rotation, Vector3 hitpoint)
//     {
//         ShootClientRpc(position, rotation,hitpoint);
//     }
//
//
//     [ClientRpc]
//     private void ShootClientRpc(Vector3 position, Quaternion rotation,Vector3 hitpoint)
//     {
//         if (IsOwner == false) LocalShoot(position, rotation,hitpoint);
//     }
//
//     /// <summary>
//     /// Shoot and instanciate the projectile in term of the actual weapon 
//     /// </summary>
//     public void LocalShoot(Vector3 position, Quaternion rotation)
//     {
//         RaycastHit hit;
//         Ray ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f));
//         if (Physics.Raycast(ray, out hit, range, layerToAim))
//         {
//             hitpointClient = hit.point;
//             if (_actualSoWeapon.spray)
//             {
//                 for (int i = 0; i < _actualSoWeapon.bulletNumber; i++)
//                 {
//                     GameObject go = ObjectPoolingManager.instance.GetObject();
//                     go.transform.position = position;
//                     go.transform.rotation = rotation;
//                     go.GetComponent<WeaponProjectile>().Init(hit.point, _actualSoWeapon.dispersion);
//                 }
//             }
//             else
//             {
//                 GameObject go = ObjectPoolingManager.instance.GetObject();
//                 go.transform.position = position;
//                 go.transform.rotation = rotation;
//                 go.GetComponent<WeaponProjectile>().Init(hit.point, _actualSoWeapon.dispersion);
//             }
//             Debug.Log(GetComponent<NetworkObject>().OwnerClientId + " : shoot at " + hit.point);
//         }
//         else  
//         {
//             hitpointClient = Vector3.zero;
//             if (_actualSoWeapon.spray)
//             {
//                 for (int i = 0; i < _actualSoWeapon.bulletNumber; i++)
//                 {
//                     GameObject go = ObjectPoolingManager.instance.GetObject();
//                     go.transform.position = position;
//                     go.transform.rotation = rotation;
//                     go.GetComponent<WeaponProjectile>().Init(_actualSoWeapon.dispersion);
//                 }
//             }
//             else
//             {
//                 GameObject go = ObjectPoolingManager.instance.GetObject();
//                 go.transform.position = position;
//                 go.transform.rotation = rotation;
//                 go.GetComponent<WeaponProjectile>().Init(_actualSoWeapon.dispersion);
//             }
//         }
//     }
//     public void LocalShoot(Vector3 position, Quaternion rotation, Vector3 hitpoint)
//     {
//         if (hitpoint != Vector3.zero)
//         {
//             if (_actualSoWeapon.spray)
//             {
//                 for (int i = 0; i < _actualSoWeapon.bulletNumber; i++)
//                 {
//                     GameObject go = ObjectPoolingManager.instance.GetObject();
//                     go.transform.position = position;
//                     go.transform.rotation = rotation;
//                     go.GetComponent<WeaponProjectile>().Init(hitpoint, _actualSoWeapon.dispersion);
//                 }
//             }
//             else
//             {
//                 GameObject go = ObjectPoolingManager.instance.GetObject();
//                 go.transform.position = position;
//                 go.transform.rotation = rotation;
//                 go.GetComponent<WeaponProjectile>().Init(hitpoint, _actualSoWeapon.dispersion);
//             }
//         }
//         else
//         {
//             if (_actualSoWeapon.spray)
//             {
//                 for (int i = 0; i < _actualSoWeapon.bulletNumber; i++)
//                 {
//                     GameObject go = ObjectPoolingManager.instance.GetObject();
//                     go.transform.position = position;
//                     go.transform.rotation = rotation;
//                     go.GetComponent<WeaponProjectile>().Init(_actualSoWeapon.dispersion);
//                 }
//             }
//             else
//             {
//                 GameObject go = ObjectPoolingManager.instance.GetObject();
//                 go.transform.position = position;
//                 go.transform.rotation = rotation;
//                 go.GetComponent<WeaponProjectile>().Init(_actualSoWeapon.dispersion);
//             }
//         }
//     }
//
//     /// <summary>
//     /// Reload the weapon 
//     /// </summary>
//     public void Reload()
//     {
//         if (ammo < maxAmmo)
//         {
//             ammo = maxAmmo;
//         }
//     }
//
//     /// <summary>
//     /// Change the actual weapon with <paramref name="newSoWeapon" />
//     /// </summary>
//     public void ChangeWeapon(SOWeapon newSoWeapon)
//     {/*
//         _actualSoWeapon = newSoWeapon;
//         gun = Instantiate(_actualSoWeapon.model, playerCamera.transform).GetComponent<Weapon>().origin;
//         maxAmmo = _actualSoWeapon.maxAmmo;
//         ammo = maxAmmo;
//         shootRate = _actualSoWeapon.shootRate;*/
//     }
//
//
//     void OnDrawGizmos()
//     {
//         if(ShowDebug)
//         {
//             Gizmos.color = Color.red;
//             Gizmos.DrawRay(gun.transform.position, _camera.transform.forward * 100);
//         }
//     }
// }
