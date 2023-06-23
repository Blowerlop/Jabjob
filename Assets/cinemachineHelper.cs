using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEditor; 
namespace Project
{
    public class cinemachineHelper : MonoBehaviour
    {
        public CinemachineVirtualCamera cinemachine;
        public float divisor;
        CinemachineTrackedDolly dolly;
        bool slowNext;
        private void Start()
        {
            dolly = cinemachine.GetCinemachineComponent<CinemachineTrackedDolly>();
        }
        private void Update()
        {
            if(slowNext)
            {
                dolly.m_PathPosition += Time.deltaTime / divisor ;
                if(dolly.m_PathPosition >= dolly.m_Path.PathLength - 1)
                {
                    dolly.m_PathPosition = 0; 
                }
            }
        }

        public void Next()
        {
            dolly.m_PathPosition++;
        }
        public void SlowNext()
        {
            slowNext = !slowNext;
        }
    }

   
    

        #if UNITY_EDITOR
    [CustomEditor(typeof(cinemachineHelper))]
    public class cinemachineHelperEditor : Editor
    {

        public override void OnInspectorGUI()
        {

            cinemachineHelper script = (cinemachineHelper)target;
            DrawDefaultInspector();
            GUILayoutOption[] GUIDOptionsShort = { GUILayout.Width(60) };
            if (GUILayout.Button("Next"))
            {
                script.Next();
            }
            if (GUILayout.Button("slow"))
            {
                script.SlowNext();
            }

        }
    }

#endif
}
