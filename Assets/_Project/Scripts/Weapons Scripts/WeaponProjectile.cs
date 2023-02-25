using System.Collections;
using Project;
using UnityEngine;
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
    private Vector3 _projectileMovement;

    private bool _hasInit = false;
    
    // References
    private Collider _colliderOfBulletOwner;
    
    
    #if UNITY_EDITOR
    private Vector3 _initialPosition;
    [SerializeField] private Timer _timer;
    #endif

    //Vector3 target;

    private Rigidbody _rigidbody;

    #endregion
    

    #region Updates

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        if (_hasInit)
        { 
            _rigidbody.velocity = _projectileMovement;
            // _rigidbody.velocity = transform.forward * _projectileSpeed;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (_isOwner == false || other == _colliderOfBulletOwner)
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
        Vector3 position, Quaternion rotation, Collider collider)
    {
        _isOwner = isBulletOwner;
        RandomizeRotation(projectileDispersion);
        _projectileSpeed = projectileSpeed;
        _projectileDamage = projectileDamage;
        _colliderOfBulletOwner = collider;
        transform.position = position;
        transform.rotation = rotation;
        _projectileMovement = transform.forward * _projectileSpeed;

#if UNITY_EDITOR
        _initialPosition = transform.position;
#endif

        StartCoroutine(DestroyProjectileCooldown());

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
        _timer.StartSimpleTimer(_despawnProjectileTimer);
#endif
        yield return new WaitForSeconds(_despawnProjectileTimer);
        ObjectPoolingManager.instance.ReturnGameObject(gameObject);
    }

    private void OnDrawGizmos()
    {
        // Gizmos.DrawLine(_initialPosition, transform.position);
    }
    
    #endregion
}


