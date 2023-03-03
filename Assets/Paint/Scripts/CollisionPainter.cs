using Project;
using UnityEngine;

public class CollisionPainter : MonoBehaviour
{
    public Color paintColor;
    
    public float radius = 1;
    public float strength = 1;
    public float hardness = 1;

    [Header("References")] 
    [SerializeField] private OnTriggerEnterEventClass _onTriggerEnterEventClass;

    
    public void Start()
    {
        _onTriggerEnterEventClass.@event.Subscribe(OnTriggerEnter, this);
    }

    private void OnTriggerEnter(Collider other) 
    { 
        Paintable p = other.GetComponent<Paintable>();
        
        if(p != null){
            Debug.Log("Painting");
            Vector3 pos = transform.position;
            PaintManager.instance.paint(p, pos, radius, hardness, strength, paintColor);
        }
    }

    
}
