using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI; 

namespace Project
{
    public class GetHitFeedBackUI : MonoBehaviour
    {
        public Transform damagerTransform;
        public Transform playerTransform;
        [SerializeField] private RectTransform feedBackRectTransform;
        [SerializeField] Image image;
        [ReadOnlyField] public float angle;
        [ReadOnlyField] public Vector3 projectVector;
        public float visibleTime;
        float timer;
        [SerializeField]Player player;
        int playerWoundedLoop, playerHitByBulletLoop;
        private void Awake()
        {
            player.onDamageTaken += StartFadeOut; 
        }
        public void RotateUIFeedback()
        {
            if (damagerTransform == null) return; 
            Vector3 newVector = playerTransform.position - damagerTransform.position;
            projectVector = Vector3.ProjectOnPlane(newVector, playerTransform.up);
            angle = Vector3.SignedAngle(playerTransform.forward, -projectVector, playerTransform.up );
            Vector3 newRotation = new Vector3(0, 0, -angle);

            // Juste pour des tests en editor
            //string direction = angle < 0 ? "left" : "right";
            //Debug.Log("Got hit at " + direction + " -  angle " +  angle);
            
            feedBackRectTransform.localEulerAngles = newRotation; 
        }
        private void OnEnable()
        {
            if (player.health == 100) { timer = -1; this.gameObject.SetActive(false); }
        }
        private void OnDestroy()
        {
            player.onDamageTaken -= StartFadeOut;
        }
        public void Update()
        {
            RotateUIFeedback();
            timer -= Time.deltaTime; 
            if(timer < 0 )
            {
                this.gameObject.SetActive(false);
            }
        }
        public void StartFadeOut(Transform damager, Color color)
        {
            damagerTransform = damager;
            image.color = color; 
            feedBackRectTransform.gameObject.SetActive(true);
            timer = visibleTime;
            SoundManager2D.instance.PlayInGameSound("PlayerHitByBullet" + playerHitByBulletLoop);
            playerHitByBulletLoop = (playerHitByBulletLoop + 1) % 6; 
            if(player.health < 60)
            {
                if(player.isMale)  {   SoundManager2D.instance.PlayInGameSound("PlayerWoundMale" + playerWoundedLoop);    }
                else   { SoundManager2D.instance.PlayInGameSound("PlayerWound" + playerWoundedLoop); }
                playerWoundedLoop = (playerWoundedLoop + 1) % 6;
            }

        }

        private void OnDrawGizmos()
        {
            if(damagerTransform != null && playerTransform != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(playerTransform.position, damagerTransform.position);
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(playerTransform.position, playerTransform.position - projectVector);
                Gizmos.color = Color.green;
                Gizmos.DrawLine(playerTransform.position, playerTransform.position + playerTransform.forward);
            }
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(GetHitFeedBackUI))]
    public class GetHitFeedBackUIEditor : Editor
    {

        public override void OnInspectorGUI()
        {

            GetHitFeedBackUI script = (GetHitFeedBackUI)target;
            DrawDefaultInspector();
            GUILayoutOption[] GUIDOptionsShort = { GUILayout.Width(60) };
            if (GUILayout.Button("Test Get Hit Rotation"))
            {
                script.RotateUIFeedback();
            }

        }
    }

#endif
}
