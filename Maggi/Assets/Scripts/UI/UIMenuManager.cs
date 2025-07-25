using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMenuManager : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private SaveLoadSystem _saveLoadSystem;
    [SerializeField] private UIMainMenu _mainMenuPanel;
    [SerializeField] private UISettingController _settingsPanel;
    [SerializeField] private UIControlController _controlPanel;
    [SerializeField] private UIPopup _popupPanel;
    
    [Header("Listening to")]
    [SerializeField] private VoidEventChannelSO _onChangeResolution;

    [Header("Broadcasting on")]
    [SerializeField] private VoidEventChannelSO _startNewGameEvent;
    [SerializeField] private VoidEventChannelSO _continueGameEvent;

    private bool _hasSaveData = false;

    private IEnumerator Start()
    {
        _inputReader.EnableMenuInput();
        yield return new WaitForSeconds(0.4f);
        SetMenuScreen();
    }

    private void SetMenuScreen()
    {
        _saveLoadSystem.LoadSaveDataFromDisk();
        _hasSaveData = _saveLoadSystem.saveData._pointStorage != null;
        _mainMenuPanel.SetMenuScreen(_hasSaveData);
        _mainMenuPanel.NewGameButtonAction += ButtonStartNewGameClicked;
        _mainMenuPanel.ContinueButtonAction += _continueGameEvent.RaiseEvent;
        _mainMenuPanel.ControlButtonAction += OpenControlScreen;
        _mainMenuPanel.SettingsButtonAction += OpenSettingsScreen;
        _mainMenuPanel.ExitButtonAction += ShowExitConfirmationPopup;
    }

    private void ButtonStartNewGameClicked()
    {
        if (!_hasSaveData)
        {
            ConfirmStartNewGame();
        }
        else
        {
            ShowStartNewGameConfirmationPopup();
        }
    }

    private void ConfirmStartNewGame()
    {
        _startNewGameEvent.RaiseEvent();
    }

    private void ShowStartNewGameConfirmationPopup()
    {
        _popupPanel.ConfirmationResponseAction += StartNewGamePopupResponse;
        _popupPanel.ClosePopupAction += HidePopup;

        _popupPanel.gameObject.SetActive(true);
        _popupPanel.SetPopup(PopupType.NewGame);
    }

    private void StartNewGamePopupResponse(bool startNewGameConfirmed)
    {
        _popupPanel.gameObject.SetActive(false);
        if (startNewGameConfirmed)
        {
            ConfirmStartNewGame();
        }
        else
        {
            HidePopup();
        }
        _mainMenuPanel.SetMenuScreen(_hasSaveData);

        _popupPanel.ConfirmationResponseAction -= StartNewGamePopupResponse;
        _popupPanel.ClosePopupAction -= HidePopup;
    }

    private void HidePopup()
    {
        _popupPanel.gameObject.SetActive(false);
        _mainMenuPanel.SetMenuScreen(_hasSaveData);

        _popupPanel.ClosePopupAction -= HidePopup;
    }

    public void OpenSettingsScreen()
    {
        _settingsPanel.gameObject.SetActive(true);
        _settingsPanel.Closed += CloseSettingsScreen;
    }

    public void CloseSettingsScreen()
    {
        _settingsPanel.Closed -= CloseSettingsScreen;
        _settingsPanel.gameObject.SetActive(false);
        _mainMenuPanel.SetMenuScreen(_hasSaveData);
    }
    
    public void OpenControlScreen()
    {
        _controlPanel.gameObject.SetActive(true);
        _controlPanel.Closed += CloseControlScreen;
    }

    public void CloseControlScreen()
    {
        _controlPanel.Closed -= CloseControlScreen;
        _controlPanel.gameObject.SetActive(false);
        _mainMenuPanel.SetMenuScreen(_hasSaveData);
    }

    public void ShowExitConfirmationPopup()
    {
        _popupPanel.ConfirmationResponseAction += HideExitConfirmationPopup;

        _popupPanel.gameObject.SetActive(true);
        _popupPanel.SetPopup(PopupType.Quit);
    }

    void HideExitConfirmationPopup(bool quitConfirmed)
    {
        _popupPanel.gameObject.SetActive(false);
        if (quitConfirmed)
        {

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        _mainMenuPanel.SetMenuScreen(_hasSaveData);

        _popupPanel.ConfirmationResponseAction -= HideExitConfirmationPopup;
    }

    private void OnDestroy()
    {
        _popupPanel.ConfirmationResponseAction -= HideExitConfirmationPopup;
        _popupPanel.ConfirmationResponseAction -= StartNewGamePopupResponse;
    }
}
