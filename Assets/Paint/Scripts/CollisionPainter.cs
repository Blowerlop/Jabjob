using UnityEngine;

public class CollisionPainter : MonoBehaviour{
    public Color paintColor;
    
    public float radius = 1;
    public float strength = 1;
    public float hardness = 1;

    private void OnTriggerEnter(Collider other) {
        Paintable p = other.GetComponent<Paintable>();
        if(p != null){
            Vector3 pos = transform.position;
            PaintManager.instance.paint(p, pos, radius, hardness, strength, paintColor);
            Destroy(gameObject);    
        }
    }
}
