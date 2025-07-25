using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

namespace Maggi.Character.Boss
{
    public enum Mode
    {
        Inactive, Idle, Walk, Detect, Trigger, Catch
    }

    public class Boss : MonoBehaviour
    {
        public List<Transform[]> PatrolAreas = new List<Transform[]>();
        public Mode CurrentMode => _currentMode;
        public Transform Target => _target;
        public ChainIKConstraint HandIK_Catch;
        public ChainIKConstraint HandIK_UnCatch;
        public Animator HandAnimator;
        public int CurrentRootIndex 
        { 
            set => _currentRootIndex = value;
            get { return _currentRootIndex; } 
        }
        
        [SerializeField] private List<Transform> _patrolAreaRoot; // patrol area에 저장된 위치로 walk 한다.
        [SerializeField] private Mode _currentMode = Mode.Idle;
        [SerializeField] private PlayableDirectorSO _currentPlayable;
        
        [Header("Listening to")]
        [SerializeField] private VoidEventChannelSO _stageTransition; // 다음 스테이지로 바꾸고 모드를 초기화 한다.
        [SerializeField] private TransformEventChannelSO _moveToTargetEvent; // target position으로 이동한다.
        
        private Transform _target;
        private int _currentRootIndex;
        private NavMeshAgent _agent;
        private const string PLAYER_TAG = "Player";
        private Vector3 _handCatch_InitPosition;
        private Vector3 _handUnCatch_InitPosition;

        private void Awake()
        {
            foreach (var item in _patrolAreaRoot)
            {
                Transform[] children = new Transform[item.childCount];

                for (int i = 0; i < item.childCount; i++)
                {
                    children[i] = item.GetChild(i);
                }

                PatrolAreas.Add(children);
            }

            _agent = GetComponent<NavMeshAgent>();
        }

        private void OnEnable()
        {
            _stageTransition.OnEventRaised += ResetModeAndUpdatePatrol;
            _moveToTargetEvent.OnEventRaised += ConfigureDetectionWithTarget;
        }

        private void OnDisable()
        {
            _stageTransition.OnEventRaised -= ResetModeAndUpdatePatrol;
            _moveToTargetEvent.OnEventRaised -= ConfigureDetectionWithTarget;
        }

        private void Start()
        {
            _handCatch_InitPosition = HandIK_Catch.data.target.localPosition;
            _handUnCatch_InitPosition = HandIK_UnCatch.data.target.localPosition;
        }

        private void Update()
        {
            if (_currentMode == Mode.Idle)
            {
                // 현재 위치 → 초기 위치 로 Lerp
                Transform tc = HandIK_Catch.data.target;
                tc.position = Vector3.Lerp(
                    tc.position, 
                    _handCatch_InitPosition, 
                    5f * Time.deltaTime
                );
                
                Transform tuc = HandIK_UnCatch.data.target;
                tuc.position = Vector3.Lerp(
                    tuc.position, 
                    _handUnCatch_InitPosition, 
                    5f * Time.deltaTime
                );
            }
        }

        private void ConfigureDetectionWithTarget(Transform target)
        {
            // Target 설정
            _target = target;
            
            // Detect 모드로 바꿔 앞서 설정한 Target을 추적함
            SetMode(Mode.Detect, "ConfigureDetectionWithTarget");
        }

        /// <summary>
        /// Idle 모드로 설정하고, 다음 이동 장소를 설정
        /// </summary>
        public void ResetModeAndUpdatePatrol()
        {
            _currentMode = Mode.Idle;
            
            // Set Next patrol area
            _currentRootIndex = (_currentRootIndex + 1) % _patrolAreaRoot.Count;
        }

