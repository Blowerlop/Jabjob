using System;
using System.Collections;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Object = UnityEngine.Object;


namespace Project
{
    [RequireComponent(typeof(WeaponManager))]
    public class PlayerShoot : NetworkBehaviour, IGameEventListener
    {
        #region Variables

        [Header("Shoot state")] 
        public Color paintColor;
        [SerializeField] private float knifeHitDistance;
        [SerializeField] private int knifeDamage; 
        private float _nextShoot;
        private float _nextKnife; 
        private float _shootRate;
        [SerializeField] private float _knifeRate; 
        //private Vector3 _hitPointClient = Vector3.zero;
        private bool _canShoot = true;
        //[SerializeField] private LayerMask _layerToAim;
        bool hasKnifeEquipped = false; 
        [Header("Other References")]
        private Player _player;
        private Weapon _weapon, _fakeWeapon;
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

        [Header("Animation")]
        [SerializeField] EventAnimHelpers bodyAnimHelpers;
        [SerializeField] EventAnimHelpers ArmsAnimHelper;
        [SerializeField] private Animator _fakeWeaponAnim;
        [SerializeField] private Animator _weaponAnim;
        [SerializeField] GameObject _fakeKnife, _knife; 
        public GameObject projectile;

        [Header("Sound")]
        public AudioClip knifeSlash; 
        #endregion


        #region Updates

        private void Awake()
        {
            _player = GetComponent<Player>();
            _weaponManager = GetComponent<WeaponManager>();
            _collider = GetComponent<Collider>();
        }

        private void Start()
        {
            _weaponHolder = _weaponManager.weaponHandler.transform;
        }

        public void OnEnable()
        {
            InputManager.instance.reload.AddListener(StartReload);
            GameEvent.onPlayerWeaponChangedLocalEvent.Subscribe(UpdateCurrentWeapon, this);
            GameEvent.onPlayerWeaponChangedServerEvent.Subscribe(UpdateCurrentWeapon, this);
            LocalEquipKnife(true);
            EquipKnifeServerRpc(true);
            hasKnifeEquipped = false;
        }

        public void OnDisable()
        {
            InputManager.instance.reload.RemoveListener(StartReload);
            GameEvent.onPlayerWeaponChangedLocalEvent.Unsubscribe(UpdateCurrentWeapon);
            GameEvent.onPlayerWeaponChangedServerEvent.Unsubscribe(UpdateCurrentWeapon);
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner == false) enabled = false;
        }

