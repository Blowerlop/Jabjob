using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI; 
namespace Project
{
    public class PlayerScoreboardUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _kill;
        [SerializeField] private TextMeshProUGUI _death;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _assist;
        [SerializeField] private TextMeshProUGUI _ping;
        [SerializeField] private Image _colorImage;
        
        // private void Awake()
        // {
        //     _name = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        //     _kill = gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        //     _death = gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        //     _score = gameObject.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        //     _assist = gameObject.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        //     _health = gameObject.transform.GetChild(4).GetComponent<TextMeshProUGUI>();
        //     _ping = gameObject.transform.GetChild(5).GetComponent<TextMeshProUGUI>();   
        // }
        
        
        
        public void UpdateNameText(string newValue) => _name.text = newValue;
        public void UpdateKillText(int newValue) => _kill.text = newValue.ToString();
        public void UpdateDeathText(int newValue) => _death.text = newValue.ToString();
        public void UpdateAssistText(int newValue) => _assist.text = newValue.ToString();
        public void UpdateColorImage(Color newValue) => _colorImage.color = newValue; 
    }
}
