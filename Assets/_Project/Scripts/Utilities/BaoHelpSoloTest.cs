using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Cinemachine;

namespace Project
{
    public class BaoHelpSoloTest : MonoBehaviour
    {
        public SkinnedMeshRenderer playerMesh;
        public Transform weaponHandler; 
        public MeshRenderer weaponRenderer;

        public void SetSoloTest()
        {
            playerMesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            weaponRenderer = weaponHandler.GetComponentInChildren<MeshRenderer>();
            weaponRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        } 
    }
     

#if UNITY_EDITOR
    [CustomEditor(typeof(BaoHelpSoloTest)), CanEditMultipleObjects]
    public class BaoHelpSoloTestEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            BaoHelpSoloTest t = target as BaoHelpSoloTest;

            if (GUILayout.Button("Visible Solo Test"))
            {
                t.SetSoloTest();
            }
        }
    }

#endif
}
