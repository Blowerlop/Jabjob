using TMPro;
using UnityEditor;
using UnityEngine;

namespace Project
{
    public class HardwareInformationUI : MonoBehaviour
    {
        [Header("Fps")]
        [SerializeField] private bool _showFps = false;
        public bool showFps
        {
            get => _showFps;
            set
            {
                _showFps = value;
                _fpsText.gameObject.SetActive(value);
            }
        }
        
        
        [SerializeField] private float _fpsIntervalSeconds = 0.5f;
        private int _frameCounter = 0;
        private float _nextFpsCalculPeriod = 0;
        private int _currentFps;
        private const string _FPSDISPLAY = "FPS : {0}";
        [SerializeField] private TMP_Text _fpsText;


        private void Start()
        {
            _nextFpsCalculPeriod = Time.realtimeSinceStartup + _fpsIntervalSeconds;
        }
 
 
        private void Update()
        {
            CalculateFps();
        }

        private void CalculateFps()
        {
            if (showFps == false) return;
            _frameCounter++;
            if (Time.realtimeSinceStartup > _nextFpsCalculPeriod) {
                _currentFps = (int)(_frameCounter / _fpsIntervalSeconds);
                _frameCounter = 0;
                _nextFpsCalculPeriod += _fpsIntervalSeconds;
                _fpsText.text = string.Format(_FPSDISPLAY, _currentFps);
            }
        }

        private void ToggleFps(bool state) => showFps = state;
    }


    #if UNITY_EDITOR
    [CustomEditor(typeof(HardwareInformationUI))]
    public class HardwareInformationEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            HardwareInformationUI t = target as HardwareInformationUI;
            if (t == null) return;
            
            if (GUILayout.Button("Toggle Fps"))
            {
                t.showFps = !t.showFps;
            }
        }
    }
    #endif
}
