using System;
using System.Collections;
using System.Collections.Generic;
using Project;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Authentication;
using System.Threading.Tasks;

public class AuthenticateUI : MonoBehaviour {


    public static AuthenticateUI Instance { get; private set; }
    [SerializeField] private TMP_Text _playerNameText;
    [SerializeField] private TMP_InputField _playerNameInputField;
    [SerializeField] private GameObject LobbyList, PopupPrefab;
    [SerializeField] private List<GameObject> LoadingAnimGO;
    private string _playerName;
    private bool canTryToConnect;

    private OpenCloseUI _openCloseUI;

    private void Awake()
    {
        Instance = this;
        _openCloseUI = GetComponent<OpenCloseUI>();
    }

    private void OnEnable()
    {
        canTryToConnect = true;
        _playerNameInputField.onEndEdit.AddListener(UpdateInputName);
        _playerNameInputField.onDeselect.AddListener(UpdateInputName);
    }

    private void OnDisable()
    {
        _playerNameInputField.onEndEdit.RemoveAllListeners();
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && canTryToConnect)
        {
            Authenticate();
        }
    }

    public string GetPlayerName()
    {
        return _playerName;
    }
    public void UpdateInputName(string input)
    {
        _playerName = input;
        LobbyManager.Instance.UpdatePlayerName(_playerName);
    }

    public async void Authenticate()
    {
        canTryToConnect = false;
        _playerNameInputField.DeactivateInputField();
        _playerNameInputField.interactable = false;
        for (int i = 0; i < LoadingAnimGO.Count; i++) { LoadingAnimGO[i].SetActive(true); }
        try
        {
            Debug.Log("D�j� connect� : " + AuthenticationService.Instance);
            bool AuthentifactionSucess = await LobbyManager.Instance.AuthenticateOnlyVivox(_playerName);
            for (int i = 0; i < LoadingAnimGO.Count; i++) { LoadingAnimGO[i].SetActive(false); }
            if (AuthentifactionSucess)
            {
                _playerNameText.text += _playerName;
                LobbyList.SetActive(true);
                gameObject.SetActive(false);
                canTryToConnect = true;
                LobbyManager.Instance.RefreshLobbyList();
            }
            else
            {
                MessagePopUpUI PopupGO = Instantiate(PopupPrefab).GetComponent<MessagePopUpUI>();
                PopupGO.Closebutton.onClick.AddListener(() => {
                    canTryToConnect = true;
                    _playerNameInputField.ActivateInputField();
                    _playerNameInputField.interactable = true;
                    _playerNameInputField.Select();
                    _playerNameInputField.caretPosition = _playerNameInputField.text.Length;
                });
                PopupGO.Message.text = "Authentification failed. Please check your internet connexion and retry.";
            }
        }
        catch (Exception e)
        {
            Debug.Log("Pas encore connect�");
            bool AuthentifactionSucess = await LobbyManager.Instance.Authenticate(_playerName);
            for (int i = 0; i < LoadingAnimGO.Count; i++) { LoadingAnimGO[i].SetActive(false); }
            if (AuthentifactionSucess)
            {
                _playerNameText.text += _playerName;
                LobbyList.SetActive(true);
                gameObject.SetActive(false);
                canTryToConnect = true;
                LobbyManager.Instance.RefreshLobbyList();
            }
            else
            {
                MessagePopUpUI PopupGO = Instantiate(PopupPrefab).GetComponent<MessagePopUpUI>();
                PopupGO.Closebutton.onClick.AddListener(() => {
                    canTryToConnect = true;
                    _playerNameInputField.ActivateInputField();
                    _playerNameInputField.interactable = true;
                    _playerNameInputField.Select();
                    _playerNameInputField.caretPosition = _playerNameInputField.text.Length;
                });
                PopupGO.Message.text = "Authentification failed. Please check your internet connexion and retry.";
            }
        }
    }
}
