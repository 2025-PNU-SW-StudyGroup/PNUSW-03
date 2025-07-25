using System;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public enum SettingFieldType
{
    Volume_Master,
    Volume_Music,
    Volume_Sfx,
    Resolution,
    FullScreen,
    AntiAliasing,
    ShadowDistance,
    ShadowQuality,
}

public enum SettingsType
{
    Audio,
    Graphic,
}

public class UISettingController : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;
    
    [Header("Setting UI")]
    [SerializeField] private UIGenericButton _backButton;
    
    [Header("Setting")]
    [SerializeField] private UISettingsAudioComponent _audioComponent;
    [SerializeField] private UISettingsGraphicsComponent _graphicsComponent;
    [SerializeField] private SettingsSO _currentSettings;
    
    [Header("Broadcasting on")]
    [SerializeField] private VoidEventChannelSO _saveSettingEvent;

    public UnityAction Closed;

    private void OnEnable()
    {
        _backButton.Clicked += ClosedScreen;
        _audioComponent._save += SaveAudioSettings;
        _graphicsComponent._save += SaveGraphicsSettings;
        _inputReader.MenuCloseEvent += ClosedScreen;

        OpenSetting();
    }

    private void OnDisable()
    {
        _backButton.Clicked -= ClosedScreen;
        _audioComponent._save -= SaveAudioSettings;
        _graphicsComponent._save -= SaveGraphicsSettings;
        _inputReader.MenuCloseEvent -= ClosedScreen;
    }

    private void ClosedScreen()
    {
        Closed?.Invoke();
    }

    private void OpenSetting()
    {
        _audioComponent.Setup(_currentSettings.MasterVolume, _currentSettings.MusicVolume, _currentSettings.SfxVolume);
        _graphicsComponent.Setup(_currentSettings.ResolutionIndex);
    }

    private void SaveAudioSettings(float masterVolume, float musicVolume, float sfxVolume)
    {
        _currentSettings.SaveAudioSettings(
        Mathf.Clamp01(masterVolume), 
        Mathf.Clamp01(musicVolume), 
        Mathf.Clamp01(sfxVolume));
        _saveSettingEvent.RaiseEvent();
    }

    public void SaveGraphicsSettings(int newResolutionsIndex, int newAntiAliasingIndex, float newShadowDistance, bool fullscreenState)
    {
        _currentSettings.SaveGraphicsSettings(newResolutionsIndex, newAntiAliasingIndex, newShadowDistance, fullscreenState);
        _saveSettingEvent.RaiseEvent();
    }
}
