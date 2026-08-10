using UnityEngine;

/// <summary>
/// 迷路のゴール地点。
/// プレイヤーが触れると MazeGameManager にクリアを通知する。
/// </summary>
public class MazeGoal : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;
        MazeGameManager.Instance?.OnGoalReached();
    }
}