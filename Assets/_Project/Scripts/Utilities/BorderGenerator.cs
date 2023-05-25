using UnityEditor;
#if UNITY_EDITOR 
using UnityEditor.SceneManagement; 
#endif
using UnityEngine;


namespace Project
{
    [RequireComponent(typeof(BoxCollider))]
    public class BorderGenerator : MonoBehaviour
    {
        [SerializeField] private BoxCollider _boxCollider;
        private BoxCollider[] _border = new BoxCollider[6];
        [SerializeField] private string _borderTag = "Border";

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider>();
        }


        public void GenerateBorder()
        {
            tag = _borderTag;
            ClearBorder();
            _boxCollider.enabled = false;

            Vector3 colliderCenter = _boxCollider.center;
            Vector3 colliderSize = _boxCollider.size;
            BoxCollider border;

            // Up
            _border[0] = gameObject.AddComponent<BoxCollider>();
            border = _border[0];

            border.center = new Vector3(colliderCenter.x, colliderCenter.y + colliderSize.y * 0.5f, colliderCenter.z);
            border.size = new Vector3(colliderSize.x, 0.1f, colliderSize.z);



            // Down
            _border[1] = gameObject.AddComponent<BoxCollider>();
            border = _border[1];

            border.center = new Vector3(colliderCenter.x, colliderCenter.y - colliderSize.y * 0.5f, colliderCenter.z);
            border.size = new Vector3(colliderSize.x, 0.1f, colliderSize.z);



            // Left
            _border[2] = gameObject.AddComponent<BoxCollider>();
            border = _border[2];

            border.center = new Vector3(colliderCenter.x - colliderSize.x * 0.5f, colliderCenter.y, colliderCenter.z);
            border.size = new Vector3(0.1f, colliderSize.y, colliderSize.z);



            // Right
            _border[3] = gameObject.AddComponent<BoxCollider>();
            border = _border[3];

            border.center = new Vector3(colliderCenter.x + colliderSize.x * 0.5f, colliderCenter.y, colliderCenter.z);
            border.size = new Vector3(0.1f, colliderSize.y, colliderSize.z);



            // Front
            _border[4] = gameObject.AddComponent<BoxCollider>();
            border = _border[4];

            border.center = new Vector3(colliderCenter.x, colliderCenter.y, colliderCenter.z + colliderSize.z * 0.5f);
            border.size = new Vector3(colliderSize.x, colliderSize.y, 0.1f);



            // Back
            _border[5] = gameObject.AddComponent<BoxCollider>();
            border = _border[5];

            border.center = new Vector3(colliderCenter.x, colliderCenter.y, colliderCenter.z - colliderSize.z * 0.5f);
            border.size = new Vector3(colliderSize.x, colliderSize.y, 0.1f);
            

            #if UNITY_EDITOR
            SetSceneAsModified();
            #endif
        }

        public void ClearBorder()
        {
            if (_border == null || _border[0] == null)
            {
                BoxCollider[] borderTemp = GetComponents<BoxCollider>();
                if (borderTemp.Length <= 1) return;

                for (int i = 1; i < borderTemp.Length; i++)
                {
                    _border[i - 1] = borderTemp[i];
                }
            }

            for (int i = 0; i < _border.Length; i++)
            {
                if (Application.isEditor)
                {
                    DestroyImmediate(_border[i]);
                }
                else
                {
                    Destroy(_border[i]);
                }
            }

            _boxCollider.enabled = true;
            
            #if UNITY_EDITOR
            SetSceneAsModified();
            #endif
        }
        
        #if UNITY_EDITOR
        private void SetSceneAsModified()
        {
            EditorSceneManager.MarkSceneDirty(gameObject.scene);
            EditorUtility.SetDirty(this);
        }
        #endif
    }
    
    
    #if UNITY_EDITOR
        [CustomEditor(typeof(BorderGenerator))]
        public class BorderGeneratorEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                
                BorderGenerator t = target as BorderGenerator;
                if (t == null) return;
                
                if (GUILayout.Button("Generate Border"))
                {
                    t.GenerateBorder();
                }
                else if (GUILayout.Button("Clear Border"))
                {
                    t.ClearBorder();
                }
            }
        }
    #endif
}



