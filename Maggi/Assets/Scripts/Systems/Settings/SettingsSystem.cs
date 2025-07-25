using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SettingsSystem : MonoBehaviour
{
    [SerializeField] private SettingsSO _currentSettings;
    [SerializeField] private SaveLoadSystem _saveLoadSystem;
    [SerializeField] private UniversalRenderPipelineAsset _urpAsset;

    [Header("Listening to")]
    [SerializeField] private VoidEventChannelSO _saveSettingEvent;

    [Header("Broadcasting on")]
    [SerializeField] private FloatEventChannelSO _changeMasterVolumeEventChannel;
    [SerializeField] private FloatEventChannelSO _changeMusicVolumeEventChannel;
    [SerializeField] private FloatEventChannelSO _changeSfxVolumeEventChannel;

    private void Awake()
    {
        _saveLoadSystem.LoadSaveDataFromDisk();
        _currentSettings.LoadSavedSettings(_saveLoadSystem.saveData);
    }

    private void OnEnable()
    {
        _saveSettingEvent.OnEventRaised += SaveSettings;
    }

    private void OnDisable()
    {
        _saveSettingEvent.OnEventRaised -= SaveSettings;
    }

    private void Start()
    {
        // Execute after init volume channels in AudioManager.cs
        SetCurrentSettings();
    }

    private void SetCurrentSettings()
    {
        // 소리가 Max치로 저장되는 오류 방지
        _changeMasterVolumeEventChannel.RaiseEvent(Mathf.Clamp01(_currentSettings.MasterVolume));
        _changeMusicVolumeEventChannel.RaiseEvent(Mathf.Clamp01(_currentSettings.MusicVolume));
        _changeSfxVolumeEventChannel.RaiseEvent(Mathf.Clamp01(_currentSettings.SfxVolume));
        
        Resolution currentResolution = Screen.currentResolution;
        
        // if (_currentSettings.ResolutionIndex < Screen.resolutions.Length)
        // {
        //     currentResolution = Screen.resolutions[_currentSettings.ResolutionIndex];
        // }
        Screen.SetResolution(currentResolution.width, currentResolution.height, _currentSettings.IsFullScreen);
        // _urpAsset.shadowDistance = _currentSettings.ShadowDistance;
        // _urpAsset.msaaSampleCount = _currentSettings.AntiAliasingIndex;
    }

    private void SaveSettings()
    {
        _saveLoadSystem.SaveDataToDisk();
    }
}