        private void Update()
        {
            if (!hasKnifeEquipped)
            {
                if (InputManager.instance.isShooting && (_weaponData.automatic || _canShoot))
                {
                    if (Time.time >= _nextShoot && _weapon.ammo > 0)
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
                                var id = hit.colliderInstanceID;
                                LocalShoot(true, weaponHolderPosition, rootCameraPosition, hit.point, true);
                                ShootServerRpc(weaponHolderPosition, rootCameraPosition, hit.point, true);
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
            }
            else
            {
                if (InputManager.instance.isShooting)
                {
                    if (Time.time >= _nextKnife)
                    {
                        _nextKnife = Time.time + _knifeRate;
                        LocalAnimOnlyKnife();
                        AnimOnlyKnifeServerRpc();
                    }
                }
            }
                
            if (!InputManager.instance.isShooting && !hasKnifeEquipped)
            {
                _canShoot = true;
            }    
            
            if(Input.GetKeyDown(KeyCode.T))
            {
                LocalEquipKnife(hasKnifeEquipped);
                EquipKnifeServerRpc(hasKnifeEquipped);
                hasKnifeEquipped = !hasKnifeEquipped;
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
            Weapon currentWeapon = _weaponManager.GetFakeWeapon();
            _fakeWeapon = _weaponManager.GetFakeWeapon(); 

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
                CreateBulletTrail(currentWeapon.bulletStartPoint, _fakeWeapon.bulletStartPoint, hitPoint);
                if (paintable == null) goto AnimationAndSound;
                
                if (_weaponData.spray)
                {
                    for (int i = 0; i < _weaponData.bulletNumber; i++)
                    {
                        PaintManager.instance.Paint(paintable, hitPoint, _weaponData.paintRadius, _weaponData.paintHardness, _weaponData.paintStrength, paintColor);
                    }
                } 
                else
                {
                    PaintManager.instance.Paint(paintable, hitPoint, _weaponData.paintRadius, _weaponData.paintHardness, _weaponData.paintStrength, paintColor); 
                }
            }
            else
            {
                if (_weaponData.spray)
                {
                    for (int i = 0; i < _weaponData.bulletNumber; i++)
                    {
                        GameObject go = ObjectPoolingManager.instance.GetObject();
                        MovingTrailScript movingTrail = go.GetComponentInChildren<MovingTrailScript>();
                        movingTrail.SetTrailColor(paintColor, paintColor); 
                        go.GetComponent<WeaponProjectile>().Init(isTheShooter, _weaponData.dispersion, _weaponData.bulletSpeed, _weaponData.damage, weaponHolderPosition, _collider, rootCameraPosition, hitPoint, OwnerClientId, _weaponData.paintRadius, _weaponData.paintStrength, _weaponData.paintHardness, paintColor);
                    }
                }
                else
                {
                    GameObject go = ObjectPoolingManager.instance.GetObject();
                    MovingTrailScript movingTrail = go.GetComponentInChildren<MovingTrailScript>();
                    movingTrail.SetTrailColor(paintColor, paintColor);
                    go.GetComponent<WeaponProjectile>().Init(isTheShooter, _weaponData.dispersion, _weaponData.bulletSpeed, _weaponData.damage, weaponHolderPosition, _collider, rootCameraPosition, hitPoint, OwnerClientId, _weaponData.paintRadius, _weaponData.paintStrength, _weaponData.paintHardness, paintColor);
                }
            }

            AnimationAndSound:
            {
                bodyAnimHelpers.SetRigWeight(1f);
                _fakeWeaponAnim.SetTrigger("Fire");
                _weaponAnim.SetTrigger("Fire");

                if (isTheShooter) { _fakeWeapon.SetFiringColorPart(paintColor); _fakeWeapon.PlayFiringPart(); }
                else { currentWeapon.SetFiringColorPart(paintColor); currentWeapon.PlayFiringPart(); }
                _audioSource.PlayOneShot(_weaponData.FiringSound);
            }
        }
        [ServerRpc]
        private void KnifeServerRpc(Vector3 weaponHolderPosition, Vector3 rootCameraPosition, Vector3 hitPoint)
        {
            KnifeClientRpc(weaponHolderPosition, rootCameraPosition, hitPoint);
        }
        [ClientRpc]
        private void KnifeClientRpc(Vector3 weaponHolderPosition, Vector3 rootCameraPosition, Vector3 hitPoint)
        {
            if (IsOwner == false) LocalKnife(false, weaponHolderPosition, rootCameraPosition, hitPoint);
        }
        private void LocalKnife(bool isTheShooter, Vector3 weaponHolderPosition, Vector3 rootCameraPosition, Vector3 hitPoint)
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
            if (paintable != null)  PaintManager.instance.Paint(paintable, hitPoint, _weaponData.paintRadius, _weaponData.paintHardness, _weaponData.paintStrength, paintColor);
        }
        [ServerRpc]
        private void AnimOnlyKnifeServerRpc()
        {
            AnimOnlyKnifeClientRpc();
        }
        [ClientRpc]
        private void AnimOnlyKnifeClientRpc()
        {
            if (IsOwner == false) LocalAnimOnlyKnife();
        }
        private void LocalAnimOnlyKnife()
        {
            _fakeWeaponAnim.SetTrigger("Fire");
            _weaponAnim.SetTrigger("Fire");
            _audioSource.PlayOneShot(knifeSlash);
        }

