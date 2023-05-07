using System;
using System.Collections;
using Project;
using Project.Utilities;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using Timer = Project.Utilities.Timer;

public class WeaponProjectile : MonoBehaviour
{
    #region Variables
    
    [Header("Projectile Infos")]
    [SerializeField] private float _despawnProjectileTimer = 10.0f;
    [SerializeField] [ReadOnlyField] private int _projectileDamage;
    [SerializeField] [ReadOnlyField] private float _projectileSpeed;
    [SerializeField] [ReadOnlyField] private bool _isOwner = false;
    private Vector3 _physicsProjectileMovement;
    private Vector3 _visualProjectileMovement;
    [SerializeField] private Rigidbody _rigidbodyPhysicsProjectile;
    [SerializeField] private Rigidbody _rigidbodyVisualProjectile;
    private Vector3 _bulletDirection;
    private ulong _ownerId;

    // References
    [SerializeField] private OnTriggerEnterEventClass _onTriggerEnterEventClass;
    private Collider _colliderOfBulletOwner;
    private CollisionPainter _collisionPainter;


    public float mult = 0.1f;

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private bool _debug = false;
    private Vector3 _initialPosition;
    private Vector3 _initialPosition2;
    [SerializeField] private Timer _timer;
    #endif
    #endregion
    

    #region Updates

    private void Awake()
    {
        _collisionPainter = GetComponentInChildren<CollisionPainter>();
    }

    public void Start()
    {
        _onTriggerEnterEventClass.@event.Subscribe(OnTriggerEnter, this);
    }
    
    private void OnDisable()
    {
        _rigidbodyPhysicsProjectile.ResetVelocities();
        _rigidbodyVisualProjectile.ResetVelocities();

    }

    void FixedUpdate()
    {
        _rigidbodyPhysicsProjectile.velocity = Vector3.Lerp(_rigidbodyPhysicsProjectile.velocity, _rigidbodyPhysicsProjectile.transform.forward * _projectileSpeed, Time.fixedDeltaTime * mult);
        _rigidbodyVisualProjectile.velocity = Vector3.Lerp(_rigidbodyVisualProjectile.velocity, _rigidbodyVisualProjectile.transform.forward * _projectileSpeed, Time.fixedDeltaTime * mult);
    }
     
    private void OnTriggerEnter(Collider other)
    {
        if (other == _colliderOfBulletOwner || other.CompareTag("Border")) return;
        
        if (_isOwner == false)
        {
            ObjectPoolingManager.instance.ReturnGameObject(gameObject);
            return;
        }

        if (other.TryGetComponent(out IHealthManagement healthManagement))
        {
            Debug.Log("Hit :" + other.name);
            healthManagement.Damage(_projectileDamage, _ownerId);
        }
        
        ObjectPoolingManager.instance.ReturnGameObject(gameObject);
    }
    
    #endregion 
    
    
    #region Methods
    
    public void Init(bool isBulletOwner, float projectileDispersion, float projectileSpeed, int projectileDamage,
        Vector3 weaponHolderPosition, Collider playerCollider, Vector3 rootCameraPosition, Vector3 collisionPoint, ulong ownerId, float paintRadius, float paintStrength, float paintHardness, Color paintColor)
    {
        // Global projectile setup
        _isOwner = isBulletOwner;
        _projectileSpeed = projectileSpeed;
        _projectileDamage = projectileDamage;
        _colliderOfBulletOwner = playerCollider;
        _ownerId = ownerId;

        Vector3 direction;
        // Physics projectile setup
        _rigidbodyPhysicsProjectile.position = rootCameraPosition;
        direction = collisionPoint - _rigidbodyPhysicsProjectile.position;
        _rigidbodyPhysicsProjectile.transform.rotation = Quaternion.LookRotation(direction, _rigidbodyPhysicsProjectile.transform.up);

        // Visual projectile setup
        _rigidbodyVisualProjectile.position = weaponHolderPosition;
        direction = collisionPoint - _rigidbodyVisualProjectile.position;
        _rigidbodyVisualProjectile.transform.rotation = Quaternion.LookRotation(direction, _rigidbodyVisualProjectile.transform.up);
        
        RandomizeRotation(projectileDispersion);

        _collisionPainter.UpdateParams(paintRadius, paintStrength, paintHardness, paintColor);
        

#if UNITY_EDITOR
        _initialPosition = weaponHolderPosition;
        _initialPosition2 = rootCameraPosition;;
#endif
        
        StartCoroutine(DestroyProjectileCooldown());
    }

    /// <summary>
    /// Randomize the bullet rotation in term of the <paramref name="projectileDispersion" />
    /// </summary>
    public void RandomizeRotation(float projectileDispersion)
    {
        if (projectileDispersion == 0.0f) return;
        
        transform.Rotate(new Vector3(Random.Range(-projectileDispersion, projectileDispersion), Random.Range(-projectileDispersion, projectileDispersion), 0), Space.Self);
    }

    /// <summary>
    /// Destroy the GameObject after x seconds
    /// </summary> 
    public IEnumerator DestroyProjectileCooldown()
    {
#if  UNITY_EDITOR
        _timer.StartSimpleTimer(_despawnProjectileTimer, true);
#endif
        yield return new WaitForSeconds(_despawnProjectileTimer);
        ObjectPoolingManager.instance.ReturnGameObject(gameObject);
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_debug == false) return;
        
        Gizmos.DrawSphere(_rigidbodyPhysicsProjectile.position, 0.1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_initialPosition2, _rigidbodyPhysicsProjectile.position);
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(_initialPosition, _rigidbodyVisualProjectile.position);
    } 
    #endif

    #endregion
}


