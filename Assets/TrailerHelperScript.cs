using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Project
{
    public class TrailerHelperScript : MonoBehaviour
    {

        public GameObject[] gameObjects;
        public int index;
        public float timerChange;
        public float timer;

        private void Update()
        {
            timer += Time.deltaTime; 
            if(timer >= timerChange)
            {
                timer = 0;
                NextModel();
            }
        }
        public void NextModel()
        {
            gameObjects[index++].SetActive(false);
            index = index % gameObjects.Length; 
            gameObjects[index].SetActive(true);
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(TrailerHelperScript))]
    public class TrailerHelperScriptEditor : Editor
    {

        public override void OnInspectorGUI()
        {

            TrailerHelperScript script = (TrailerHelperScript)target;
            DrawDefaultInspector();
            GUILayoutOption[] GUIDOptionsShort = { GUILayout.Width(60) };
            if (GUILayout.Button("Update Blend"))
            {
                script.NextModel();
            }

        }
    }

#endif
}
