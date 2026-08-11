using System;
using UnityEngine;

/// <summary>
/// 迷路ゲーム全体の進行を統括するシングルトン。
/// 迷路生成、タイマー管理、アイテム取得、ゴール到達、
/// ゲームオーバー、スコア計算、シーン遷移を管理する。
/// </summary>
public class MazeGameManager : MonoBehaviour
{
    public static MazeGameManager Instance { get; private set; }

    [Header("参照")]
    public MazeGenerator mazeGenerator;
    public MazeTimer mazeTimer;
    public Transform player;
    public MazeCameraController mazeCamera;

    [Header("スコア設定")]
    [Tooltip("アイテム1個あたりのスコア")]
    public int itemScoreValue = 100;

    [Tooltip("クリア時、残り時間1秒あたりに加算されるスコア")]
    public int timeScoreMultiplier = 10;

    [Tooltip("ゴール到達時に加算される固定ボーナス")]
    public int clearBonus = 500;

    /// <summary>
    /// アイテム取得時に呼ばれる。
    /// 引数は現在の合計アイテムスコア。
    /// </summary>
    public Action<int> OnScoreChanged;

    /// <summary>
    /// アイテム取得数が変更されたときに呼ばれる。
    /// 引数は現在の獲得アイテム数。
    /// </summary>
    public Action<int> OnItemCountChanged;

    /// <summary>
    /// ゴール到達時に呼ばれる。
    /// 引数は最終スコア。
    /// </summary>
    public Action<int> OnGameClear;

    /// <summary>
    /// 制限時間切れ時に呼ばれる。
    /// 引数は最終スコア。
    /// </summary>
    public Action<int> OnGameOver;

    // アイテムによって獲得したスコア
    private int itemScore = 0;

    // 獲得したアイテム数
    private int collectedItemCount = 0;

    // クリア済みか
    private bool isCleared = false;

    // ゲームオーバー済みか
    private bool isGameOver = false;

    /// <summary>
    /// 現在獲得しているアイテム数。
    /// MazeUIControllerなどから参照する。
    /// </summary>
    public int CollectedItemCount => collectedItemCount;

