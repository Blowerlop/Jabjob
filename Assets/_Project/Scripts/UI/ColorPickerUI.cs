using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project
{
    public class ColorPickerUI : MonoBehaviour
    {
        [SerializeField] RectTransform _paletteRectTransform;
        [SerializeField] Image _imageColor;
        [SerializeField] Texture2D _refSprite;
        [SerializeField] CanvasScaler canvasScaler;
        Color _currentColor;

        public void OnClickColorPickerButton()
        {
            SetColor(GetColor());
        }

        private void OnEnable()
        {
            _currentColor = _imageColor.color;
        }
        private Color GetColor()
        {
            Vector3 palettePos = _paletteRectTransform.position;
            float globalPosX = Input.mousePosition.x - palettePos.x ;
            float globalPosY = Input.mousePosition.y - palettePos.y ;
            
            int localPosX = (int)Mathf.Clamp((globalPosX + (_refSprite.width /   _paletteRectTransform.rect.width  )), 0, _refSprite.width - 1);
            int localPosY = (int)Mathf.Clamp((globalPosY + (_refSprite.height / _paletteRectTransform.rect.height  )) , 0, _refSprite.height - 1);
             
            return _refSprite.GetPixel(localPosX, localPosY);
        }

        private void SetColor(Color color)
        {
            _imageColor.color = color;
        }

        public void UpdateLobbyColor()
        {
            _currentColor = _imageColor.color;
            LobbyManager.Instance.UpdatePlayerColor(ColorUtility.ToHtmlStringRGB(_currentColor));
        }

        public void GetPreviousColor()
        {
            _imageColor.color = _currentColor;
        }
    }
}