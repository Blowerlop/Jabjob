using System;
using System.Collections;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;


namespace Project
{
    [RequireComponent(typeof(WeaponManager))]
    public class PlayerShoot : NetworkBehaviour, IGameEventListener
    {
        #region Variables

        [Header("Shoot state")] 
        [SerializeField] private Color _paintColor;
        private float _nextShoot;
        private float _shootRate;
        //private Vector3 _hitPointClient = Vector3.zero;
        private bool _canShoot = true;
        //[SerializeField] private LayerMask _layerToAim;

        [Header("Other References")] 
        private Weapon _weapon;
        private SOWeapon _weaponData;
        private WeaponManager _weaponManager;
        private Transform _weaponHolder;
        [SerializeField] private Transform _rootCamera;
        private Collider _collider;
        [SerializeField] private LayerMask _shootLayerMaskPhysics;
        [SerializeField] private LayerMask _shootLayerMaskNoPhysics;
 
        [Header("Debug")] 
        [SerializeField] private bool _showDebug;
        [SerializeField] private AudioSource _audioSource;


        public GameObject projectile;
        #endregion


        #region Updates

        private void Awake()
        {
            _weaponManager = GetComponent<WeaponManager>();
            _collider = GetComponent<Collider>();
        }

        private void Start()
        {
            _weaponHolder = _weaponManager.weaponHandler.transform;
        }

        public void OnEnable()
        {
            InputManager.instance.reload.AddListener(Reload);
            GameEvent.onPlayerWeaponChangedLocalEvent.Subscribe(UpdateCurrentWeapon, this);
            GameEvent.onPlayerWeaponChangedServerEvent.Subscribe(UpdateCurrentWeapon, this);
        }

        public void OnDisable()
        {
            InputManager.instance.reload.RemoveListener(Reload);
            GameEvent.onPlayerWeaponChangedLocalEvent.Unsubscribe(UpdateCurrentWeapon);
            GameEvent.onPlayerWeaponChangedServerEvent.Unsubscribe(UpdateCurrentWeapon);
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
                    GameEvent.onPlayerWeaponAmmoChangedEvent.Invoke(this, false, _weapon.ammo);
                    
                    Vector3 weaponHolderPosition = _weaponHolder.position;
                    Vector3 rootCameraPosition = _rootCamera.position;
                    
                    if (_weaponData.raycast)
                    {
                        if (Physics.Raycast(_rootCamera.position, _rootCamera.forward, out RaycastHit hit,
                                Mathf.Infinity, _shootLayerMaskNoPhysics))
                        {
                            if (hit.transform.TryGetComponent(out Paintable paintable))
                            {
                                var id = hit.colliderInstanceID;
                                LocalShoot(true, weaponHolderPosition, rootCameraPosition, hit.point, true);
                                ShootServerRpc(weaponHolderPosition, rootCameraPosition, hit.point, true);
                            }
                            
                            if (hit.transform.TryGetComponent(out IHealthManagement healthManagement))
                            {
                                Debug.Log("Hit");
                                healthManagement.Damage(_weaponData.damage, OwnerClientId);
                            }
                            
                            
                            
                        }
                    }
                    else
                    {
                        Vector3 direction = Vector3.zero;
                        if (Physics.Raycast(_rootCamera.position, _rootCamera.forward, out RaycastHit hit,
                                Mathf.Infinity, _shootLayerMaskPhysics))
                        {
                            direction = hit.point;
                        }
                        
                        LocalShoot(true, weaponHolderPosition, rootCameraPosition, direction, false);
                        ShootServerRpc(weaponHolderPosition, rootCameraPosition, direction, false);
                    }
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
        private void ShootServerRpc(Vector3 weaponHolderPosition, Vector3 rootCameraPosition, Vector3 hitPoint, bool isRaycast)
        {
            ShootClientRpc(weaponHolderPosition, rootCameraPosition, hitPoint, isRaycast);
        }


        [ClientRpc]
        private void ShootClientRpc(Vector3 weaponHolderPosition, Vector3 rootCameraPosition, Vector3 hitPoint, bool isRaycast)
        {
            if (IsOwner == false) LocalShoot(false, weaponHolderPosition, rootCameraPosition, hitPoint, isRaycast);
        }
        
        private void LocalShoot(bool isTheShooter, Vector3 weaponHolderPosition, Vector3 rootCameraPosition, Vector3 hitPoint, bool isRaycast)
        {
            if (isRaycast)
            {
                Paintable paintable = null;
                var a = Physics.OverlapSphere(hitPoint, 0.25f);
                for (int i = 0; i < a.Length; i++)
                {
                    if (a[i].TryGetComponent(out paintable))
                    {
                        break;
                    }
                }

                if (paintable == null) return;
                
                if (_weaponData.spray)
                {
                    for (int i = 0; i < _weaponData.bulletNumber; i++)
                    {
                        PaintManager.instance.Paint(paintable, hitPoint, _weaponData.paintRadius, _weaponData.paintHardness, _weaponData.paintStrength, _paintColor);
                    }
                }
                else
                {
                    PaintManager.instance.Paint(paintable, hitPoint, _weaponData.paintRadius, _weaponData.paintHardness, _weaponData.paintStrength, _paintColor);
                }
            }
            else
            {
                if (_weaponData.spray)
                {
                    for (int i = 0; i < _weaponData.bulletNumber; i++)
                    {
                        GameObject go = ObjectPoolingManager.instance.GetObject();

                        go.GetComponent<WeaponProjectile>().Init(isTheShooter, _weaponData.dispersion, _weaponData.bulletSpeed, _weaponData.damage, weaponHolderPosition, _collider, rootCameraPosition, hitPoint, OwnerClientId, _weaponData.paintRadius, _weaponData.paintStrength, _weaponData.paintHardness, _paintColor);
                    }
                }
                else
                {
                    GameObject go = ObjectPoolingManager.instance.GetObject();

                    go.GetComponent<WeaponProjectile>().Init(isTheShooter, _weaponData.dispersion, _weaponData.bulletSpeed, _weaponData.damage, weaponHolderPosition, _collider, rootCameraPosition, hitPoint, OwnerClientId, _weaponData.paintRadius, _weaponData.paintStrength, _weaponData.paintHardness, _paintColor);
                }
            }
            

            _audioSource.PlayOneShot(_weaponData.FiringSound);
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
            GameEvent.onPlayerWeaponAmmoChangedEvent.Invoke(this, true, _weapon.ammo);
            _canShoot = true;
        }

        private void UpdateCurrentWeapon(Weapon weapon)
        {
            _weapon = weapon;
            _weaponData = weapon.weaponData;

            GameEvent.onPlayerWeaponAmmoChangedEvent.Invoke(this, true, _weapon.ammo);
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


static class UnityEngineObjectUtility
{
    // delegate that lets us invoke an internal method in UnityEngine.Object
    static readonly Func<int, UnityEngine.Object> findObjectFromInstanceId
        = (Func<int, UnityEngine.Object>)
        typeof(UnityEngine.Object)
            .GetMethod("FindObjectFromInstanceID", BindingFlags.NonPublic | BindingFlags.Static)
            .CreateDelegate(typeof(Func<int, UnityEngine.Object>));

    /// <summary> Get object instance based on its instance ID. See also: <see cref="UnityEngine.Object.GetInstanceID"/> </summary>
    public static TObject FindObjectFromInstanceID<TObject>(int instanceId) where TObject : UnityEngine.Object
        => findObjectFromInstanceId.Invoke(instanceId) as TObject;
}