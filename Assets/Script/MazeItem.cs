using UnityEngine;

/// <summary>
/// 迷路内に配置される収集アイテム。
/// プレイヤーが触れると MazeGameManager に取得を通知して消滅する。
/// </summary>
public class MazeItem : MonoBehaviour
{
    [Header("スコア加算値")]
    public int scoreValue = 100;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        // アイテム取得をゲームマネージャーに通知
        if (MazeGameManager.Instance != null)
        {
            MazeGameManager.Instance.OnItemCollected(scoreValue);
        }

        // アイテムを削除
        Destroy(gameObject);
    }
}