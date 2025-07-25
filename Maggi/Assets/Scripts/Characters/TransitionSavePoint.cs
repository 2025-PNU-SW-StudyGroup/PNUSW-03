using System;
using UnityEngine;

public class TransitionSavePoint : MonoBehaviour
{
    [SerializeField] private Transform[] _savePoints;

    private int index;
    private Player _player;
    private CharacterController _cc;

    private void Start()
    {
    }

    private void Update()
    {
        if (_player == null)
        {
            _player = FindAnyObjectByType<Player>();
            return;
        }

        if (_cc == null && _player != null)
        {
            _cc = _player.GetComponent<CharacterController>();
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            Vector3 dest = _savePoints[index].position;
            _cc.enabled = false;           // 충돌계산 잠깐 끔
            _player.transform.position = dest;
            _cc.enabled = true;
            index = (index + 1) % _savePoints.Length;
        }
    }
}
