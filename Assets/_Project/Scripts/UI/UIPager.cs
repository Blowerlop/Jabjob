using System;
using System.Collections;
using System.Collections.Generic;
using Project.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Project
{
    [System.Serializable]
    public class Page
    {
        [ReadOnlyField] public string gameObjectName;
        [ReadOnlyField] public CanvasGroup canvasGroup;
        public UnityEvent onPageSelectedEvent = new UnityEvent();

        public Page(string gameObjectName, CanvasGroup canvasGroup)
        {
            this.gameObjectName = gameObjectName;
            this.canvasGroup = canvasGroup;
        }
    }
    
    
    public class UIPager : MonoBehaviour
    {
        private RectTransform _rectTransform;
        
        public List<Page> _pages = new List<Page>();
        private int _currentPage;
        private int _previousPage;


        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

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
            _pages.Clear();

            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }
            
            List<RectTransform> children = _rectTransform.GetComponentsInChildrenFirstDepthWithoutTheParent<RectTransform>();
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i].TryGetComponent(out CanvasGroup canvasGroup))
                {
                    _pages.Add(new Page(canvasGroup.name, canvasGroup));
                }
                else
                {
                    CanvasGroup childrenCanvasGroup = children[i].gameObject.AddComponent<CanvasGroup>();
                    _pages.Add(new Page(canvasGroup.name, childrenCanvasGroup));
                }
            }
        }
    }


#if UNITY_EDITOR
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
    #endif
}
