using UnityEngine;

/// <summary>
/// MazeSceneでのゲーム結果をClearSceneへ引き継ぐ。
/// </summary>
public class MazeResultHolder : MonoBehaviour
{
    public static MazeResultHolder Instance { get; private set; }

    /// <summary>
    /// 最終スコア
    /// </summary>
    public int LastScore { get; private set; }

    /// <summary>
    /// クリアしたかどうか
    /// </summary>
    public bool LastCleared { get; private set; }

    /// <summary>
    /// 最後に取得したアイテム数
    /// </summary>
    public int LastItemCount { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// ゲーム結果を保存する。
    /// </summary>
    public void SetResult(
        int score,
        bool cleared,
        int itemCount)
    {
        LastScore = score;
        LastCleared = cleared;
        LastItemCount = itemCount;
    }
}