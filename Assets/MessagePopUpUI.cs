using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project
{
    public class MessagePopUpUI : MonoBehaviour
    {

        public TextMeshProUGUI Message;
        public Button ContactSupportButton;
        public Button Closebutton;

        void Start()
        {

            Closebutton.onClick.AddListener(() => {
                Destroy(this.gameObject) ;
            });

            ContactSupportButton.onClick.AddListener(() => {
                Application.OpenURL("https://www.youtube.com/watch?v=xvFZjo5PgG0");
            });
        }
    }
}
