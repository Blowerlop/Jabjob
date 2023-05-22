using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace Project
{
    public class FeedBackIgUI : MonoBehaviour
    {
        public Color startColor, endColor;
        [SerializeField] Image image;
        [SerializeField] Material materialBase; 

        public void SetColor(Color color1, Color color2)
        {
            Material material = Instantiate(materialBase);
            image.material = material; 
            material.SetColor("_startColor", startColor);
            material.SetColor("_endColor", endColor);
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(FeedBackIgUI))]
    [CanEditMultipleObjects]
    public class FeedBackIgUIEditor : Editor
    {

        public override void OnInspectorGUI()
        {

            FeedBackIgUI script = (FeedBackIgUI)target;
            DrawDefaultInspector();
            GUILayoutOption[] GUIDOptionsShort = { GUILayout.Width(60) };
            if (GUILayout.Button("ChangeColor Test"))
            {
                script.SetColor(script.startColor, script.endColor);
            }

        }
    }

#endif
}
