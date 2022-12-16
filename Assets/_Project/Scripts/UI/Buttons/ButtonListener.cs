using UnityEngine;
using UnityEngine.UI;

public class ButtonListener : MonoBehaviour
{
    public Button button;
    
    private void Awake()
    {
        button = GetComponent<Button>();
    }
}