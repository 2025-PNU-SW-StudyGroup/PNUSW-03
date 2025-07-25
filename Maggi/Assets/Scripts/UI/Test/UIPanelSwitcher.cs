using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelSwitcher : MonoBehaviour
{
    // 패널들 부모 설정
    [SerializeField] private RectTransform _home;
    // 패널 간 전환되는 속도 설정
    [SerializeField] private float _transitionDuration = 1.5f;
    // Switch 시 따라가는 UI
    [SerializeField] private RectTransform _followedObject;
    private Vector2 _followedObjectOriginalSize;
    // back button 뒷 배경 그라데이션
    [SerializeField] private Image[] _backButtonBackgrounds;
    // Pause window side images
    [SerializeField] private Image[] _sideImages;
    
    private RectTransform _panelContainer; // Pause 화면
    
    private void Awake()
    {
        _panelContainer = GetComponent<RectTransform>();
        _followedObjectOriginalSize = _followedObject.sizeDelta;
    }

    /// <summary>
    /// 현재 위치에서 target 위치로 이동
    /// direction: 1은 right 방향(setting), -1은 left 방향(control). 애니메이션 위함
    /// </summary>
    public void SwitchToTarget(RectTransform target, int direction)
    {
        if (target == null || _panelContainer == null || _followedObject == null) 
            return;
        
        // 1. 타겟 패널의 컨테이너 기준 상대 좌표 계산 후 이동
        // panelContainer의 anchoredPosition으로부터 target의 anchoredPosition 차이 계산
        Vector2 targetPosition = _panelContainer.anchoredPosition - (target.anchoredPosition - _panelContainer.anchoredPosition);
        _panelContainer.DOAnchorPos(targetPosition, _transitionDuration)
            .SetEase(Ease.InOutCubic)
            .SetUpdate(true);
        
        // 2. `_followedObject` width와 height를 타겟 높이로 서서히 변경
        Vector2 targetSize = new Vector2(target.rect.height, target.rect.height);
        
        _followedObject.DOSizeDelta(targetSize, _transitionDuration)
            .SetEase(Ease.InOutCubic).SetUpdate(true);
        
        // 3. 타겟 중심 기준으로 _followedObject 위치 계산 후 이동
        //Vector2 followedTargetPosition = (Vector2)target.anchoredPosition + new Vector2(-target.rect.width * 0.5f, 0f);
        Vector2 followedTargetPosition = (target.anchoredPosition - _panelContainer.anchoredPosition) * 0.5f;
        _followedObject.DOAnchorPos(followedTargetPosition, _transitionDuration)
            .SetEase(Ease.InOutCubic)
            .SetUpdate(true);
        
        // 4. 그라데이션 이미지 서서히 보이도록 변경
        for (int i = 0; i < 2; ++i)
        {
            _backButtonBackgrounds[direction * 2 + i].DOFade(1f, _transitionDuration)
                .SetEase(Ease.InOutQuad).SetUpdate(true); // 부드러운 Ease 애니메이션
        }
        
        // 5. 사이드 이미지 서서히 사라지도록 변경
        _sideImages[direction].DOFade(0f, _transitionDuration / 2f)
            .SetEase(Ease.InOutQuad).SetUpdate(true); // 부드러운 Ease 애니메이션
    }
    
    public void SwitchToHome(int direction)
    {
        if (_home == null || _panelContainer == null) 
            return;

        // 1. 패널 이동 애니메이션 실행
        _panelContainer.DOAnchorPos(_home.anchoredPosition, _transitionDuration)
            .SetEase(Ease.InOutCubic)
            .SetUpdate(true);
        
        // 2. _followedObject 사이즈 및 위치 원상복귀
        _followedObject.DOSizeDelta(_followedObjectOriginalSize, _transitionDuration)
            .SetEase(Ease.InOutCubic).SetUpdate(true);

        _followedObject.DOAnchorPos(_home.anchoredPosition, _transitionDuration)
            .SetEase(Ease.InOutCubic).SetUpdate(true);
        
        // 3. 그라데이션 이미지 서서히 사라지도록 변경
        for (int i = 0; i < 2; ++i)
        {
            _backButtonBackgrounds[direction * 2 + i].DOFade(0f, _transitionDuration)
                .SetEase(Ease.InOutQuad).SetUpdate(true); // 부드러운 Ease 애니메이션
        }
        
        // 4. 사이드 이미지 서서히 보이도록 변경
        _sideImages[direction].DOFade(1f, _transitionDuration / 2f)
            .SetEase(Ease.InOutQuad).SetUpdate(true); // 부드러운 Ease 애니메이션
    }
}