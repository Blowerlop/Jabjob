using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Project
{
    [RequireComponent(typeof(WeaponManager))]
    public class PlayerShoot : NetworkBehaviour, IGameEventListener
    {
        #region Variables

        [Header("Shoot state")]
        private float _nextShoot;
        private float _shootRate;
        private Vector3 _hitPointClient;
        private bool _canShoot = true;
        [SerializeField] private LayerMask _layerToAim;
        [SerializeField] private Transform _projectilePrefab;

        [Header("Other References")]
        private Weapon _weapon;
        private SOWeapon _weaponData;
        private WeaponManager _weaponManager;
        [SerializeField] private Camera _camera;

        [Header("Debug")] 
        [SerializeField] private bool _showDebug;


        #endregion


        #region Updates

        private void Awake()
        {
            _weaponManager = GetComponent<WeaponManager>();
        }

        private void Start()
        {
            InputManager.instance.reload.AddListener(Reload);
        }

        public void OnEnable()
        {
            GameEvent.onPlayerWeaponChange.Subscribe(UpdateCurrentWeapon, this);
        }

        public void OnDisable()
        {
            GameEvent.onPlayerWeaponChange.Unsubscribe(UpdateCurrentWeapon);
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner == false) enabled = false; 
        }

        #endregion


        #region Methods

        private void UpdateCurrentWeapon(Weapon weapon)
        {
            _weapon = weapon;
            _weaponData = weapon.weaponData;
            
            GameEvent.onPlayerWeaponAmmoChange.Invoke(this, true, _weapon.ammo);
        }

        #endregion
        
        
        



    

    
    

    void Update()
    {
        if (InputManager.instance.isShooting && (_weaponData.riffle || _canShoot))
        {
            if(Time.time >= _nextShoot && _weapon.ammo >0)
            {
                if (!_weaponData.riffle)
                {
                    _canShoot = false;
                }
                _nextShoot = Time.time + _shootRate;
                _weapon.ammo--;
                GameEvent.onPlayerWeaponAmmoChange.Invoke(this, true, _weapon.ammo);
                
                Transform weaponHandlerTransform = _weaponManager.weaponHandler.transform;
                Vector3 weaponHandlerPosition = weaponHandlerTransform.position;
                Quaternion weaponHandlerRotation = weaponHandlerTransform.rotation;
                
                LocalShoot(weaponHandlerPosition, weaponHandlerRotation);
                ShootServerRpc(weaponHandlerPosition, weaponHandlerRotation, _hitPointClient);
            }
        }
        if (!InputManager.instance.isShooting)
        {
            _canShoot = true;
        }    
    }

    [ServerRpc]
    private void ShootServerRpc(Vector3 position, Quaternion rotation, Vector3 hitPoint)
    {
        ShootClientRpc(position, rotation,hitPoint);
    }


    [ClientRpc]
    private void ShootClientRpc(Vector3 position, Quaternion rotation,Vector3 hitPoint)
    {
        if (IsOwner == false) LocalShoot(position, rotation,hitPoint);
    }

    /// <summary>
    /// Shoot and instanciate the projectile in term of the actual weapon 
    /// </summary>
    public void LocalShoot(Vector3 position, Quaternion rotation)
    {
        RaycastHit hit;
        Ray ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f));
        if (Physics.Raycast(ray, out hit, 100, _layerToAim))
        {
            _hitPointClient = hit.point;
            if (_weaponData.spray)
            {
                for (int i = 0; i < _weaponData.bulletNumber; i++)
                {
                    GameObject go = ObjectPoolingManager.instance.GetObject();
                    go.transform.position = position;
                    go.transform.rotation = rotation;
                    go.GetComponent<WeaponProjectile>().Init(hit.point, _weaponData.dispertion);
                }
            }
            else
            {
                GameObject go = ObjectPoolingManager.instance.GetObject();
                go.transform.position = position;
                go.transform.rotation = rotation;
                go.GetComponent<WeaponProjectile>().Init(hit.point, _weaponData.dispertion);
            }
            Debug.Log(GetComponent<NetworkObject>().OwnerClientId + " : shoot at " + hit.point);
        }
        else  
        {
            _hitPointClient = Vector3.zero;
            if (_weaponData.spray)
            {
                for (int i = 0; i < _weaponData.bulletNumber; i++)
                {
                    GameObject go = ObjectPoolingManager.instance.GetObject();
                    go.transform.position = position;
                    go.transform.rotation = rotation;
                    go.GetComponent<WeaponProjectile>().Init(_weaponData.dispertion);
                }
            }
            else
            {
                GameObject go = ObjectPoolingManager.instance.GetObject();
                go.transform.position = position;
                go.transform.rotation = rotation;
                go.GetComponent<WeaponProjectile>().Init(_weaponData.dispertion);
            }
        }
    }
    
    public void LocalShoot(Vector3 position, Quaternion rotation, Vector3 hitpoint)
    {
        if (hitpoint != Vector3.zero)
        {
            if (_weaponData.spray)
            {
                for (int i = 0; i < _weaponData.bulletNumber; i++)
                {
                    GameObject go = ObjectPoolingManager.instance.GetObject();
                    go.transform.position = position;
                    go.transform.rotation = rotation;
                    go.GetComponent<WeaponProjectile>().Init(hitpoint, _weaponData.dispertion);
                }
            }
            else
            {
                GameObject go = ObjectPoolingManager.instance.GetObject();
                go.transform.position = position;
                go.transform.rotation = rotation;
                go.GetComponent<WeaponProjectile>().Init(hitpoint, _weaponData.dispertion);
            }
        }
        else
        {
            if (_weaponData.spray)
            {
                for (int i = 0; i < _weaponData.bulletNumber; i++)
                {
                    GameObject go = ObjectPoolingManager.instance.GetObject();
                    go.transform.position = position;
                    go.transform.rotation = rotation;
                    go.GetComponent<WeaponProjectile>().Init(_weaponData.dispertion);
                }
            }
            else
            {
                GameObject go = ObjectPoolingManager.instance.GetObject();
                go.transform.position = position;
                go.transform.rotation = rotation;
                go.GetComponent<WeaponProjectile>().Init(_weaponData.dispertion);
            }
        }
    }

    /// <summary>
    /// Reload the weapon 
    /// </summary>
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
        GameEvent.onPlayerWeaponAmmoChange.Invoke(this, true, _weapon.ammo);
        _canShoot = true;
    }
    


    void OnDrawGizmos()
    {
        if(_showDebug)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(_weaponManager.weaponHandler.transform.position, _camera.transform.forward * 100);
        }
    }
    }
}