        /// <summary>
        /// Boss의 현재 Mode를 설정한다. 이 모드를 통해
        /// State 전환이 일어난다.
        /// </summary>
        public void SetMode(Mode newMode, string org)
        {
            _currentMode = newMode;
            Debug.Log($"현재 모드 : {_currentMode}, 출처 : {org}");
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        /// <summary>
        /// 보스를 조작하는 Timeline 실행 시 모드를 Trigger로 설정
        /// </summary>
        public void SetTrigger()
        {
            // 이전 타임라인에서 바인딩 제거
            if (_currentPlayable.PreDirector != null)
            {
                RemoveBindingsFromTimeline(_currentPlayable.PreDirector, gameObject);
            }

            // 모드 설정
            _currentMode = Mode.Trigger;
            Debug.Log($"현재 모드 : {_currentMode}, 출처 : SetTrigger()");
        }

        /// <summary>
        /// 지정된 PlayableDirector의 타임라인에서 특정 GameObject와 관련된 모든 바인딩을 제거합니다.
        /// </summary>
        /// <param name="director">바인딩을 제거할 PlayableDirector</param>
        /// <param name="targetObject">바인딩을 제거할 대상 GameObject</param>
        /// <returns>하나 이상의 바인딩이 제거되었는지 여부</returns>
        private bool RemoveBindingsFromTimeline(PlayableDirector director, GameObject targetObject)
        {
            // PlayableDirector 및 타임라인 자산 확인
            if (director == null || !(director.playableAsset is TimelineAsset timelineAsset))
                return false;

            bool anyBindingRemoved = false;
            
            // 타임라인의 모든 트랙 순회
            foreach (var track in timelineAsset.GetOutputTracks())
            {
                // 트랙에 바인딩된 객체 가져오기
                var boundObject = director.GetGenericBinding(track);
                
                // 안전하게 바인딩 객체 확인
                if (boundObject != null)
                {
                    bool shouldRemove = false;
                    
                    // 바인딩된 객체가 Component인 경우 (Animator, AudioSource 등) 
                    if (boundObject is Component component)
                    {
                        shouldRemove = component.gameObject == targetObject;
                    }
                    // 바인딩된 객체가 GameObject인 경우 (Control 트랙 등)
                    else if (boundObject is GameObject obj)
                    {
                        shouldRemove = obj == targetObject;
                    }
                    
                    // 제거 조건이 충족되면 바인딩 제거
                    if (shouldRemove)
                    {
                        director.SetGenericBinding(track, null);
                        anyBindingRemoved = true;
                    }
                }
            }
            
            return anyBindingRemoved;
        }

        /// <summary>
        /// Detect Trigger와 충돌한 경우, Raycast를 쏘아 실제로 보았는지 확인.
        /// Ray와 충돌한다면 보스의 Mode를 Detect로 설정한다.
        /// </summary>
        public void OnTriggerChangeDetected(bool entered, GameObject obj)
        {
            if (CurrentMode == Mode.Detect || CurrentMode == Mode.Catch)
                return;
            
            if (entered && obj.CompareTag(PLAYER_TAG))
            {
                // Player와 Boss 사이의 오브젝트 확인
                _target = obj.transform;
                
                // Ray를 쏴서 장애물 여부 확인
                Vector3 origin = transform.position;               // Ray 시작점
                origin.y += 6.0f;                                 // Y축을 살짝 올리기 (1.0f는 조정 가능)

                Vector3 directionToTarget = (_target.position - origin).normalized; // 새로운 시작점 기준 방향
                float distanceToTarget = Vector3.Distance(origin, _target.position);
                                
                Ray ray = new Ray(origin, directionToTarget);
                RaycastHit[] hits = Physics.RaycastAll(ray, distanceToTarget);

                // 충돌 결과를 가까운 순서대로 정렬
                System.Array.Sort(hits, (hit1, hit2) => hit1.distance.CompareTo(hit2.distance));
                
                Color rayColor = hits[0].transform.CompareTag(PLAYER_TAG) ? Color.green : Color.red;
                Debug.DrawRay(origin, directionToTarget * distanceToTarget, rayColor, 3.0f);
                //Debug.Log(hits[0].transform.gameObject.name);

                // 플레이어 탐지를 했을 때 Detect 모드로 변경하고, 현재 타임라인을 중단
                if (hits[0].transform.CompareTag(PLAYER_TAG))
                {
                    // 보스 오브젝트 제어 중인 타임라인에서 바인딩 제거
                    if (_currentPlayable.Director != null)
                    {
                        RemoveBindingsFromTimeline(_currentPlayable.Director, gameObject);    
                    }
                    // Detect 모드로 변경
                    SetMode(Mode.Detect, "PlayerDetected");
                    return;
                }
            }
        }

        /// <summary>
        /// Catch Trigger(Boss의 Hand)와 충돌한 경우
        /// 즉, 보스에게 잡힌 경우 Gameover 시켜야 한다.
        /// </summary>
        /// <param name="entered"></param>
        /// <param name="obj"></param>
        public void OnTriggerChangeCatched(bool entered, GameObject obj)
        {
            if (entered && obj.CompareTag(PLAYER_TAG))
            {
                _target = obj.transform;
                // 보스 오브젝트 제어 중인 타임라인에서 바인딩 제거
                if (_currentPlayable.Director != null)
                {
                    RemoveBindingsFromTimeline(_currentPlayable.Director, gameObject);    
                }
                // Catch 모드로 변경
                SetMode(Mode.Catch, "OnTriggerChangeCatched");
            }
        }

        /// <summary>
        /// Boss의 NavAgent가 멈췄는지 여부를 반환
        /// </summary>
        public bool IsStopped()
        {
            if (!_agent.pathPending                                         // 경로 계산이 완료되었고
                && _agent.remainingDistance <= _agent.stoppingDistance      // 목표 지점까지 남은 거리가 stoppingDistance 이하이며
                && (!_agent.hasPath || _agent.velocity.sqrMagnitude <= 0f)) // 이동 중이 아니면
            {
                return true;
            }
            return false;
        }
    }
}