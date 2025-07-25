using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimelineEventListener : MonoBehaviour
{
    public enum EventType
    {
        OnTriggerEnter,
        OnCollisionEnter
    };
    
    [SerializeField] private string _tag = "Player";
    [SerializeField] private EventType eventType;
    [SerializeField] private TimelineAsset _timeline;
    [SerializeField] private PlayableDirectorSO _currentPlayable;

    [SerializeField] private InputReader _inputReader;
    [SerializeField] private bool _stopedPlayer;
    private PlayableDirector _playableDirector;
    
    private void Awake()
    {
        _playableDirector = GetComponent<PlayableDirector>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (eventType != EventType.OnTriggerEnter)
            return;
        
        // 플레이어가 트리거에 들어가면 타임라인 실행
        if (other.CompareTag(_tag)) 
        {
            if (_timeline != null)
            {
                // 만약 플레이어 정지시키고 싶으면
                if (_stopedPlayer)
                    _inputReader.EnableMenuInput();
                
                // 현재 실행 중인 타임라인과 실행할 타임라인을 저장한다.
                // 동일한 오브젝트(ex. 보스)를 복수 개의 타임라인이 다루는 걸 방지하기 위해
                // 이전 실행 중인 타임라인과 현재 실행할 타임라인을 저장한다.
                if (_currentPlayable != null)
                {
                    _currentPlayable.PreDirector = _currentPlayable.Director;
                    _currentPlayable.Director = _playableDirector;
                    // Debug.Log($"이전 director : {_currentPlayable.PreDirector}, " +
                    //           $"현재 director : {_currentPlayable.Director}");
                    _playableDirector.stopped -= _ =>
                    {
                        _inputReader.EnableGameplayInput();
                        _currentPlayable.Director = null;
                    }; // 이전 이벤트 제거
                    _playableDirector.stopped += _ =>
                    {
                        _inputReader.EnableGameplayInput();
                        _currentPlayable.Director = null;
                    }; // 재등록
                }
                
                // 타임라인에셋을 실행시킨다.
                _playableDirector.Play(_timeline);
                GetComponent<Collider>().enabled = false;
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (eventType != EventType.OnCollisionEnter)
            return;

        if (other.gameObject.CompareTag(_tag))
        {
            if (_timeline != null)
            {
                _playableDirector.Play(_timeline);
                GetComponent<Collider>().enabled = false;
            }
        }
    }
}
