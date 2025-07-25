using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class InteractionEventListener : MonoBehaviour
{
    [SerializeField] private TimelineAsset _timeline;
    [SerializeField] private bool _isEnable = true;
    [SerializeField] private bool _isOnlyOnce;
    [SerializeField] private PlayableDirectorSO _currentPlayable;
    
    public bool IsEnable { set => _isEnable = value; get => _isEnable; }

    [Header("If it is closed")]
    [SerializeField] private KeySO _requiredKey;
    public KeySO RequiredKey => _requiredKey;

    private PlayableDirector _playableDirector;
    
    private void Awake()
    {
        _playableDirector = GetComponent<PlayableDirector>();
        
        if (_requiredKey)
            _isEnable = false;
        else
            _isEnable = true;
    }

    // Interact Action executes this event
    public void OnInteract()
    {
        if (_isEnable)
        {
            if (_timeline != null)
            {
                // 현재 실행 중인 타임라인과 실행할 타임라인을 저장한다.
                if (_currentPlayable != null)
                {
                    _currentPlayable.PreDirector = _currentPlayable.Director;
                    _currentPlayable.Director = _playableDirector;
                    // Debug.Log($"이전 director : {_currentPlayable.PreDirector}, " +
                    //           $"현재 director : {_currentPlayable.Director}");
                    _playableDirector.stopped -= _ => _currentPlayable.Director = null; // 이전 이벤트 제거
                    _playableDirector.stopped += _ => _currentPlayable.Director = null; // 재등록
                }
                
                // 타임라인에셋을 실행시킨다.
                _playableDirector.Play(_timeline);
                
                if (_isOnlyOnce)
                {
                    this.enabled = false;
                }
            }
        }
        else
        {
            Debug.Log("Interaction is disabled");
        }
    }
}