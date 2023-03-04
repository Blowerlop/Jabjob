using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project
{
    [RequireComponent(typeof(WeaponManager))]
    public class PlayerShoot : NetworkBehaviour, IGameEventListener
    {
        #region Variables

        [Header("Shoot state")]
        private float _nextShoot;
        private float _shootRate;
        //private Vector3 _hitPointClient = Vector3.zero;
        private bool _canShoot = true;
        //[SerializeField] private LayerMask _layerToAim;

        [Header("Other References")] 
        private Weapon _weapon;
        private SOWeapon _weaponData;
        private WeaponManager _weaponManager;
        private Transform _weaponHandler;
        [SerializeField] private Transform _rootCamera;
        private Collider _collider;
        private Transform _cameraTransform;
        [SerializeField] private LayerMask _shootLayerMask;

        [Header("Debug")] 
        [SerializeField] private bool _showDebug;
        
        #endregion


        #region Updates

        private void Awake()
        {
            _weaponManager = GetComponent<WeaponManager>();
            _collider = GetComponent<Collider>();
            if (Camera.main != null) _cameraTransform = Camera.main.transform;
            else
            {
                Debug.Log("There is no main camera !");
            }
        }

        private void Start()
        {
            _weaponHandler = _weaponManager.weaponHandler.transform;
        }

        public void OnEnable()
        {
            InputManager.instance.reload.AddListener(Reload);
            GameEvent.onPlayerWeaponChangedLocal.Subscribe(UpdateCurrentWeapon, this);
            GameEvent.onPlayerWeaponChangedServer.Subscribe(UpdateCurrentWeapon, this);
        }

        public void OnDisable()
        {
            InputManager.instance.reload.RemoveListener(Reload);
            GameEvent.onPlayerWeaponChangedLocal.Unsubscribe(UpdateCurrentWeapon);
            GameEvent.onPlayerWeaponChangedServer.Unsubscribe(UpdateCurrentWeapon);
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner == false) enabled = false;
        }

        private void Update()
        {
            if (InputManager.instance.isShooting && (_weaponData.automatic || _canShoot))
            {
                if(Time.time >= _nextShoot && _weapon.ammo >0)
                {
                    if (!_weaponData.automatic)
                    {
                        _canShoot = false;
                    }
                    _nextShoot = Time.time + _weaponData.shootRate;
                    _weapon.ammo--;
                    GameEvent.onPlayerWeaponAmmoChanged.Invoke(this, false, _weapon.ammo);

                    // LocalShoot(true, weaponHandlerPosition, weaponHandlerRotation);
                    // ShootServerRpc(weaponHandlerPosition, weaponHandlerRotation, _hitPointClient);
                    Vector3 position = _weaponHandler.position;
                    // float x = Screen.width * 0.5f;
                    // float y = Screen.height * 0.5f;
                    // Vector3 direction = Camera.main.ScreenPointToRay(new Vector3(x, y, 0)).direction;
                    Vector3 direction = Vector3.zero;
                    if (Physics.Raycast(_rootCamera.position, _rootCamera.forward, out RaycastHit hit,
                            Mathf.Infinity, _shootLayerMask))
                    {
                        direction = hit.point;
                    }
                    LocalShoot(true, position, direction);
                    ShootServerRpc(position, direction);
                }
            }
            if (!InputManager.instance.isShooting)
            {
                _canShoot = true;
            }    
            
        }

        #endregion
        

        #region Methods

        [ServerRpc]
        private void ShootServerRpc(Vector3 weaponHolderPosition, Vector3 hitPoint)
        {
            ShootClientRpc(weaponHolderPosition, hitPoint);
        }


        [ClientRpc]
        private void ShootClientRpc(Vector3 weaponHolderPosition, Vector3 hitPoint)
        {
            if (IsOwner == false) LocalShoot(false, weaponHolderPosition, hitPoint);
        }

        private void LocalShoot(bool isTheShooter, Vector3 weaponHolderPosition, Vector3 hitPoint)
        {
            Debug.Log("Shoot");
            if (_showDebug)
            {
            }
            

            if (_weaponData.spray)
            {
                for (int i = 0; i < _weaponData.bulletNumber; i++)
                {
                    GameObject go = ObjectPoolingManager.instance.GetObject();
                    go.GetComponent<WeaponProjectile>().Init(isTheShooter, _weaponData.dispersion, _weaponData.bulletSpeed, _weaponData.damage, weaponHolderPosition, _collider, _rootCamera, hitPoint);
                }
            }
            else
            {
                GameObject go = ObjectPoolingManager.instance.GetObject();
                go.GetComponent<WeaponProjectile>().Init(isTheShooter, _weaponData.dispersion, _weaponData.bulletSpeed, _weaponData.damage, weaponHolderPosition, _collider, _rootCamera, hitPoint);
            }
        }
        
        public void Reload()
        {
            if (_weapon.ammo == _weaponData.maxAmmo) return;

            StartCoroutine(ReloadCoroutine());
        }

        public IEnumerator ReloadCoroutine()
        {
            _canShoot = false;
            // Start animation
            yield return new WaitForSeconds(_weaponData.reloadDuration);
            _weapon.ammo = _weaponData.maxAmmo;
            GameEvent.onPlayerWeaponAmmoChanged.Invoke(this, true, _weapon.ammo);
            _canShoot = true;
        }

        private void UpdateCurrentWeapon(Weapon weapon)
        {
            _weapon = weapon;
            _weaponData = weapon.weaponData;

            GameEvent.onPlayerWeaponAmmoChanged.Invoke(this, true, _weapon.ammo);
        }
        
        private void UpdateCurrentWeapon(byte weaponID)
        {
            _weaponData = SOWeapon.GetWeaponPrefab(weaponID).weaponData;  
        }

        void OnDrawGizmos()
        {
            if (_showDebug)
            {
                Gizmos.color = Color.red;
                var transform1 = _rootCamera.transform;
                Gizmos.DrawRay(transform1.position, transform1.forward * 100);
            }
        }

        #endregion
    }
}




