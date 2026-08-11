using UnityEngine;

/// <summary>
/// MazeSceneでのゲーム結果をClearSceneへ引き継ぐ。
/// </summary>
public class MazeResultHolder : MonoBehaviour
{
    public static MazeResultHolder Instance { get; private set; }

    // 最終スコア
    public int LastScore { get; private set; }

    // クリアしたか
    public bool LastCleared { get; private set; }

    // 獲得アイテム数
    public int LastItemCount { get; private set; }

    // アイテムによるスコア
    public int LastItemScore { get; private set; }

    // 残り時間によるスコア
    public int LastTimeScore { get; private set; }

    // クリアボーナス
    public int LastClearBonus { get; private set; }

    // クリア時の残り時間
    public int LastRemainingTime { get; private set; }


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
        int itemCount,
        int itemScore,
        int timeScore,
        int clearBonus,
        int remainingTime)
    {
        LastScore = score;
        LastCleared = cleared;

        LastItemCount = itemCount;

        LastItemScore = itemScore;
        LastTimeScore = timeScore;
        LastClearBonus = clearBonus;

        LastRemainingTime = remainingTime;
    }
}