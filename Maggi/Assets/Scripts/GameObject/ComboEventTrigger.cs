using System;
using UnityEngine;
using UnityEngine.Events;

public class ComboEventTrigger : MonoBehaviour
{
    // 콤보 성공 시 레버 잠금 해제
    [SerializeField] private UnityEvent _onComboEvent;
    [SerializeField] private int _comboCount = 3; // 성공해야 하는 콤보 수
    [SerializeField] private float _comboTime = 2.0f; // 다음 콤보까지의 제한 시간
    [SerializeField] private float _lastComboTime;
    [SerializeField] private int _currentComboCount;
    [SerializeField] private bool isActive;
    [SerializeField] private Renderer _cordRenderer;
    
    // Audio Player
    [SerializeField] private AudioCueSO _generatorOnAudioCue;
    [SerializeField] private AudioCueEventChannelSO _sfxEventChannel;
    [SerializeField] private AudioConfigurationSO _audioConfig;
    
    [Header("Listening to")]
    // Generator Animation clip에서 Broadcasting 시킴
    [SerializeField] private VoidEventChannelSO _onGeneratorActivated;

    private Material _cordMaterial;
    private Material _copiedMaterial;
    
    private void OnEnable()
    {
        _onGeneratorActivated.OnEventRaised += IncreaseComboCount;
    }
    private void OnDisable()
    {
        _onGeneratorActivated.OnEventRaised -= IncreaseComboCount;
    }

    private void Start()
    {
        _cordMaterial = _cordRenderer.material;
        _copiedMaterial =  new Material(_cordMaterial);;
        _cordRenderer.material = _copiedMaterial;
    }

    /// <summary>
    /// combo 시간 내에 combo count 만큼 combo를 쌓을 시 onComboEvent 실행
    /// </summary>
    private void Update()
    {
        if (!isActive) 
            return;
        
        if (_currentComboCount <= 0)
            return;

        // 제한 시간 지나면 콤보 제거
        if (Time.time - _lastComboTime > _comboTime && !isActive)
        {
            _currentComboCount--;
            _lastComboTime = Time.time;
            _copiedMaterial.SetFloat("_On_Off", 0.0f);
        }

        // 콤보 다 쌓았을 때 레버 해제
        if (_currentComboCount >= _comboCount)
        {
            _onComboEvent?.Invoke(); // Animator parameter isLocked = false 설정
            _copiedMaterial.SetFloat("_On_Off", 1.0f);
            _sfxEventChannel.RaisePlayEvent(_generatorOnAudioCue, _audioConfig, transform.position);
            isActive = false; // 더 이상 발전기 작동 안 되게 설정
        }
    }
    
    private void IncreaseComboCount()
    {
        if (!isActive)
            return;
        
        _copiedMaterial.SetFloat("_On_Off", 0.2f);
        _currentComboCount++;
        _lastComboTime = Time.time;
    }
}
