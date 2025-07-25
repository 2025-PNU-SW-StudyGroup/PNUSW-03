using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITutorial : MonoBehaviour
{
    [SerializeField] private TutorialSO _tutorial;
    [SerializeField] private float _duration = 0.8f;

    [Header("Listening to")]
    [SerializeField] private FloatEventChannelSO _floatTutorial;

    private void OnEnable()
    {
        _tutorial.Image = transform.GetChild(0).GetComponent<Image>();
        _tutorial.Tmp = transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        _floatTutorial.OnEventRaised += SetAlphaFloatingUI;
    }

    private void SetAlphaFloatingUI(float alpha)
    {
        // 알파값 조정
        _tutorial.Image.DOFade(alpha, _duration);
        _tutorial.Tmp.DOFade(alpha, _duration);
    }
}
