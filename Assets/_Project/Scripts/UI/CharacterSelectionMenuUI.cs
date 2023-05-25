using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

namespace Project
{
    public class CharacterSelectionMenuUI : MonoBehaviour
    {
        [SerializeField] Button _previousCharacterButton, _nextCharacterButton, _randomCharacterButton; 
        [SerializeField] SkinnedMeshRenderer _skinMeshRenderer;
        [SerializeField] TextMeshProUGUI _modelNameText;
        float _refreshTimer = 1.25f;
        bool _characterChanged = false; 
        PlayerModelsManager _playerModelManager; 
        // Start is called before the first frame update
        void Start()
        {
            _previousCharacterButton.onClick.AddListener(ChangeToPreviousCharacter);
            _nextCharacterButton.onClick.AddListener(ChangeToNextCharacter);
            _randomCharacterButton.onClick.AddListener(GetRandomCharacter);
        }

        void Update()
        {
            _refreshTimer -= Time.deltaTime; 
            if(_characterChanged && _refreshTimer < 0 )
            {
                LobbyManager.Instance.UpdatePlayerCharacter(_modelNameText.text); //évite le refresh trop souvent
                _refreshTimer = 1.25f;
                _characterChanged = false; 
            }
        }
        private void ChangeToNextCharacter() {  
            PlayerModelsManager.instance.ChangeCharacterModelMenu(_skinMeshRenderer, _modelNameText, true);
            _characterChanged = true;
        }
        private void ChangeToPreviousCharacter()  {   
            PlayerModelsManager.instance.ChangeCharacterModelMenu(_skinMeshRenderer, _modelNameText, false);
            _characterChanged = true;
        }
        private void GetRandomCharacter() { 
            PlayerModelsManager.instance.ChangeRandomCharacter(_skinMeshRenderer, _modelNameText);
            _characterChanged = true;
        }
    }
}
