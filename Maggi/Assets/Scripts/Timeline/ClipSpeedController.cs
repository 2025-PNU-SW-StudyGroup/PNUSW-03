using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class ClipSpeedController : MonoBehaviour
{
    [SerializeField] private const string TARGET_CLIP_NAME = "hallway"; // 속도 조절할 클립 이름
    [SerializeField] private float _defaultSpeed = 1.0f;
    [SerializeField] private float _minSpeed = 0.5f;
    [SerializeField] private float _maxSpeed = 1.3f;
    [SerializeField] private Vector2 _distanceThreshold = new Vector2(6.0f, 12.0f);
    [SerializeField] private Transform _bossBodyTransform;
    [SerializeField] private TransformAnchor _playerTransform;

    // 최소 속도 유지 관련 필드
    [SerializeField] private float _minSpeedHoldTime = 0.5f; // 최소 속도를 유지할 시간(초)
    private float _minSpeedTimer = 0f;
    private bool _isHoldingMinSpeed = false;

    private PlayableDirector _director;

    private void Start()
    {
        _director = GetComponent<PlayableDirector>();
    }

    private void Update()
    {
        if (!_director || _director.state != PlayState.Playing)
            return;

        // 1. 최소 속도 유지 타이머 처리
        if (_isHoldingMinSpeed)
        {
            _minSpeedTimer -= Time.deltaTime;
            if (_minSpeedTimer <= 0f)
                _isHoldingMinSpeed = false;
        }

        // 2. 거리에 따라 목표 속도 결정
        float distance = Vector3.Distance(_bossBodyTransform.position, _playerTransform.Value.position);
        float targetSpeed = _defaultSpeed;

        if (_isHoldingMinSpeed)
        {
            // 아직 최소 속도 유지 중
            targetSpeed = _minSpeed;
        }
        else
        {
            if (distance < _distanceThreshold.x)
            {
                // 최소 속도로 전환 & 타이머 시작
                targetSpeed = _minSpeed;
                _isHoldingMinSpeed = true;
                _minSpeedTimer = _minSpeedHoldTime;
            }
            else if (distance < _distanceThreshold.y)
            {
                targetSpeed = _maxSpeed;
            }
        }

        // 3. 타임라인에 속도 적용
        TimelineAsset timeline = _director.playableAsset as TimelineAsset;
        if (timeline == null)
        {
            Debug.Log("[Timeline] TimelineAsset is null");
            return;
        }

        int trackIndex = 0;
        foreach (var track in timeline.GetOutputTracks())
        {
            if (track is AnimationTrack animTrack)
            {
                int clipIndex = 0;
                foreach (TimelineClip clip in animTrack.GetClips())
                {
                    if (clip.displayName == TARGET_CLIP_NAME)
                    {
                        var rootPlayable = _director.playableGraph.GetRootPlayable(0);
                        var trackPlayable = rootPlayable.GetInput(trackIndex);
                        var clipPlayable = trackPlayable.GetInput(clipIndex);

                        clipPlayable.SetSpeed(targetSpeed);
                        rootPlayable.SetSpeed(targetSpeed);

                        //Debug.Log($"[Timeline] '{TARGET_CLIP_NAME}' speed set to {targetSpeed:F2}, distance: {distance:F2}");
                        return;
                    }
                    clipIndex++;
                }
            }
            trackIndex++;
        }
    }
}