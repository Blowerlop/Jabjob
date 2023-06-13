using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Project
{
    public class VideoManager : Singleton<VideoManager>
    {
        public int currentQualityIndex => QualitySettings.GetQualityLevel();
        [SerializeField] [Min(0)] private int _lowQualityIndex = 0;
        [SerializeField] [Min(0)] private int _mediumQualityIndex = 1;
        [SerializeField] [Min(0)] private int _highQualityIndex = 2;
        [SerializeField] [Min(0)] private int _customQualityIndex = 3;
        
        public const string FRAMERATE = "FrameRate";
        public int GetFrameRate => PlayerPrefs.GetInt(FRAMERATE, Screen.currentResolution.refreshRate); 

        protected override void Awake()
        {
            keepAlive = false;
            base.Awake();
        }

        private void Start()
        {
            LoadMaximumFrameRate();
        }

        public void SetLowQuality()
        {
            Debug.Log("Setting Quality Level To Low...");
            QualitySettings.SetQualityLevel(_lowQualityIndex, true);
            Debug.Log("Quality setting is now low !");
        }

        public void SetMediumQuality()
        {
            Debug.Log("Setting Quality Level To Medium...");
            QualitySettings.SetQualityLevel(_mediumQualityIndex, true);
            Debug.Log("Quality setting is now Medium !");
        }

        public void SetHighQuality()
        {
            Debug.Log("Setting Quality Level To High...");
            QualitySettings.SetQualityLevel(_highQualityIndex, true);
            Debug.Log("Quality setting is now High ! : index : " + QualitySettings.GetQualityLevel());
        }

        public void SetCustomQuality()
        {
            Debug.Log("Setting Quality Level To Custom...");
            // Getting the RenderPipelineAsset of the current quality settings before switching quality to override this renderPipeline
            RenderPipelineAsset currentRenderPipelineAsset = QualitySettings.GetRenderPipelineAssetAt(currentQualityIndex);
            QualitySettings.SetQualityLevel(_customQualityIndex, true);
            QualitySettings.renderPipeline = currentRenderPipelineAsset;
            Debug.Log($"Quality setting is now Custom with the {currentRenderPipelineAsset.name} render pipeline");
        }

        public void SetMaximumFrameRate(int targetFrameRate)
        {
            Application.targetFrameRate = targetFrameRate;
            PlayerPrefs.SetInt(FRAMERATE, targetFrameRate);
            PlayerPrefs.Save();
            Debug.Log($"MaximumFrameRate saved : {targetFrameRate}");
        }

        private void LoadMaximumFrameRate()
        {
            Application.targetFrameRate = GetFrameRate;
            Debug.Log($"MaximumFrameRate loaded : {Application.targetFrameRate}");
        }
    }
}
