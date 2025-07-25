using UnityEngine;

public class Attack : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(gameObject.tag))
        {
            if (other.TryGetComponent(out Damagable damagableComp))
            {
                Debug.Log(gameObject.name);
                damagableComp.Die();
            }
        }
    }
}
