using System;
using System.Collections;
using System.Collections.Generic;
using Project.Utilities;
using UnityEditor;
using UnityEngine;

namespace Project
{
    public class Page
    {
        
    }
    
    
    public class UIPager : MonoBehaviour
    {
        public List<CanvasGroup> _pages = new List<CanvasGroup>();


        private void Start()
        {
            RefreshPager();
        }

        public void NextPage()
        {
            
        }

        public void PreviousPage()
        {
            
        }

        public void RefreshPager()
        {
            Debug.Log("Refreshing the pager...");
            List<GameObject> children = transform.GetChildren<GameObject>();
            for (int i = 0; i < children.Count; i++)
            {
                Debug.Log("a");
                if (children[i].TryGetComponent(out CanvasGroup canvasGroup))
                {
                    _pages.Add(canvasGroup);
                }
                else
                {
                    _pages.Add(children[i].AddComponent<CanvasGroup>());
                }
            }
        }
    }



    [CustomEditor(typeof(UIPager))]
    public class UIPagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            UIPager t = target as UIPager;;
            if (t == null) return;
            
            if (GUILayout.Button("Next Page"))
            {
                t.NextPage();
            }
            else if (GUILayout.Button("Previous Page"))
            {
                t.PreviousPage();
            }
            else if (GUILayout.Button("Refresh Pager"))
            {
                t.RefreshPager();
            }
        }
    }
}
