using UnityEngine;

public class PrototypeCompleteZone : MonoBehaviour
{
    [SerializeField] private UIPopup _popupPanel;
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private string _playerTag = "Player";

    [Header("Gameplay")]
    [SerializeField] private MenuSO _mainMenu;

    [Header("Broadcasting on")]
    [SerializeField] private BoolEventChannelSO _toggleLoadingScreen;
    [SerializeField] private FadeChannelSO _fadeRequestChannel;
    [SerializeField] private LoadEventChannelSO _loadMenuEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_playerTag))
        {
            ShowThanksPopup();
        }
    }

    private void ShowThanksPopup()
    {
        Time.timeScale = 0.0f;

        _popupPanel.ConfirmationResponseAction += BackToMainMenu;

        _popupPanel.gameObject.SetActive(true);
        _popupPanel.SetPopup(PopupType.DonePrototype);

        _inputReader.EnableMenuInput();
    }

    private void BackToMainMenu(bool confirm)
    {
        Time.timeScale = 1.0f;

        HideThanksPopup();// hide confirmation screen, show close UI pause, 

        if (confirm)
        {
            _loadMenuEvent.RaiseEvent(_mainMenu, false);
        }
    }

    private void HideThanksPopup()
    {
        _popupPanel.ConfirmationResponseAction -= BackToMainMenu;
        _popupPanel.gameObject.SetActive(false);
    }
}