// Old Shoot system --> Guillaume system
        
// [ServerRpc]
// private void ShootServerRpc(Vector3 position, Quaternion rotation, Vector3 hitPoint)
// {
//     ShootClientRpc(position, rotation, hitPoint);
// }
//
//
// [ClientRpc]
// private void ShootClientRpc(Vector3 position, Quaternion rotation, Vector3 hitPoint)
// {
//     if (IsOwner == false)
//     {
//         //LocalShoot(position, rotation, hitPoint, false); 
//         // LocalShoot(position, rotation, false);
//         LocalShoot(false, position, rotation);
//     }
// }



        /// <summary>
        /// Shoot and instanciate the projectile in term of the actual weapon 
        /// </summary>
        // public void LocalShoot(Vector3 position, Quaternion rotation, bool isBulletOwner)
        // {
        //     RaycastHit hit;
        //     Ray ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f));
        //     if (Physics.Raycast(ray, out hit, 100, _layerToAim))
        //     {
        //         _hitPointClient = hit.point;
        //         if (_weaponData.spray)
        //         {
        //             for (int i = 0; i < _weaponData.bulletNumber; i++)
        //             {
        //                 GameObject go = ObjectPoolingManager.instance.GetObject();
        //                 go.transform.position = position;
        //                 go.transform.rotation = rotation;
        //                 go.GetComponent<WeaponProjectile>().Init(hit.point, _weaponData.dispersion, isBulletOwner);
        //             }
        //         }
        //         else
        //         {
        //             GameObject go = ObjectPoolingManager.instance.GetObject();
        //             go.transform.position = position;
        //             go.transform.rotation = rotation;
        //             go.GetComponent<WeaponProjectile>().Init(hit.point, _weaponData.dispersion, isBulletOwner);
        //         }
        //
        //         Debug.Log(GetComponent<NetworkObject>().OwnerClientId + " : shoot at " + hit.point);
        //     }
        //     else
        //     {
        //         _hitPointClient = Vector3.zero;
        //         if (_weaponData.spray)
        //         {
        //             for (int i = 0; i < _weaponData.bulletNumber; i++)
        //             {
        //                 GameObject go = ObjectPoolingManager.instance.GetObject();
        //                 go.transform.position = position;
        //                 go.transform.rotation = rotation;
        //                 go.GetComponent<WeaponProjectile>().Init(_weaponData.dispersion, isBulletOwner);
        //             }
        //         }
        //         else
        //         {
        //             GameObject go = ObjectPoolingManager.instance.GetObject();
        //             go.transform.position = position;
        //             go.transform.rotation = rotation;
        //             go.GetComponent<WeaponProjectile>().Init(_weaponData.dispersion, isBulletOwner);
        //         }
        //     }
        // }
        //
        // public void LocalShoot(Vector3 position, Quaternion rotation, Vector3 hitpoint, bool isBulletOwner)
        // {
        //     if (hitpoint != Vector3.zero)
        //     {
        //         if (_weaponData.spray)
        //         {
        //             for (int i = 0; i < _weaponData.bulletNumber; i++)
        //             {
        //                 GameObject go = ObjectPoolingManager.instance.GetObject();
        //                 go.transform.position = position;
        //                 go.transform.rotation = rotation;
        //                 go.GetComponent<WeaponProjectile>().Init(hitpoint, _weaponData.dispersion, isBulletOwner);
        //             }
        //         }
        //         else
        //         {
        //             GameObject go = ObjectPoolingManager.instance.GetObject();
        //             go.transform.position = position;
        //             go.transform.rotation = rotation;
        //             go.GetComponent<WeaponProjectile>().Init(hitpoint, _weaponData.dispersion, isBulletOwner);
        //         }
        //     }
        //     else
        //     {
        //         if (_weaponData.spray)
        //         {
        //             for (int i = 0; i < _weaponData.bulletNumber; i++)
        //             {
        //                 GameObject go = ObjectPoolingManager.instance.GetObject();
        //                 go.transform.position = position;
        //                 go.transform.rotation = rotation;
        //                 go.GetComponent<WeaponProjectile>().Init(_weaponData.dispersion, isBulletOwner);
        //             }
        //         }
        //         else
        //         {
        //             GameObject go = ObjectPoolingManager.instance.GetObject();
        //             go.transform.position = position;
        //             go.transform.rotation = rotation;
        //             go.GetComponent<WeaponProjectile>().Init(_weaponData.dispersion, isBulletOwner);
        //         }
        //     }
        // }

    

    
    

    

    

    