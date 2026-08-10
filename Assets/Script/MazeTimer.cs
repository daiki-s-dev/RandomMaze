using System;
using UnityEngine;

/// <summary>
/// 制限時間のカウントダウンを管理する。
/// 0になると OnTimeUp イベントを発火する。
/// </summary>
public class MazeTimer : MonoBehaviour
{
    [Header("制限時間（秒）")]
    public float timeLimit = 60f;

    public Action OnTimeUp;
    public Action<float> OnTimeChanged;

    private float remainingTime;
    private bool isRunning = false;

    public float RemainingTime => remainingTime;

    private void Update()
    {
        if (!isRunning) return;

        remainingTime -= Time.deltaTime;
        OnTimeChanged?.Invoke(remainingTime);

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            isRunning = false;
            OnTimeUp?.Invoke();
        }
    }

    public void StartTimer()
    {
        remainingTime = timeLimit;
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }
}