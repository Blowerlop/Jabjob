using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro; 

namespace Project
{
    public class SinglePlayerEndStatsUI : MonoBehaviour
    {

        [SerializeField] private Camera cameraPrefab;
        [SerializeField] private RawImage cameraTextureRender; 
        [SerializeField] private Image backgroundImage ;
        [SerializeField] private TextMeshProUGUI pseudoText;
        [SerializeField] private TextMeshProUGUI endingPlaceText;

        [SerializeField] private TextMeshProUGUI killsText, assistsText, deathTexts, damageDealtText, scoreText; 
        private Camera celebratingCamera; 
        public int endingPlace { get; set; }

        public void SetPlayerSingleUI(string Pseudo, Color color, int finalPlace, VertexGradient colorGrad, int kill, int death, int assist, int score, int damages)
        {
            pseudoText.text = Pseudo;
            backgroundImage.color = color;
            endingPlace = finalPlace;
            endingPlaceText.text = finalPlace.ToString();
            endingPlaceText.colorGradient = colorGrad;
            celebratingCamera = Instantiate(cameraPrefab);
            killsText.text = kill.ToString();
            deathTexts.text = death.ToString();
            assistsText.text = assist.ToString();
            damageDealtText.text = damages.ToString("N0");
            scoreText.text = score.ToString();
        }

        public void ConfigureCamera(Vector3 position, RenderTexture cameraTexture)
        {
            celebratingCamera.transform.position = position;
            celebratingCamera.targetTexture = cameraTexture;
            cameraTextureRender.texture = cameraTexture;
        }

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SinglePlayerEndStatsUI))]
    [CanEditMultipleObjects]
    public class SinglePlayerEndStatsUIEditor : Editor
    {
        
        public override void OnInspectorGUI()
        {
            
            SinglePlayerEndStatsUI script = (SinglePlayerEndStatsUI)target;
            DrawDefaultInspector();
            GUILayoutOption[] GUIDOptionsShort = { GUILayout.Width(60) };
            //if (GUILayout.Button("Change Place"))
            //{

            //}

        }
    }

#endif
}
