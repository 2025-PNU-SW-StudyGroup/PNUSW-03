using UnityEngine;

public class SlowWall : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerMaggiRun player))
        {
            // 부모 SlowWall 오브젝트 기준으로 슬로우모드 진입
            Transform root = transform.parent != null ? transform.parent : transform;
            player.EnterSlowMode(root.gameObject);
        }
    }
}