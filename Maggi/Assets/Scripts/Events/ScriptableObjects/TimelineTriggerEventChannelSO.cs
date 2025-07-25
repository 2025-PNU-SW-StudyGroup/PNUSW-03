using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Timeline;

[CreateAssetMenu(menuName = "Events/Timeline Trigger Event Channel")]
public class TimelineTriggerEventChannelSO : DescriptionBaseSO
{
    public UnityAction<TimelineAsset, Transform> OnEventRaised;

    public void RaiseEvent(TimelineAsset timeline, Transform triggerTransform)
    {
        if (OnEventRaised != null)
            OnEventRaised?.Invoke(timeline, triggerTransform);
    }
}
