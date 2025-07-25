using UnityEngine;

public class TileRunner : MonoBehaviour
{
    public TileSpawner spawner;
    private PlayerMaggiRun player;

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.GetComponent<PlayerMaggiRun>();
    }

    void Update()
    {
        if (spawner == null || player == null || player.IsDead)
            return;

        transform.Translate(spawner.moveDirection * spawner.moveSpeed * Time.deltaTime, Space.World);
    }

    void FixedUpdate()
    {
        if (spawner == null || player == null || player.IsDead)
            return;

        if (transform.position.z < spawner.destroyZ)
        {
            spawner.RecycleTile(this);
        }
    }
}