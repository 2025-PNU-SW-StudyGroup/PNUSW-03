using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Scene UI")]
    [SerializeField] private UIPanelSwitcher _panelContainer;
    [SerializeField] private UIPopup _popupPanel;
    [SerializeField] private UIPause _pauseScreen;
    [SerializeField] private UISettingController _settingScreen;
    [SerializeField] private UIControlController _controlScreen;
    private RectTransform _settingScreenRect;
    private RectTransform _controlScreenRect;
    
    
    [Header("Gameplay")]
    [SerializeField] private MenuSO _mainMenu;
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private SaveLoadSystem _saveLoadSystem; // 얘는 나중에 다른 곳으로 옮겨야 할지도
    [SerializeField] private PointStorageSO _pointStorageSO; // 얘는 나중에 다른 곳으로 옮겨야 할지도

    [Header("Listening to")]
    [SerializeField] private VoidEventChannelSO _onChangeResolution;
    
    [Header("Broadcasting on")]
    [SerializeField] private LoadEventChannelSO _loadMenuEvent;
    [SerializeField] private VoidEventChannelSO _onContinueButton;

    private void Awake()
    {
        RepositionPanels();
    }

    private void OnEnable()
    {
        _inputReader.MenuPauseEvent += OpenUIPause;
        _onChangeResolution.OnEventRaised += RepositionPanels;
    }

    private void OnDisable()
    {
        _inputReader.MenuPauseEvent -= OpenUIPause;
        _onChangeResolution.OnEventRaised -= RepositionPanels;
        
        // Pause UI 위치 원위치
        transform.position = Vector3.zero;
    }

    private void RepositionPanels()
    {
        // 1) CanvasScaler에서 Reference Resolution 가져오기
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) return;

        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler == null)
        {
            Debug.LogError("CanvasScaler 컴포넌트를 찾을 수 없습니다.");
            return;
        }

        Vector2 refResolution = scaler.referenceResolution;
        float refW = refResolution.x;  // 예: 1920
        float refH = refResolution.y;  // 예: 1080

        // 2) Window들의 RectTransform 가져오기
        _settingScreenRect = _settingScreen.GetComponent<RectTransform>();
        _controlScreenRect = _controlScreen.GetComponent<RectTransform>();

        // 3) Stretch 상태에서 offset으로 좌우 끝 배치
        // 오른쪽 패널: 레퍼런스 폭만큼 밀어서 캔버스 우측 바깥으로
        _settingScreenRect.offsetMin = new Vector2(+refW, _settingScreenRect.offsetMin.y);
        _settingScreenRect.offsetMax = new Vector2(+refW, _settingScreenRect.offsetMax.y);

        // 왼쪽 패널: 레퍼런스 폭만큼 밀어서 캔버스 좌측 바깥으로
        _controlScreenRect.offsetMin = new Vector2(-refW, _controlScreenRect.offsetMin.y);
        _controlScreenRect.offsetMax = new Vector2(-refW, _controlScreenRect.offsetMax.y);
    }

    private void OpenUIPause()
    {
        _inputReader.MenuPauseEvent -= OpenUIPause; // you can open UI pause menu again, if it's closed

        Time.timeScale = 0.0f; // Pause Time

        _pauseScreen.Restarted += RestartAtLastSavePoint;
        _pauseScreen.SettingScreenOpened += OpenSettingScreen;
        _pauseScreen.ControlScreenOpened += OpenControlScreen;
        _pauseScreen.Resumed += CloseUIPause;
        _pauseScreen.BackToMainRequested += ShowBackToMenuConfirmationPopup;

        _panelContainer.gameObject.SetActive(true);

        _inputReader.EnableMenuInput();
    }

    private void RestartAtLastSavePoint()
    {
        CloseUIPause();

        _onContinueButton.RaiseEvent();
    }

    private void CloseUIPause()
    {
        Time.timeScale = 1.0f;

        _inputReader.MenuPauseEvent += OpenUIPause; // you can open UI pause menu again, if it's closed

        _pauseScreen.Restarted -= RestartAtLastSavePoint;
        _pauseScreen.SettingScreenOpened -= OpenSettingScreen;
        _pauseScreen.ControlScreenOpened -= OpenControlScreen;
        _pauseScreen.Resumed -= CloseUIPause;
        _pauseScreen.BackToMainRequested -= ShowBackToMenuConfirmationPopup;

        _panelContainer.gameObject.SetActive(false);

        _inputReader.EnableGameplayInput();
    }

    private void OpenSettingScreen()
    {
        _settingScreen.Closed += CloseSettingScreen;
        _panelContainer.SwitchToTarget(_settingScreenRect, 0);
    }

    private void OpenControlScreen()
    {
        _controlScreen.Closed += CloseControlScreen;
        _panelContainer.SwitchToTarget(_controlScreenRect, 1);
    }

    private void CloseSettingScreen()
    {
        _settingScreen.Closed -= CloseSettingScreen;
        _panelContainer.SwitchToHome(0);
    }

    private void CloseControlScreen()
    {
        _controlScreen.Closed -= CloseControlScreen;
        _panelContainer.SwitchToHome(1);
    }

    private void ShowBackToMenuConfirmationPopup()
    {
        _pauseScreen.gameObject.SetActive(false);

        _popupPanel.ClosePopupAction += HideBackToMenuConfirmationPopup;
        _popupPanel.ConfirmationResponseAction += BackToMainMenu;

        _inputReader.EnableMenuInput();
        _popupPanel.gameObject.SetActive(true);
        _popupPanel.SetPopup(PopupType.BackToMenu);
    }

    private void BackToMainMenu(bool confirm)
    {
        HideBackToMenuConfirmationPopup(); // hide confirmation screen, show close UI pause, 

        if (confirm)
        {
            CloseUIPause();
            _loadMenuEvent.RaiseEvent(_mainMenu, false);
        }
    }

    private void HideBackToMenuConfirmationPopup()
    {
        _popupPanel.ClosePopupAction -= HideBackToMenuConfirmationPopup;
        _popupPanel.ConfirmationResponseAction -= BackToMainMenu;

        _popupPanel.gameObject.SetActive(false);
        _pauseScreen.gameObject.SetActive(true);
    }
}
