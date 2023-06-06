using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;

namespace Project
{
    public class PlayerEndScreenUI : MonoBehaviour
    {

        public Color[] placeColor;
        public RenderTexture[] cameraTextureList; 
        public SinglePlayerEndStatsUI playerSingleTemplate;

        [SerializeField] private Image imageBackground;
        private Dictionary<int, VertexGradient> colorDictionnary = new Dictionary<int, VertexGradient>();
        private List<SinglePlayerEndStatsUI> playerList = new List<SinglePlayerEndStatsUI>();
        private Vector3 positionPlayerOffset = new Vector3(0, -1.22f, 2.06f);
        private Vector3 rotationPlayerOffset = new Vector3(0, -184, 0);

        private const float CAMERA_DISPLACE = 50f; 
        private void Awake()
        {
            for (int i = 0; i < 8; i += 2)
            {
                VertexGradient colorGradient = new VertexGradient();
                colorGradient.topLeft = placeColor[i];
                colorGradient.topRight = placeColor[i];
                colorGradient.bottomLeft = placeColor[i + 1];
                colorGradient.bottomRight = placeColor[i + 1]; 
                colorDictionnary.Add(i / 2 + 1, colorGradient);
            }
        }


        private void OnEnable()
        {
            GameEvent.onGameFinishedEvent.Subscribe(Initialize, this);
        }

        private void OnDisable()
        {
            GameEvent.onGameFinishedEvent.Unsubscribe(Initialize);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.F10)) { Initialize(); }
        }
        public void Initialize()
        {
            imageBackground.enabled = true;
            Player[] players = GameManager.instance.GetPlayers();
            Array.Sort(players, (x, y) => x.score.CompareTo(y.score));
            Array.Reverse(players);

            for (int i = 0; i < players.Length; i++)
            {
                Player player = players[i];
                SinglePlayerEndStatsUI singlePlayer = Instantiate(playerSingleTemplate, this.transform).GetComponent<SinglePlayerEndStatsUI>();
                singlePlayer.transform.localEulerAngles = Vector3.zero;
                int celebrationNumber = GetCelebrationNumber(i, players);
                int finalPlace = i > 0 && players[i].score == players[i - 1].score ? playerList[i - 1].endingPlace : i + 1; 
                singlePlayer.SetPlayerSingleUI(player.playerName, player.playerColor, finalPlace, colorDictionnary[finalPlace], player.kills, player.deaths, player.assists, player.score);
                singlePlayer.ConfigureCamera(Vector3.zero + i * new Vector3(0, CAMERA_DISPLACE,0), cameraTextureList[i]);
                singlePlayer.gameObject.SetActive(true);
                singlePlayer.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                playerList.Add(singlePlayer);
                SetPlayerVisualandAnim(player, celebrationNumber , i);
            }
        }

        private int GetCelebrationNumber(int indexPos, Player[] players)
        {
            if (indexPos == 0) return 1;
            else if(players.Length == 2)
            {
                if (indexPos == 1 && players[0].score == players[1].score) return 1;
                else return 4; 
            }
            else if(players.Length == 3)
            {
                if (players[indexPos - 1].score == players[indexPos].score) return playerList[indexPos - 1].endingPlace;
                else return indexPos + 2; 
            }
            else
            {
                if (players[indexPos - 1].score == players[indexPos].score) return playerList[indexPos - 1].endingPlace;
                else return indexPos + 1; 
            }
        }

        private void SetPlayerVisualandAnim(Player player, int finalPlace, int playerNumber)
        {
            WeaponManager playerWeapManag = player.GetComponent<WeaponManager>();
            playerWeapManag.humanMesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            playerWeapManag.GetFakeWeapon().gameObject.SetActive(false);
            playerWeapManag.GetCurrentWeapon().gameObject.SetActive(false);
            playerWeapManag.aimRig.enabled = false; 
            MonoBehaviour[] comps = player.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour c in comps)
            {
                c.enabled = false;
            }
            Animator playerAnim = playerWeapManag.aimRig.GetComponent<Animator>();
            playerAnim.SetTrigger(finalPlace.ToString());
            player.transform.position = positionPlayerOffset + playerNumber * new Vector3(0, CAMERA_DISPLACE, 0);
            player.transform.localEulerAngles = rotationPlayerOffset;
        }

    }



#if UNITY_EDITOR
    [CustomEditor(typeof(PlayerEndScreenUI))]
    [CanEditMultipleObjects]
    public class PlayerEndScreenUIEditor : Editor
    {

        public override void OnInspectorGUI()
        {

            PlayerEndScreenUI script = (PlayerEndScreenUI)target;
            DrawDefaultInspector();
            GUILayoutOption[] GUIDOptionsShort = { GUILayout.Width(60) };
            if (GUILayout.Button("Generate random Card"))
            {
                SinglePlayerEndStatsUI singlePlayer = Instantiate(script.playerSingleTemplate, script.transform).GetComponent<SinglePlayerEndStatsUI>();
                singlePlayer.gameObject.SetActive(true);
            }

        }
    }

#endif
}
