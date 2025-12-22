using UnityEngine;
using TMPro;
using System;

public class TimerController : MonoBehaviour
{
    [Header("Timer")]
    public TMP_Text timerText;
    public float totalTime = 60f;

    private float timeLeft;
    bool timerRunning = false;

    public Action OnTimerEnd;

    void Update()
    {
        if (!timerRunning) return;

        timeLeft -= Time.deltaTime;
        timerText.text = $"{Mathf.Ceil(timeLeft)}";

        if (timeLeft <= 0)
        {
            timerRunning = false;
            timeLeft = 0;
            timerText.text = "0";
            OnTimerEnd?.Invoke();
        }
    }

    public void StartTimer()
    {
        timeLeft = totalTime;
        timerRunning = true;
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    public bool IsRunning()
    {
        return timerRunning;
    }
}