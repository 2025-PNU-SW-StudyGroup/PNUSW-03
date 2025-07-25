using Maggi.Character.Boss;
using UnityEngine;

public class TriggerDeadzoneListener : MonoBehaviour
{
    [SerializeField] private string _tag = "Player";
    [SerializeField] private Boss _boss;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_tag))
        {
            _boss.SetTarget(other.transform);
            _boss.SetMode(Mode.Detect, "TriggerDeadzoneListener");
        }
    }
}
