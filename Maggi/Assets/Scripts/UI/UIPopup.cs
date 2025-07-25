using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum PopupType
{
    Quit,
    NewGame,
    BackToMenu,
    DonePrototype,
}

public class UIPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private Button _buttonClose ;
    [SerializeField] private UIGenericButton _popupButton1;
    [SerializeField] private UIGenericButton _popupButton2;
    [SerializeField] private InputReader _inputReader;

    [SerializeField] private PopupType _actualType;

    public event UnityAction<bool> ConfirmationResponseAction;
    public event UnityAction ClosePopupAction;

    public void SetPopup(PopupType popupType)
    {
        _actualType = popupType;
        bool isConfirmation = false;
        bool hasExitButton = false;

        switch (_actualType)
        {
        case PopupType.Quit:
            isConfirmation = true;
            _titleText.text = "EXIT";
            _descriptionText.text = "정말 게임을 나가시겠습니까?";
            _popupButton1.SetButton("예", true);
            _popupButton2.SetButton("취소", false);
            hasExitButton = false;
            break;
        case PopupType.NewGame:
            isConfirmation = true;
            _titleText.text = "New";
            _descriptionText.text = "새로운 게임을 시작하시겠습니까?";
            _popupButton1.SetButton("예", true);
            _popupButton2.SetButton("취소", false);
            hasExitButton = false;
            break;
        case PopupType.BackToMenu:
            isConfirmation = true;
            _titleText.text = "Main";
            _descriptionText.text = "메인 메뉴로 나가시겠습니까?";
            _popupButton1.SetButton("예", true);
            _popupButton2.SetButton("취소", false);
            hasExitButton = false;
            break;
        case PopupType.DonePrototype:
            isConfirmation = false;
            _titleText.text = "";
            _descriptionText.text = 
                "마기와 함께 여기까지 달려오신\n" +
                "여러분, 진심으로 감사드립니다\n" +
                "마기의 여정은 아직 끝나지 않았습니다\n" +
                "마기와 함께하는 다음 모험, 기대해 주세요.";
            _popupButton1.SetButton("감사합니다", true);
            hasExitButton = false;
            break;
        default:
            isConfirmation = false;
            hasExitButton = false;
            break;
        }

        if (isConfirmation) // needs two button : Is a decision 
        {
            _popupButton1.gameObject.SetActive(true);
            _popupButton2.gameObject.SetActive(true);

            _popupButton1.Clicked += ConfirmButtonClicked;
            _popupButton2.Clicked += CancelButtonClicked;
        }
        else // needs only one button : Is an information 
        {
            _popupButton1.gameObject.SetActive(true);
            _popupButton2.gameObject.SetActive(false);

            _popupButton1.Clicked += ConfirmButtonClicked;
        }

        if (hasExitButton)
        {
            _inputReader.MenuCloseEvent += ClosePopupButtonClicked;
        }

        _buttonClose.gameObject.SetActive(hasExitButton);
    }

    private void ClosePopupButtonClicked()
    {
        ClosePopupAction?.Invoke();
    }

    private void ConfirmButtonClicked()
    {
        ConfirmationResponseAction?.Invoke(true);
    }

    private void CancelButtonClicked()
    {
        ConfirmationResponseAction?.Invoke(false);
    }

    private void OnValidate()
    {
        SetPopup(_actualType);
    }
}