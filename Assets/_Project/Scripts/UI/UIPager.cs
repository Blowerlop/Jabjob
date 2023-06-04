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
        public int pageIndex;
        public UnityEvent onPageSelectedEvent = new UnityEvent();

        public Page(string gameObjectName, CanvasGroup canvasGroup, int pageIndex)
        {
            this.gameObjectName = gameObjectName;
            this.canvasGroup = canvasGroup;
            this.pageIndex = pageIndex;
        }
    }
    
    
    public class UIPager : MonoBehaviour
    {
        private RectTransform _rectTransform;
        
        public List<Page> _pages = new List<Page>();
        [SerializeField] [ReadOnlyField] private int _currentPageIndex;
        [SerializeField] [ReadOnlyField] private int _previousPageIndex;
        [SerializeField] private bool pageLoop = true;
        [SerializeField] private bool crossFadePages;
        [SerializeField] private float _crossFadeDuration = 0.25f;
        private Coroutine _crossFadeCoroutineCurrentPage = null;
        private Coroutine _crossFadeCoroutinePreviousPage = null;
        

        public void NextPage()
        {
            if (_currentPageIndex + 1 > _pages.Count - 1)
            {
                if (pageLoop)
                {
                    _previousPageIndex = _currentPageIndex;
                    _currentPageIndex = 0;
                }
                else
                {
                    return;
                }
            }
            else
            {
                _previousPageIndex = _currentPageIndex;
                _currentPageIndex++;
            }
            
            if (crossFadePages)
            {
                CancelCrossPages();
            }
            
            GetPage(_currentPageIndex).onPageSelectedEvent.Invoke();
            ChangePageVisualBehaviour();
        }

        public void PreviousPage()
        {
            if (_currentPageIndex - 1 < 0)
            {
                if (pageLoop)
                {
                    _previousPageIndex = _currentPageIndex;
                    _currentPageIndex = _pages.Count - 1;
                }
                else
                {
                    return;
                }
            }
            else
            {
                _previousPageIndex = _currentPageIndex;
                _currentPageIndex--;
            }

            
            if (crossFadePages)
            {
                CancelCrossPages();
            }
            
            GetPage(_currentPageIndex).onPageSelectedEvent.Invoke();
            ChangePageVisualBehaviour();
            
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
                    _pages.Add(new Page(canvasGroup.name, canvasGroup, i));
                }
                else
                {
                    CanvasGroup childrenCanvasGroup = children[i].gameObject.AddComponent<CanvasGroup>();
                    _pages.Add(new Page(childrenCanvasGroup.name, childrenCanvasGroup, i));

                }
            }
        }

        public void GoToPage(CanvasGroup canvasGroup)
        {
            if (GetPage(_currentPageIndex).canvasGroup == canvasGroup) return;
            
            for (int i = 0; i < _pages.Count; i++)
            {
                if (GetPage(i).canvasGroup == canvasGroup)
                {
                    _previousPageIndex = _currentPageIndex;
                    _currentPageIndex = i;
                    
                    GetPage(_currentPageIndex).onPageSelectedEvent.Invoke();
                    ChangePageVisualBehaviour();
                    break;
                }
            }
        }

        private void CrossFadePages()
        {
            CanvasGroup previousPageCanvasGroup = GetPage(_previousPageIndex).canvasGroup;
            CanvasGroup currentCanvasGroup = GetPage(_currentPageIndex).canvasGroup;

            currentCanvasGroup.gameObject.SetActive(true);
            
            _crossFadeCoroutinePreviousPage =
                StartCoroutine(UtilitiesClass.LerpInTimeCoroutine(_crossFadeDuration, 1.0f, 0.0f, lerpCurrentValue =>
                {
                    previousPageCanvasGroup.alpha = lerpCurrentValue;
                }
                    , () => previousPageCanvasGroup.gameObject.SetActive(false)
                    ));
            
            _crossFadeCoroutineCurrentPage =
                StartCoroutine(UtilitiesClass.LerpInTimeCoroutine(_crossFadeDuration, 0.0f, 1.0f, lerpCurrentValue =>
                {
                    currentCanvasGroup.alpha = lerpCurrentValue;
                }
                    , () => currentCanvasGroup.gameObject.SetActive(true)
                    ));
        }

        private Page GetPage(int pageIndex) => _pages[pageIndex];

        private void CancelCrossPages()
        {
            if (_crossFadeCoroutinePreviousPage != null)
            {
                CanvasGroup previousPageCanvasGroup = GetPage(_previousPageIndex).canvasGroup;
                
                
                if (previousPageCanvasGroup.gameObject.activeInHierarchy)
                {
                    StopCoroutine(_crossFadeCoroutinePreviousPage);
                    previousPageCanvasGroup.alpha = 0.0f;
                    previousPageCanvasGroup.gameObject.SetActive(false);
                }
                
                _crossFadeCoroutinePreviousPage = null;
            }
            
            if (_crossFadeCoroutineCurrentPage != null)
            {
                CanvasGroup currentCanvasGroup = GetPage(_currentPageIndex).canvasGroup;
                
                if (currentCanvasGroup.gameObject.activeInHierarchy)
                {
                    StopCoroutine(_crossFadeCoroutineCurrentPage);
                    currentCanvasGroup.alpha = 0.0f;
                    currentCanvasGroup.gameObject.SetActive(false);
                }
                
                _crossFadeCoroutineCurrentPage = null;
            }
        }

        private void ChangePageVisualBehaviour()
        {
            if (crossFadePages)
            {
#if UNITY_EDITOR
                if (Application.isPlaying == false)
                {
                    GetPage(_previousPageIndex).canvasGroup.alpha = 0.0f;
                    GetPage(_previousPageIndex).canvasGroup.gameObject.SetActive(false);
                    GetPage(_currentPageIndex).canvasGroup.alpha = 1.0f;
                    GetPage(_currentPageIndex).canvasGroup.gameObject.SetActive(true);
                }
                else
                {
                    CrossFadePages();
                }
#else
                CrossFadePages();
#endif
            }
            else
            {
                _pages[_previousPageIndex].canvasGroup.gameObject.SetActive(false);
                _pages[_currentPageIndex].canvasGroup.gameObject.SetActive(true);
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
            
            if (GUILayout.Button("Previous Page"))
            {
                t.PreviousPage();
            }
            else if (GUILayout.Button("Next Page"))
            {
                t.NextPage();
            }
            else if (GUILayout.Button("Refresh Pager"))
            {
                t.RefreshPager();
            }
        }
    }
    #endif
}
