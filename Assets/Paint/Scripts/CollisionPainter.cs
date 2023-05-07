using Project;
using UnityEngine;

public class CollisionPainter : MonoBehaviour
{
    public Color color;
    
    public float radius = 1;
    public float strength = 1;
    public float hardness = 1;
    
    
    private Rigidbody rb;
    public Collider[] a;
    public LayerMask layerMask;

    [Header("References")] [SerializeField]
    private OnTriggerEnterEventClass _onTriggerEnterEventClass;

    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Start()
    {
        _onTriggerEnterEventClass.@event.Subscribe(OnTriggerEnter, this);
    }

    private void OnTriggerEnter(Collider other)
    {
        Paintable p = other.GetComponent<Paintable>();
        if (p == null) return;
        
        Debug.Log("Painting");
        
        Vector3 pos = transform.position;
        pos = other.ClosestPoint(pos);
        PaintManager.instance.Paint(p, pos, radius, hardness, strength, color);
    }

    public void UpdateParams(float radius, float strength, float hardness, Color color)
    {
        this.radius = radius;
        this.strength = strength;
        this.hardness = hardness;
        this.color = color;
    }
    
    
    private void FixedUpdate()
    {
        HighSpeedCollision();
    }

    private void HighSpeedCollision()
    {
        a = Physics.OverlapSphere(rb.position, 0.25f, layerMask);
        if (a.Length != 0)
        {
            Debug.Log("OnCollision Overlap : " + a[0].name);
            OnTriggerEnter(a[0]);
        }
    }
}


