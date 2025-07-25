using System;
using UnityEngine;
using UnityEngine.Events;



// TODO : 조작키를 바꿀 수 있게 구현해야 한다.

public class UIControlController : MonoBehaviour
{
    [SerializeField] private UIGenericButton _backButton = default;
    
    public UnityAction Closed;

    private void OnEnable()
    {
        _backButton.Clicked += ClosedScreen;
    }

    private void OnDisable()
    {
        _backButton.Clicked -= ClosedScreen;
    }

    private void ClosedScreen()
    {
        Closed?.Invoke();
    }
}
