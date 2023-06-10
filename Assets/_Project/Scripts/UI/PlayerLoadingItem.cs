using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

namespace Project
{
    public class PlayerLoadingItem : MonoBehaviour
    {

        [SerializeField] TextMeshProUGUI playerName;
        [SerializeField] TextMeshProUGUI modelName;
        [SerializeField] Image playerColor;
        [SerializeField] Image playerModel; 
        public void SetName(string playerName)
        {
            this.playerName.text = playerName;
        }
        public void SetModel(string modelName)
        {
            this.modelName.text = modelName;
            int modelIndex = PlayerModelsManager.instance.GetCurrentIndexInList(modelName);
            this.playerModel.sprite = PlayerModelsManager.instance.PlayerModelList[modelIndex].portrait;
        }
        public void SetColor(Color color)
        {
            playerColor.color = color;
        }
        public void SetPlayerItem(string playerName, string modelName, Color color)
        {
            playerColor.color = color;
            this.playerName.text = playerName;
            this.modelName.text = modelName;
            int modelIndex = PlayerModelsManager.instance.GetCurrentIndexInList(modelName);
            this.playerModel.sprite = PlayerModelsManager.instance.PlayerModelList[modelIndex].portrait; 
        }
    }
}
