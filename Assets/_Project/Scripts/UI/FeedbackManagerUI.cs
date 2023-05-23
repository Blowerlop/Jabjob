using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;

namespace Project
{
    [RequireComponent(typeof(Canvas))]
    public class FeedbackManagerUI : MonoBehaviour
    {

        public FeedBackIgUI[] feedbacks;
        private int nextFeedbackNumber = 0; 
        [SerializeField] Canvas canvas;
        [SerializeField] Material materialBase;
        private void Awake()
        {
            canvas = GetComponent<Canvas>();
            if (canvas.worldCamera == null) canvas.worldCamera = Camera.main;
            canvas.planeDistance = canvas.worldCamera.nearClipPlane + 0.01f; 
            for(int i = 0; i < feedbacks.Length; i++)
            {
                Material material = Instantiate(materialBase);
                feedbacks[i].SetMaterial(material);
            }
        }


        public async void SendFeedback(ulong killerId, ulong killedId, ulong[] assistsPlayers)
        {
            FeedBackIgUI targetFeedback = feedbacks[nextFeedbackNumber % (feedbacks.Length)]; 
            Player killer = GameManager.instance.GetPlayer(killerId);
            Player killed = GameManager.instance.GetPlayer(killedId);

            targetFeedback.transform.SetAsLastSibling();
            targetFeedback.SetNames(killer.playerName, killed.playerName);
            targetFeedback.SetColors(killer.playerColor, killed.playerColor);

            int nbAssists = 0; 
            foreach (ulong assistId in assistsPlayers)
            {
                if (assistId == killer.OwnerClientId) continue;
                Player assistPlayer = GameManager.instance.GetPlayer(assistId);
                targetFeedback.SetAssistColors(assistPlayer.playerColor, nbAssists++); 
            }
            nextFeedbackNumber++;
            targetFeedback.FadeIn();

            await Task.Delay(5000);
            targetFeedback.FadeOut();
        }
    }



#if UNITY_EDITOR
    [CustomEditor(typeof(FeedbackManagerUI))]
    [CanEditMultipleObjects]
    public class FeedbackManagerUIEditor : Editor
    {

        public override void OnInspectorGUI()
        {

            FeedbackManagerUI script = (FeedbackManagerUI)target;
            DrawDefaultInspector();
            GUILayoutOption[] GUIDOptionsShort = { GUILayout.Width(60) };
            if (GUILayout.Button("Test pop feedback"))
            {

            }

        }
    }

#endif
}
