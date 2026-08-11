using UnityEngine;

/// <summary>
/// 迷路内に配置される収集アイテム。
/// プレイヤーが触れるとスコアを加算し、SEを再生して消滅する。
/// </summary>
public class MazeItem : MonoBehaviour
{
    [Header("スコア加算値")]
    public int scoreValue = 100;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        // スコア加算
        if (MazeGameManager.Instance != null)
        {
            MazeGameManager.Instance.OnItemCollected(scoreValue);
        }

        // アイテム取得SE
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayItemGet();
        }

        // アイテムを削除
        Destroy(gameObject);
    }
}