        [ServerRpc]
        private void EquipKnifeServerRpc(bool hasKnife)
        {
            EquipKnifeClientRpc(hasKnife);
        }
        [ClientRpc]
        private void EquipKnifeClientRpc(bool hasKnife)
        {
            if (IsOwner == false) LocalEquipKnife(hasKnife);
        }
        private void LocalEquipKnife(bool hasKnife)
        {
            if (_weapon == null) _weapon = _weaponManager.GetCurrentWeapon();
            if (_fakeWeapon == null) _fakeWeapon = _weaponManager.GetFakeWeapon();
            if (!hasKnife) //Si on a pas le couteaux on l'�quipe
            {
                _weaponAnim.ResetTrigger("Fire");
                _weaponAnim.SetBool("isGunEquipped", false);
                _weaponAnim.SetTrigger("EquipKnife");
                _fakeWeaponAnim.SetTrigger("EquipKnife");
                _weapon.gameObject.SetActive(false);
                _fakeWeapon.gameObject.SetActive(false);
                _fakeKnife.SetActive(true);
                _knife.SetActive(true);
            }
            else
            {
                _weaponAnim.ResetTrigger("Fire");
                bodyAnimHelpers.SetRigWeight(1f);
                _fakeWeaponAnim.SetTrigger("EquipGun");
                _weaponAnim.SetTrigger("EquipGun");
                _weaponAnim.SetBool("isGunEquipped", true);
                _fakeKnife.SetActive(false);
                _knife.SetActive(false);
                _fakeWeapon.gameObject.SetActive(true);
                _weapon.gameObject.SetActive(true);
            }
        }
        public void PerformKnifeCalculation()
        {
            Vector3 weaponHolderPosition = _weaponHolder.position;
            Vector3 rootCameraPosition = _rootCamera.position;
            if (Physics.Raycast(_rootCamera.position, _rootCamera.forward, out RaycastHit hit,
                      knifeHitDistance, _shootLayerMaskNoPhysics))
            {
                var id = hit.colliderInstanceID;
                LocalKnife(true, weaponHolderPosition, rootCameraPosition, hit.point);
                KnifeServerRpc(weaponHolderPosition, rootCameraPosition, hit.point);
                if (hit.transform.TryGetComponent(out IHealthManagement healthManagement))
                {
                    healthManagement.Damage(knifeDamage, OwnerClientId);
                }
            }
        }
        public void StartReload()
        {
            if (hasKnifeEquipped || _weapon.ammo == _weaponData.maxAmmo || _fakeWeaponAnim.GetCurrentAnimatorStateInfo(0).IsName("Reload")) return;
            _weaponAnim.ResetTrigger("Fire");
            _weaponAnim.SetTrigger("Reload");
            _fakeWeaponAnim.SetTrigger("Reload");
            //_canShoot = false;
            //StartCoroutine(ReloadCoroutine());
        }
        public void AutoReload() //Use by Animation, end of fire POV
        {
            if (_weapon != null && _weapon.ammo <= 0) StartReload();
        }
        public void EndOfReload() //Use by Animation, mid-end of Reload POV
        {
            if (_weapon == null) return;
            _weapon.ammo = _weaponData.maxAmmo;
            GameEvent.onPlayerWeaponAmmoChangedEvent.Invoke(this, true, _weapon.ammo);
        }
        public IEnumerator ReloadCoroutine()
        {
            _canShoot = false;
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
            _fakeWeapon = _weaponManager.GetFakeWeapon();
        }
        
        private void UpdateCurrentWeapon(byte weaponID)
        {
            _weaponData = SOWeapon.GetWeaponPrefab(weaponID).weaponData;  
        }

        #region Trail effects
        [Header("VFX")]
        [SerializeField] private GameObject bulletTrail;

        public void CreateBulletTrail(Transform spawnPoint, Transform fakeSpawnPoint, Vector3 hitpoint)
        {
            GameObject clone; 
            if (IsOwner) {
                clone = Instantiate(bulletTrail, fakeSpawnPoint.position, bulletTrail.transform.rotation);
            }
            else 
            {
                clone = Instantiate(bulletTrail, spawnPoint.position, bulletTrail.transform.rotation);
            }
            MovingTrailScript movingTrail = clone.GetComponent<MovingTrailScript>();
            movingTrail.hitpoint = hitpoint;
            movingTrail.SetTrailColor(paintColor, paintColor);
            movingTrail.Initialize();
        }
        #endregion
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