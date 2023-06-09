using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using TMPro; 
namespace Project
{
    public class PlayerModelsManager : MonoBehaviour
    {
        public static PlayerModelsManager instance { get; private set; }

        public PlayerModelsList[] PlayerModelList;
        private Dictionary<string, Mesh> meshDictionary = new Dictionary<string, Mesh>();
        private Dictionary<string, Material> materialDictionary = new Dictionary<string, Material>();
        private void Awake()
        {
            instance = this;
            for (int i = 0; i < PlayerModelList.Length; i++)
            {
                meshDictionary.Add(PlayerModelList[i].name, PlayerModelList[i].meshModel);
                materialDictionary.Add(PlayerModelList[i].name, PlayerModelList[i].materialModel);
            }
        }
        
        public void ChangeCharacterModelIg(SkinnedMeshRenderer playerMeshRenderer, string modelName)
        {
            if (meshDictionary.TryGetValue(modelName, out Mesh mesh))
            {
                playerMeshRenderer.sharedMesh = mesh;
                playerMeshRenderer.GetComponent<MeshCollider>().sharedMesh = mesh;
            }
            else
            {
                Debug.LogError($"There is no mesh attributed to the {modelName} model name");
            }

            if (materialDictionary.TryGetValue(modelName, out Material material))
            {
                playerMeshRenderer.material = material;
            }
            else
            {
                Debug.LogError($"There is no material attributed to the {modelName} model name");
            }

            playerMeshRenderer.GetComponent<Paintable>().Initialize();  
        }
        public void ChangeCharacterModelMenu(SkinnedMeshRenderer playerMeshRenderer, TextMeshProUGUI modelNameTMPRO, bool isNext) 
        {
            //Si isNext == on veut le prochain skin, sinon on veut le pr�c�dent
            string modelName = modelNameTMPRO.text;
            int index = isNext ? (GetCurrentIndexInList(modelName) + 1) % PlayerModelList.Length : (GetCurrentIndexInList(modelName) - 1 + PlayerModelList.Length) % PlayerModelList.Length;
            playerMeshRenderer.sharedMesh = meshDictionary[PlayerModelList[index].name];
            playerMeshRenderer.material = materialDictionary[PlayerModelList[index].name];
            modelNameTMPRO.text = PlayerModelList[index].name; 
        }
        public void ChangeRandomCharacter(SkinnedMeshRenderer playerMeshRenderer, TextMeshProUGUI modelNameTMPRO)
        {
            string modelName = modelNameTMPRO.text;
            int currentIndex = GetCurrentIndexInList(modelName);
            int newIndex = Random.Range(0, PlayerModelList.Length);
            while(newIndex == currentIndex) newIndex = Random.Range(0, PlayerModelList.Length);
            playerMeshRenderer.sharedMesh = meshDictionary[PlayerModelList[newIndex].name];
            playerMeshRenderer.material = materialDictionary[PlayerModelList[newIndex].name];
            modelNameTMPRO.text = PlayerModelList[newIndex].name;
        }
        public int GetCurrentIndexInList(string modelName)
        {
            for(int i = 0; i < PlayerModelList.Length; i++)
            {
                if (PlayerModelList[i].name == modelName) return i;
            }
            return -1;
        }  
        public void UpdateAllPlayers()
        {
            Player[] players = GameManager.instance.GetPlayers();
            for(int i = 0; i < players.Length; i++)
            {
                ChangeCharacterModelIg(players[i].playerMesh, players[i].modelName);
            }
        }
    }


#if UNITY_EDITOR

    [CustomEditor(typeof(PlayerModelsManager))]
    [CanEditMultipleObjects]
    public class PlayerModelsManagerEditor : Editor
    {

        public override void OnInspectorGUI()
        {

            PlayerModelsManager script = (PlayerModelsManager)target;
            DrawDefaultInspector();
            GUILayoutOption[] GUIDOptionsShort = { GUILayout.Width(60) };
            //if (GUILayout.Button("button"))
            //{
            //}

        }
    }
#endif
}
