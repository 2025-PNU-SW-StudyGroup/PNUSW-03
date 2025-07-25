using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIPause : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;
    
    [Header("Pause UI")]
    [SerializeField] private UIGenericButton _restartButton;
    [SerializeField] private UIGenericButton _settingsButton;
    [SerializeField] private UIGenericButton _controlButton;
    [SerializeField] private UIGenericButton _backToMenuButton;
    [SerializeField] private UIGenericButton _resumeButton;

    [Header("Broadcasting on")]
    [SerializeField] private BoolEventChannelSO _onPauseOpened;

    public event UnityAction Restarted;
    public event UnityAction SettingScreenOpened;
    public event UnityAction ControlScreenOpened;    
    public event UnityAction Resumed;
    public event UnityAction BackToMainRequested;

    private void OnEnable()
    {
        _onPauseOpened.RaiseEvent(true);

        _resumeButton.SetButton(true);
        _inputReader.MenuCloseEvent += Resume;
        _restartButton.Clicked += Restart;
        _settingsButton.Clicked += OpenSettingScreen;
        _controlButton.Clicked += OpenControlScreen;
        _backToMenuButton.Clicked += BackToMainMenuConfirmation;
        _resumeButton.Clicked += Resume;
    }

    private void OnDisable()
    {
        _onPauseOpened.RaiseEvent(false);

        _inputReader.MenuCloseEvent -= Resume;
        _restartButton.Clicked -= Restart;
        _settingsButton.Clicked -= OpenSettingScreen;
        _controlButton.Clicked -= OpenControlScreen;
        _backToMenuButton.Clicked -= BackToMainMenuConfirmation;
        _resumeButton.Clicked -= Resume;
    }

    private void Restart()
    {
        Restarted?.Invoke();
    }

    private void Resume()
    {
        Resumed?.Invoke();
    }

    private void OpenSettingScreen()
    {
        SettingScreenOpened?.Invoke();
    }

    private void OpenControlScreen()
    {
        ControlScreenOpened?.Invoke();
    }

    private void BackToMainMenuConfirmation()
    {
        BackToMainRequested?.Invoke();
    }

    public void CloseScreen()
    {
        Resumed?.Invoke();
    }
}
