using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [Header("Component")]
    public TextMeshProUGUI timerText;

    [Header("Timer Settings")]
    public float currentTime;
    public bool countDown;

    [Header("Format Settings")]
    public bool hasFormat;
    public TimerFormats format;

    public bool timerRunning = true;
    private Dictionary<TimerFormats, string> timeFormats = new Dictionary<TimerFormats, string>();

    void Start()
    {
        timeFormats.Add(TimerFormats.Whole, "0");
        timeFormats.Add(TimerFormats.TenthDecimal, "0.0");
        timeFormats.Add(TimerFormats.HundredthsDecimal, "0.00");
    }

    void Update()
    {
        if (!timerRunning) return;

        currentTime = countDown ? currentTime -= Time.deltaTime : currentTime += Time.deltaTime;
        SetTimerText();
    }

    public void StopTimer()
    {
        timerRunning = false;
        Debug.Log("Timer stopped! Final Time:  " + currentTime.ToString());
    }

    private void SetTimerText()
    {
        timerText.text = hasFormat ? currentTime.ToString(timeFormats[format]) : currentTime.ToString();
    }
}

public enum TimerFormats
{
    Whole,
    TenthDecimal,
    HundredthsDecimal
}
