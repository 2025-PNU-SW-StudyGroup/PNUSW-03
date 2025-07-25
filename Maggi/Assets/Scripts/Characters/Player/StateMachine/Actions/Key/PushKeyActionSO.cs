using UnityEngine;
using Maggi.StateMachine;
using Maggi.StateMachine.ScriptableObjects;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PushKeyAction", menuName = "State Machines/Actions/Push Key Action")]
public class PushKeyActionSO : StateActionSO<PushKeyAction>
{
    public float pushForce = 5.0f;
    public float pushHeight = 50.0f;
}

public class PushKeyAction : StateAction
{
    private PushKeyActionSO _originSO => (PushKeyActionSO)base.OriginSO;
    private InteractionManager _interactionManager;
    private Rigidbody _interactiveObjectRigidbody;
    private InteractiveObject _keyObject;
    
    public override void Awake(InteractiveObject interactiveObject, GameObject owner)
    {
        _interactionManager = owner.GetComponent<InteractionManager>();
        _keyObject = interactiveObject;;
    }

    public override void OnUpdate() { }

    public override void OnStateEnter()
    {
        // 플레이어를 중심으로 구 콜라이더를 생성해 주변에 상호작용 오브젝트가 있는지 체크
        Collider[] hitColliders = Physics.OverlapSphere(_keyObject.transform.position, 1.0f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.TryGetComponent(out InteractionEventListener e) && _keyObject.TryGetComponent(out Key key))
            {
                List<InteractionEventListener> listeners = new List<InteractionEventListener>(hitCollider.GetComponents<InteractionEventListener>());
                
                foreach (var listener in listeners)
                {
                    if (listener.RequiredKey != null && listener.RequiredKey.ID == key.GetKeyID())
                    {
                        listener.IsEnable = true;
                        InteractWithObject(hitCollider.gameObject, key);
                        return;
                    }
                    else
                    {
                        Debug.Log("키를 사용할 수 없습니다.");
                    }
                }
            }
            else
            {
                Debug.LogWarning("There are no Interaction Event Listener or Key _ PushKeyActionSO.cs");
            }
        }

        // 상호작용 못 하면 그냥 던지는 동작
        _interactiveObjectRigidbody = _interactionManager.currentInteractiveObject.GetComponent<Rigidbody>();

        // Init Position to Player position and Add
        _interactiveObjectRigidbody.transform.position = _interactionManager.transform.position + _interactiveObjectRigidbody.transform.forward * 0.2f;
        _interactiveObjectRigidbody.linearVelocity = _interactiveObjectRigidbody.transform.forward * _originSO.pushForce + _interactiveObjectRigidbody.transform.up * _originSO.pushHeight;
        
    }

    private void InteractWithObject(GameObject target, Key key)
    {
        // 키 삭제
        key.Destroy();

        // 숨겨놨던 키 오브젝트를 활성화
        if (target.TryGetComponent(out ActivateObject activeObject))
        {
            activeObject.Activate();
        }
    }

    public override void OnStateExit()
    {
        _interactionManager.InitCurrentInteraction();
    }
}