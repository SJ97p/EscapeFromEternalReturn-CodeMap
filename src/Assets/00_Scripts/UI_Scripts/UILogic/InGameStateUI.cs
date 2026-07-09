using HBDinosaur_ER_Project.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameStateUI : MonoBehaviour
{
    [SerializeField] Image _day;
    [SerializeField] Image _night;
    [SerializeField] Image _dawn;
    [SerializeField] TextMeshProUGUI _dayText;

    private void OnEnable()
    {
        TimeManager.Instance.OnStateChanged += ImageChange;
    }

    private void OnDisable()
    {
        if (TimeManager.Instance == null)
            return;

        TimeManager.Instance.OnStateChanged -= ImageChange;
    }

    private void Start()
    {
        ImageChange(TimeManager.Instance.CurrentDay, TimeManager.Instance.CurrentState);
    }

    private void ImageChange(int day, InGameState state)
    {
        switch (state)
        {
            case InGameState.DAY:
                _day.enabled = true;
                _night.enabled = false;
                _dawn.enabled = false;
                _dayText.text = $"{day}일차";
                break;
            case InGameState.NIGHT:
                _day.enabled = false;
                _night.enabled = true;
                _dawn.enabled = false;
                _dayText.text = $"{day}일차";
                break;
            case InGameState.DAWN:
                _day.enabled = false;
                _night.enabled = false;
                _dawn.enabled = true;
                _dayText.text = $"{day}일차";
                break;
        }
    }
}
