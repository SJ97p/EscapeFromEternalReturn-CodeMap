using HBDinosaur_ER_Project.Core;
using TMPro;
using UnityEngine;

public class InGameTimerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _timerText;


    private void Start()
    {
        UpdateTimer(TimeManager.Instance.CurrentTime);
    }

    private void OnEnable()
    {
        TimeManager.Instance.OnTimeUpdated += UpdateTimer;
    }

    private void OnDisable()
    {
        if (TimeManager.Instance == null)
            return;

        TimeManager.Instance.OnTimeUpdated -= UpdateTimer;
    }

    private void UpdateTimer(float currentTime)
    {
        int minute = Mathf.FloorToInt(currentTime / 60f);
        int second = Mathf.FloorToInt(currentTime % 60f);

        _timerText.text = $"{minute:00}:{second:00}";
    }
}
