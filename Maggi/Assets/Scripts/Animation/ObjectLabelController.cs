using TMPro;
using UnityEngine;
using UnityEngine.UI; // 또는 TMPro를 쓰면 using TMPro;

public class ObjectLabelController : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Canvas _uiCanvas;        // Screen Space Overlay Canvas
    [SerializeField] private RectTransform _labelPrefab;
    [SerializeField] private float _showDistance = 3f;
    private RectTransform _labelInstance;
    private Transform _target;           // 머리 위에 띄울 오브젝트

    private CanvasGroup _canvasGroup;
    private TextMeshProUGUI _labelText;        // 또는 TextMeshProUGUI

    public void Initialize(Transform target, string text)
    {
        _target = target;
        // 프리팹 인스턴스
        _labelInstance = Instantiate(_labelPrefab, _uiCanvas.transform);
        _canvasGroup   = _labelInstance.GetComponent<CanvasGroup>();
        _labelText     = _labelInstance.GetComponentInChildren<TextMeshProUGUI>();
        _labelText.text = text;
        _canvasGroup.alpha = 0f; // 초기에 숨김
    }

    private void Update()
    {
        if (_target == null) return;

        // 1) 카메라와 거리
        float dist = Vector3.Distance(_mainCamera.transform.position, _target.position);
        if (dist <= _showDistance)
        {
            // 2) 월드 → 스크린 좌표 변환 (머리 위 오프셋 1.5 단위)
            Vector3 worldPos = _target.position + Vector3.up * 1.5f;
            Vector3 screenPos = _mainCamera.WorldToScreenPoint(worldPos);

            // 화면 뒤쪽(-z)면 숨기기
            if (screenPos.z < 0f)
            {
                _canvasGroup.alpha = 0f;
            }
            else
            {
                _canvasGroup.alpha = 1f;
                _labelInstance.position = screenPos;
            }
        }
        else
        {
            // 거리를 벗어나면 숨김
            _canvasGroup.alpha = 0f;
        }
    }

    private void OnDestroy()
    {
        if (_labelInstance != null)
            Destroy(_labelInstance.gameObject);
    }
}