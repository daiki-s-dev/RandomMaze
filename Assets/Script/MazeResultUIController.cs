using TMPro;
using UnityEngine;

/// <summary>
/// 迷路ゲームのクリア画面（ClearScene）UI。
/// スコアの内訳と最終スコアを表示する。
/// </summary>
public class MazeResultUIController : MonoBehaviour
{
    [Header("表示テキスト")]
    public TextMeshProUGUI resultText;

    [Header("スコア内訳")]
    public TextMeshProUGUI itemScoreText;
    public TextMeshProUGUI timeScoreText;
    public TextMeshProUGUI clearBonusText;

    [Header("最終結果")]
    public TextMeshProUGUI scoreText;

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
        int itemScore = MazeResultHolder.Instance.LastItemScore;

        int remainingTime =
            MazeResultHolder.Instance.LastRemainingTime;

        int timeScore =
            MazeResultHolder.Instance.LastTimeScore;

        int clearBonus =
            MazeResultHolder.Instance.LastClearBonus;


        // CLEAR / TIME UP
        if (resultText != null)
        {
            resultText.text =
                cleared ? "クリアおめでとう!" : "時間切れ";
        }


        // アイテムスコア
        if (itemScoreText != null)
        {
            itemScoreText.text =
                $"コイン : {itemCount}枚 × 100 = {itemScore}";
        }


        // 残り時間スコア
        if (timeScoreText != null)
        {
            timeScoreText.text =
                $"残り時間 : {remainingTime}秒 × 10 = {timeScore}";
        }


        // クリアボーナス
        if (clearBonusText != null)
        {
            if (cleared)
            {
                clearBonusText.text =
                    $"クリアボーナス : {clearBonus}";
            }
            else
            {
                clearBonusText.text =
                    "クリアボーナス : 0";
            }
        }


        // 最終スコア
        if (scoreText != null)
        {
            scoreText.text =
                $"トータル　スコア : {score}";
        }
    }
}