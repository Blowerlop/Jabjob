using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Project
{
    public class SkyboxHandler : MonoBehaviour
    {
        public Cubemap[] spaceCubeMaps;
        public float RotationTime;
        public float BlendValue; 


        Material skyboxMaterial;
        int currentCubemapIndex;
        bool increasingBlend; 
        private void Start()
        {
            skyboxMaterial = RenderSettings.skybox;
        }

        private void Update()
        {
            UpdateSkyBox();
        }

        public void UpdateSkyBox()
        {
            if(increasingBlend)
            {
                BlendValue += Time.deltaTime / RotationTime;
                skyboxMaterial.SetFloat("_Blend", BlendValue);
                if (BlendValue >= 0.95f) NextSkybox(increasingBlend);
            }
            else
            {
                BlendValue -= Time.deltaTime / RotationTime;
                skyboxMaterial.SetFloat("_Blend", BlendValue);
                if (BlendValue <= 0.05f) NextSkybox(increasingBlend);
            }
            
        }
        public void NextSkybox(bool isIncreasing)
        {
            currentCubemapIndex = (currentCubemapIndex + 1) % spaceCubeMaps.Length;
            skyboxMaterial = RenderSettings.skybox;
            if (isIncreasing)
            {
                skyboxMaterial.SetTexture("_Cubemap", spaceCubeMaps[currentCubemapIndex]);
            }
            else
            {
                skyboxMaterial.SetTexture("_nextCubemap", spaceCubeMaps[currentCubemapIndex]);
            }
            
            increasingBlend = !isIncreasing;
        }

        public void UpdateSkybox(float blendValue)
        {
            skyboxMaterial = RenderSettings.skybox;
            skyboxMaterial.SetFloat("_Blend", BlendValue);
        }

    }


#if UNITY_EDITOR
    [CustomEditor(typeof(SkyboxHandler))]
    public class SkyboxHandlerEditor : Editor
    {

        public override void OnInspectorGUI()
        {

            SkyboxHandler script = (SkyboxHandler)target;
            DrawDefaultInspector();
            GUILayoutOption[] GUIDOptionsShort = { GUILayout.Width(60) };
            if (GUILayout.Button("Update Blend"))
            {
                script.UpdateSkybox(script.BlendValue);
            }

        }
    }

#endif
}
