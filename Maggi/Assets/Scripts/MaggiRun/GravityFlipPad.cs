using UnityEngine;

public class GravityFlipPad : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var controller = other.GetComponent<PlayerMaggiRun>();
            if (controller != null)
            {
                controller.FlipGravity();
            }
        }
    }
}