using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
namespace Project
{
    public class FeedBackIgUI : MonoBehaviour
    {
        public Color killerColor, killedColor;

        [SerializeField] Image image, imageGradient;
        [SerializeField] TextMeshProUGUI killerName, killedName;
        [SerializeField] Image assist1_Image, assist2_Image, assist3_Image;


        public void SetNames(string killer, string killed)
        {
            killerName.text = killer;
            killedName.text = killed; 
        }
        public void SetColors(Color color1, Color color2)
        {
            image.color = color1;
            Material material = imageGradient.material;
            material.SetColor("_startColor", color1);
            material.SetColor("_endColor", color2);
            killerName.color = color1 == Color.black ? Color.white : Color.black;
            killedName.color = color2 == Color.black ? Color.white : Color.black;
        }
        public void SetMaterial(Material material)
        {
            imageGradient.material = material; 
        }
        public void SetAssistColors(Color assistColor, int position)
        {
            switch(position)
            {
                case 0:
                    assist1_Image.color = assistColor;
                    assist1_Image.enabled = true;
                    break;
                case 1:
                    assist2_Image.color = assistColor;
                    assist2_Image.enabled = true;
                    break;
                case 2:
                    assist3_Image.color = assistColor;
                    assist3_Image.enabled = true;
                    break;
                default:
                    break;
            }
        }

        private void HideAssists()
        {
            assist1_Image.enabled = false;
            assist2_Image.enabled = false;
            assist3_Image.enabled = false;
        }
        public void FadeIn()
        {
            gameObject.SetActive(true);
        }
        public void FadeOut()
        {
            HideAssists();
            gameObject.SetActive(false);
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
                script.SetColors(script.killerColor, script.killedColor);
            }

        }
    }

#endif
}
