using Maggi.StateMachine.ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;

//public enum InteractionType { General, NonPossession, Possession, 
//    None, Light, Heavy, Wall, Point, SieveWheel, Globe, Normal, Key};

public class InteractionManager : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader = default;

    // Events for the different interaction types
    [ReadOnly] public InteractionType currentInteractionType;
    [ReadOnly] public GameObject currentInteractiveObject;
    [ReadOnly] public bool pullInput = false;
    [ReadOnly] public bool pushInput = false;
    
    // To store the objects we the player could potentially interact with
    private LinkedList<Interaction> _potentialInteractions = new(); 
    // Dictionary로 중복 삽입 방지 & 빠른 제거
    private Dictionary<GameObject, LinkedListNode<Interaction>> 
        _interactionLookup = new Dictionary<GameObject, LinkedListNode<Interaction>>();
    
    private void OnEnable()
    {
        _inputReader.PullEvent += OnPullInitiated;
        _inputReader.PullCancelEvent += OnPullCancelInitiated;
        _inputReader.PushEvent += OnPushInitiated;
        _inputReader.PushCancelEvent += OnPushCancelInitiated;
    }

    private void OnDisable()
    {
        _inputReader.PullEvent -= OnPullInitiated;
        _inputReader.PullCancelEvent -= OnPullCancelInitiated;
        _inputReader.PushEvent -= OnPushInitiated;
        _inputReader.PushCancelEvent -= OnPushCancelInitiated;
    }

    public void InitCurrentInteraction()
    {
        OnTriggerChangeDetected(false, currentInteractiveObject.gameObject);
        currentInteractiveObject = null;
        currentInteractionType = InteractionType.None;
    }

    private void OnPullInitiated()
    {
        pullInput = true;

        if (_potentialInteractions.Count == 0 ) return;
        if (currentInteractiveObject != null) return;
        
        currentInteractionType = _potentialInteractions.First.Value.type;
        currentInteractiveObject = _potentialInteractions.First.Value.interactiveObject;
    }

    private void OnPullCancelInitiated()
    {
        pullInput = false;
    }

    private void OnPushInitiated()
    {
        pushInput = true;
    }

    private void OnPushCancelInitiated()
    {
        pushInput = false;
    }

    public void OnTriggerChangeDetected(bool entered, GameObject obj)
    {
        if (entered)
            AddPotentialInteraction(obj);
        else
            RemovePotentialInteraction(obj);
    }
    
    private void AddPotentialInteraction(GameObject obj)
    {
        if (!obj.TryGetComponent(out InteractiveObject io) || io.m_Type == InteractionType.None)
            return;

        // 이미 Look-up에 존재하면 더 이상 추가하지 않음
        if (_interactionLookup.ContainsKey(obj))
            return;

        // 새 Interaction 생성하여 LinkedList 앞에 추가
        var newInteraction = new Interaction(io.m_Type, obj);
        var node = _potentialInteractions.AddFirst(newInteraction);

        // Look-up dictionary에 노드 저장
        _interactionLookup[obj] = node;
    }

    private void RemovePotentialInteraction(GameObject obj)
    {
        // Look-up에서 노드를 바로 꺼내기
        if (_interactionLookup.TryGetValue(obj, out var node))
        {
            // LinkedList에서 제거
            _potentialInteractions.Remove(node);
            // Look up dictionary에서도 키 제거
            _interactionLookup.Remove(obj);
        }
    }

    // private void AddPotentialInteraction(GameObject obj)
    // {
    //     // Zone Trigger 범위에 있는 오브젝트를 가져옴
    //     if (obj.TryGetComponent(out InteractiveObject io) && io.m_Type != InteractionType.None)
    //     {
    //         _potentialInteractions.AddFirst(new Interaction(io.m_Type, obj));
    //     }
    // }
    //
    // private void RemovePotentialInteraction(GameObject obj)
    // {
    //     LinkedListNode<Interaction> currentNode = _potentialInteractions.First;
    //     while (currentNode != null)
    //     {
    //         if (currentNode.Value.interactiveObject == obj)
    //         {
    //             _potentialInteractions.Remove(currentNode);
    //             break;
    //         }
    //         currentNode = currentNode.Next;
    //     }
    // }
}