    #region Unity Lifecycle

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        StartGame();
    }

    private void OnDestroy()
    {
        if (mazeTimer != null)
        {
            mazeTimer.OnTimeUp -= HandleTimeUp;
        }
    }

    #endregion

    #region ゲーム開始

    /// <summary>
    /// ゲームを開始する。
    /// </summary>
    private void StartGame()
    {
        itemScore = 0;
        collectedItemCount = 0;

        isCleared = false;
        isGameOver = false;

        // 迷路を生成
        if (mazeGenerator != null)
        {
            mazeGenerator.Generate();
        }
        else
        {
            Debug.LogError(
                "MazeGameManager: mazeGenerator が設定されていません。"
            );
        }

        // 迷路生成後にカメラを調整
        if (mazeCamera != null)
        {
            mazeCamera.AdjustCamera();
        }

        // プレイヤーをスタート地点へ移動
        if (player != null && mazeGenerator != null)
        {
            player.position = mazeGenerator.StartWorldPosition;
        }

        // タイマー開始
        if (mazeTimer != null)
        {
            mazeTimer.OnTimeUp -= HandleTimeUp;
            mazeTimer.OnTimeUp += HandleTimeUp;

            mazeTimer.StartTimer();
        }
    }

    #endregion

    #region アイテム取得

    /// <summary>
    /// アイテムを取得したときに呼ばれる。
    /// </summary>
    public void OnItemCollected(int value)
    {
        if (isCleared || isGameOver)
            return;

        // アイテム数を1増やす
        collectedItemCount++;

        // アイテムスコアを加算
        itemScore += value;

        // UIへ通知
        OnItemCountChanged?.Invoke(collectedItemCount);
        OnScoreChanged?.Invoke(itemScore);

        // アイテム取得SE
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayItemGet();
        }
    }

    /// <summary>
    /// 現在の獲得アイテム数を取得する。
    /// </summary>
    public int GetCollectedItemCount()
    {
        return collectedItemCount;
    }

    /// <summary>
    /// 現在のアイテムスコアを取得する。
    /// </summary>
    public int GetItemScore()
    {
        return itemScore;
    }

    #endregion

    #region ゴール

    /// <summary>
    /// ゴールに到達したときに呼ばれる。
    /// </summary>
    public void OnGoalReached()
    {
        if (isCleared || isGameOver)
            return;

        isCleared = true;

        // タイマー停止
        if (mazeTimer != null)
        {
            mazeTimer.StopTimer();
        }

        // ゴールSE
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGoal();
        }

        // 残り時間
        int remainingTime = 0;

        if (mazeTimer != null)
        {
            remainingTime =
                Mathf.RoundToInt(mazeTimer.RemainingTime);
        }

        // 残り時間によるスコア
        int timeScore =
            remainingTime * timeScoreMultiplier;

        // 最終スコア
        int finalScore =
            itemScore +
            timeScore +
            clearBonus;

        // クリア通知
        OnGameClear?.Invoke(finalScore);

        // リザルト情報を保存
        if (MazeResultHolder.Instance != null)
        {
            MazeResultHolder.Instance.SetResult(
                finalScore,
                true,
                collectedItemCount,
                itemScore,
                remainingTime,
                timeScore,
                clearBonus
            );
        }
        else
        {
            Debug.LogError(
                "MazeGameManager: MazeResultHolder.Instance が見つかりません。"
            );
        }

        // ClearSceneへ移動
        if (SceneController.Instance != null)
        {
            SceneController.Instance.LoadClear();
        }
        else
        {
            Debug.LogError(
                "MazeGameManager: SceneController.Instance が見つかりません。"
            );
        }
    }

    #endregion

    #region 制限時間切れ

    /// <summary>
    /// 制限時間が0になったときに呼ばれる。
    /// </summary>
    private void HandleTimeUp()
    {
        if (isCleared || isGameOver)
            return;

        isGameOver = true;

        // BGM停止
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopBGM();

            // ゲームオーバーSE
            AudioManager.Instance.PlayGameOver();
        }

        // ゲームオーバー時はアイテムスコアのみ
        int finalScore = itemScore;

        // ゲームオーバー通知
        OnGameOver?.Invoke(finalScore);

        // リザルト情報を保存
        if (MazeResultHolder.Instance != null)
        {
            MazeResultHolder.Instance.SetResult(
                finalScore,
                false,
                collectedItemCount,
                itemScore,
                0,
                0,
                0
            );
        }
        else
        {
            Debug.LogError(
                "MazeGameManager: MazeResultHolder.Instance が見つかりません。"
            );
        }

        // ここではClearSceneには移動しない。
        // ゲームオーバーUIをMazeScene内で表示する。
    }

    #endregion

    #region スコア計算

    /// <summary>
    /// 最終スコアを計算する。
    /// </summary>
    private int CalculateFinalScore(bool cleared)
    {
        int score = itemScore;

        if (cleared && mazeTimer != null)
        {
            // クリアボーナス
            score += clearBonus;

            // 残り時間ボーナス
            score += Mathf.RoundToInt(
                mazeTimer.RemainingTime
            ) * timeScoreMultiplier;
        }

        return score;
    }

    /// <summary>
    /// アイテムスコアを取得する。
    /// </summary>
    public int GetItemScoreValue()
    {
        return itemScore;
    }

    /// <summary>
    /// 残り時間によるスコアを取得する。
    /// </summary>
    public int GetTimeScore()
    {
        if (mazeTimer == null)
            return 0;

        return Mathf.RoundToInt(
            mazeTimer.RemainingTime
        ) * timeScoreMultiplier;
    }

    /// <summary>
    /// クリアボーナスを取得する。
    /// </summary>
    public int GetClearBonus()
    {
        return clearBonus;
    }

    #endregion
}