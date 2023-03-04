using System.Collections;
using Project;
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

    private bool _hasInit = false;
    
    // References
    [FormerlySerializedAs("_onTriggerEnterEvent")] [SerializeField] private OnTriggerEnterEventClass _onTriggerEnterEventClass;
    private Collider _colliderOfBulletOwner;



#if UNITY_EDITOR
    [Header("Debug")] 
    [SerializeField] private bool _debug = false;
    private Vector3 _initialPosition;
    private Vector3 _initialPosition2;
    [SerializeField] private Timer _timer;
    #endif

    //Vector3 target;

    

    
    #endregion
    

    #region Updates

    public void Start()
    {
        _onTriggerEnterEventClass.@event.Subscribe(OnTriggerEnter, this);
    }

    void Update()
    {
        if (_hasInit)
        {
            _rigidbodyPhysicsProjectile.velocity = _rigidbodyPhysicsProjectile.transform.forward * _projectileSpeed;
            _rigidbodyVisualProjectile.velocity = _rigidbodyVisualProjectile.transform.forward * _projectileSpeed;
        }
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
            healthManagement.Damage(_projectileDamage);
        }
        
        ObjectPoolingManager.instance.ReturnGameObject(gameObject);
    }
    
    #endregion
    
    
    #region Methods
    
    /// <summary>
    /// Initialize bullet
    /// </summary>
    // public void Init(Vector3 _target, float disp, bool isOwner)
    // {
    //     this._isOwner = isOwner;
    //     target = _target;
    //     transform.LookAt(target);
    //     RandomizeRotation(disp);
    //     //StartCoroutine(DestroyProjectileCooldown());
    // }

    public void Init(bool isBulletOwner, float projectileDispersion, float projectileSpeed, int projectileDamage,
        Vector3 weaponHolderPosition, Collider playerCollider, Transform rootCamera, Vector3 collisionPoint)
    {
        // Global projectile setup
        _isOwner = isBulletOwner;
        RandomizeRotation(projectileDispersion);
        _projectileSpeed = projectileSpeed;
        _projectileDamage = projectileDamage;
        _colliderOfBulletOwner = playerCollider;

        Vector3 direction;
        // Physics projectile setup
        _rigidbodyPhysicsProjectile.position = rootCamera.position;
        direction = collisionPoint - _rigidbodyPhysicsProjectile.position;
        _rigidbodyPhysicsProjectile.transform.rotation = Quaternion.LookRotation(direction, _rigidbodyPhysicsProjectile.transform.up);

        // Visual projectile setup
        _rigidbodyVisualProjectile.position = weaponHolderPosition;
        direction = collisionPoint - _rigidbodyVisualProjectile.position;
        _rigidbodyVisualProjectile.transform.rotation = Quaternion.LookRotation(direction, _rigidbodyVisualProjectile.transform.up);


#if UNITY_EDITOR
        _initialPosition = weaponHolderPosition;
        _initialPosition2 = rootCamera.position;;
#endif

        StartCoroutine(DestroyProjectileCooldown());

        Debug.Log("Init");
        _hasInit = true;
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

    private void OnDrawGizmos()
    {
        if (_debug == false) return;
        
        Gizmos.DrawSphere(_rigidbodyPhysicsProjectile.position, 0.1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_initialPosition2, _rigidbodyPhysicsProjectile.position);
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(_initialPosition, _rigidbodyVisualProjectile.position);
    } 

    #endregion
}


