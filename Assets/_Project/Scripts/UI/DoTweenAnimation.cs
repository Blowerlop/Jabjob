using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEditor;

namespace Project
{
    public enum EAnimationType
    {
        None,
        WorldMove,
        LocalMove,
        WorldRotate,
        LocalRotate,
        Scale,
        Color,
        Fade,
        Text
    }

    public enum EMotionBehaviour
    {
        None,
        GoToVector3,
        GoToTransform,
        AddToPosition
        
    }
    
    public class DoTweenAnimation : MonoBehaviour
    {
        private Tween _tween;
        [SerializeField] private bool _autoPlay;
        [SerializeField] private EAnimationType _animationType;
        [SerializeField] private float _duration;
        [SerializeField] private float _delay;
        [SerializeField] private Ease _ease;
        [SerializeField] [Min(-1)] private int _loop;
        [SerializeField] private LoopType _loopType;
        [SerializeField] private bool _autoKill;
        [SerializeField] private EMotionBehaviour _motionBehaviour;
        [SerializeField] private Vector3 _goToVector3;
        [SerializeField] private Transform _goToTransform;
        [SerializeField] private Vector3 _addToPosition;

        [SerializeField] private RotateMode _rotateMode;
        
        private IEnumerator Start()
        {
            yield return null;

            if (_autoPlay)
            {
                InitTween();
            }
        }


        private void InitTween()
        {
            switch (_animationType)
            {
                case EAnimationType.WorldMove:
                    _tween = transform.DOMove(GetPosition(), _duration);
                    break;
                
                case EAnimationType.LocalMove:
                    _tween = transform.DOLocalMove(GetPosition(), _duration);
                    break;
                
                case EAnimationType.WorldRotate:
                    _tween = transform.DORotate(GetPosition(), _duration, _rotateMode);
                    break;
                
                case EAnimationType.LocalRotate:
                    _tween = transform.DOLocalRotate(GetPosition(), _duration, _rotateMode);
                    break;
                
                case EAnimationType.Scale:
                    _tween = transform.DOScale(GetPosition(), _duration);
                    break;
                
                case EAnimationType.Color:
                    break;
                
                case EAnimationType.Fade:
                    break;
                
                case EAnimationType.Text:
                    break;
                
                case EAnimationType.None:
                    Debug.LogError("No animation selected");
                    return;
            }
            
            _tween.SetAutoKill(_autoKill);
            _tween.SetEase(_ease);
            _tween.SetDelay(_delay);
            _tween.SetLoops(_loop, _loopType);
        }

        public void PlayAnimation()
        {
            StopAnimation();
            InitTween();
        }

        public void StopAnimation()
        {
            if (_tween == null) return;
            _tween.Restart(false);
            _tween.Kill();
        }

        private Vector3 GetPosition()
        {
            switch (_motionBehaviour)
            {
                case EMotionBehaviour.GoToVector3:
                    return _goToVector3;

                case EMotionBehaviour.GoToTransform:
                    return _goToTransform.position;
         
                case EMotionBehaviour.AddToPosition:
                    return transform.position + _addToPosition;

                default:
                    Debug.LogError("No motion behaviour selected !");
                    return Vector3.zero;
            }
        }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(DoTweenAnimation))]
    public class DoTweenAnimationEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DoTweenAnimation t = target as DoTweenAnimation;
            if (t == null) return;
            if (Application.isPlaying == false) return;

            if (GUILayout.Button("Play"))
            {
                t.PlayAnimation();
            }
            else if (GUILayout.Button("Stop"))
            {
                t.StopAnimation();
            }
        }
    }
    #endif
}
