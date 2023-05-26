using System.Text;
using Project.Utilities;
using TMPro;
using UnityEngine;

namespace Project
{
    public class AutoPrefixSuffix_TMP_Text : MonoBehaviour
    {
        private TMP_Text _text;
        
        [SerializeField] private bool _usePrefix;
        [SerializeField] private string _prefix;
        
        [SerializeField] private bool _useSuffix;
        [SerializeField] private string _suffix;
        
        
        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }
        
        private void OnEnable()
        {
            _text.OnPreRenderText += AddPrefixOrSuffix;
        }
        
        private void OnDisable()
        {
            _text.OnPreRenderText -= AddPrefixOrSuffix;
        }
        
        private void AddPrefixOrSuffix(TMP_TextInfo textInfo)
        {
            _text.OnPreRenderText -= AddPrefixOrSuffix;
        
            StringBuilder stringBuilder = new StringBuilder();
            
            stringBuilder.Append(textInfo.textComponent.text);
        
            
            if (_usePrefix && string.IsNullOrEmpty(_prefix) == false)
            {
                stringBuilder.Replace(_prefix, "");
                stringBuilder.Insert(0, _prefix);
            }
            
            
            if (_useSuffix && string.IsNullOrEmpty(_suffix) == false) 
            {
                stringBuilder.Replace(_suffix, "");
                stringBuilder.Append(_suffix);
            }
            
            textInfo.textComponent.SetText(stringBuilder);
        
            // textInfo.textComponent.text = $"{_suffix}{textInfo.textComponent.text}{_prefix}";
            
            StartCoroutine(UtilitiesClass.WaitForEndOfFrameAndDoActionCoroutine(() => _text.OnPreRenderText += AddPrefixOrSuffix));
        }
    }
}
