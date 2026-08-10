using TMPro;
using UnityEngine;

/// <summary>
/// 迷路ゲームのクリア画面（ClearScene）UI。
/// MazeResultHolder に保存された最終スコア、クリア結果、
/// 獲得アイテム数を表示する。
/// </summary>
public class MazeResultUIController : MonoBehaviour
{
    [Header("表示テキスト")]
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI itemCountText;

    private void Start()
    {
        if (MazeResultHolder.Instance == null)
        {
            Debug.LogError(
                "MazeResultUIController: MazeResultHolder が見つかりません！"
            );

            return;
        }

        int score = MazeResultHolder.Instance.LastScore;
        bool cleared = MazeResultHolder.Instance.LastCleared;
        int itemCount = MazeResultHolder.Instance.LastItemCount;

        // クリア結果
        if (resultText != null)
        {
            resultText.text = cleared ? "CLEAR!" : "TIME UP...";
        }

        // 最終スコア
        if (scoreText != null)
        {
            scoreText.text = $"SCORE: {score}";
        }

        // 獲得アイテム数
        if (itemCountText != null)
        {
            itemCountText.text = $"ITEM: {itemCount}";
        }
    }
}