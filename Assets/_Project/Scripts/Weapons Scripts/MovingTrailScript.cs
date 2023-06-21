using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    [RequireComponent(typeof(TrailRenderer))]
    public class MovingTrailScript : MonoBehaviour
    {
        public Vector3 hitpoint;

        [SerializeField] private GameObject BulletExplosion;

        private float timer; 
        private float timeToReach;
        private bool isInitialized;
        private Vector3 startingPos;
        private Vector3 movingVector;
        private TrailRenderer trailRenderer;

        public static float EXPLOSION_SCALE = 0.7f; 
        private void Awake()
        {
            trailRenderer = GetComponent<TrailRenderer>();
        }

        private void OnEnable()
        {
            trailRenderer.emitting = true;
        }
        private void OnDisable()
        {
            trailRenderer.emitting = false;
        }
        public void Initialize()
        {
            timer = 0;
            startingPos = transform.position;
            movingVector = hitpoint - startingPos;
            timeToReach = Mathf.Clamp(Vector3.Distance(startingPos, movingVector) / 600f, 0.05f, 2.5f);  // la vitesse est arbitraire
            //timeToReach = 5f; 
            isInitialized = true;

        }

        public void SetTrailColor(Color startColor, Color endColor)
        {
            trailRenderer.startColor = startColor;
            trailRenderer.endColor = endColor;
        }

        public void SetTrailColor(Gradient colorGradient)
        {
            trailRenderer.colorGradient = colorGradient; 
        }
        void MoveTrail()
        {
            if (!isInitialized) return;
            transform.position += movingVector * (Time.fixedDeltaTime/timeToReach) ;
            timer += Time.fixedDeltaTime;
            if (timer > timeToReach)
            {
                InstantiateExplosion();
                Destroy(this.gameObject);
            }
        }
        void FixedUpdate()
        {
            MoveTrail();
        }

        void InstantiateExplosion()
        {
            GameObject bulletExplosionGO = Instantiate(BulletExplosion);
            bulletExplosionGO.transform.localScale = EXPLOSION_SCALE * Vector3.one; 
            bulletExplosionGO.transform.position = transform.position;
            var bulletExplosionPart = bulletExplosionGO.GetComponent<ParticleSystem>().main;
            var bulletLeftOversPart = bulletExplosionGO.transform.GetChild(0).GetComponent<ParticleSystem>().main;
            var randomColors = new ParticleSystem.MinMaxGradient(trailRenderer.startColor, ColorHelpersUtilities.GetVariantColor(trailRenderer.startColor)) ;
            randomColors.mode = ParticleSystemGradientMode.TwoColors;

            bulletExplosionPart.startColor = randomColors;
            bulletLeftOversPart.startColor = randomColors;
        }

    }
}
