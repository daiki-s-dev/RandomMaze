using System;
using UnityEngine;

/// <summary>
/// 迷路ゲーム全体の進行を統括するシングルトン。
/// 迷路生成、タイマー管理、アイテム取得、ゴール到達、
/// スコア計算、クリアシーンへの遷移を管理する。
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
    [Tooltip("クリア時、残り時間1秒あたりに加算されるスコア")]
    public int timeScoreMultiplier = 10;

    [Tooltip("ゴール到達時に加算される固定ボーナス")]
    public int clearBonus = 500;

    /// <summary>
    /// スコアが変更されたときに呼ばれる。
    /// </summary>
    public Action<int> OnScoreChanged;

    /// <summary>
    /// アイテム取得数が変更されたときに呼ばれる。
    /// </summary>
    public Action<int> OnItemCountChanged;

    /// <summary>
    /// ゴール到達でクリアしたときに呼ばれる。
    /// </summary>
    public Action<int> OnGameClear;

    /// <summary>
    /// 制限時間切れでゲームオーバーになったときに呼ばれる。
    /// </summary>
    public Action<int> OnGameOver;

    // アイテムによる合計スコア
    private int itemScore = 0;

    // 取得したアイテム数
    private int collectedItemCount = 0;

    private bool isCleared = false;
    private bool isGameOver = false;

    /// <summary>
    /// 現在取得しているアイテム数。
    /// </summary>
    public int CollectedItemCount => collectedItemCount;

    /// <summary>
    /// 現在のアイテムスコア。
    /// </summary>
    public int ItemScore => itemScore;


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

    private void StartGame()
    {
        // ゲーム状態を初期化
        itemScore = 0;
        collectedItemCount = 0;

        isCleared = false;
        isGameOver = false;

        // 迷路を生成
        mazeGenerator.Generate();

        // 迷路生成後にカメラを調整
        if (mazeCamera != null)
        {
            mazeCamera.AdjustCamera();
        }

        // プレイヤーをスタート地点へ移動
        if (player != null)
        {
            player.position = mazeGenerator.StartWorldPosition;
        }

        // タイマー開始
        if (mazeTimer != null)
        {
            mazeTimer.OnTimeUp += HandleTimeUp;
            mazeTimer.StartTimer();
        }

        // UIを初期状態にする
        OnScoreChanged?.Invoke(itemScore);
        OnItemCountChanged?.Invoke(collectedItemCount);
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

        // アイテム取得数を1増やす
        collectedItemCount++;

        // アイテムのスコアを加算
        itemScore += value;

        // UIへ通知
        OnItemCountChanged?.Invoke(collectedItemCount);
        OnScoreChanged?.Invoke(itemScore);
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

        // 最終スコアを計算
        int finalScore = CalculateFinalScore(true);

        // クリア通知
        OnGameClear?.Invoke(finalScore);

        // 結果を保存
        if (MazeResultHolder.Instance != null)
        {
            MazeResultHolder.Instance.SetResult(
                finalScore,
                true,
                collectedItemCount
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
                "SceneController.Instance が見つかりません。"
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

        // 最終スコアを計算
        int finalScore = CalculateFinalScore(false);

        // ゲームオーバー通知
        OnGameOver?.Invoke(finalScore);

        // 結果を保存
        if (MazeResultHolder.Instance != null)
        {
            MazeResultHolder.Instance.SetResult(
                finalScore,
                false,
                collectedItemCount
            );
        }
    }

    #endregion


    #region スコア計算

    /// <summary>
    /// 最終スコアを計算する。
    /// </summary>
    private int CalculateFinalScore(bool cleared)
    {
        int score = itemScore;

        if (cleared)
        {
            // クリアボーナス
            score += clearBonus;

            // 残り時間によるボーナス
            if (mazeTimer != null)
            {
                score += Mathf.RoundToInt(
                    mazeTimer.RemainingTime
                ) * timeScoreMultiplier;
            }
        }

        return score;
    }

    #endregion
}