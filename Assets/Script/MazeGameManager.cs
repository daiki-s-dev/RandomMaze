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

    [Tooltip("アイテム1個あたりの基本スコア")]
    public int itemScoreMultiplier = 100;

    [Tooltip("ゴール到達時に加算される固定ボーナス")]
    public int clearBonus = 500;

    public Action<int> OnScoreChanged;
    public Action<int> OnItemCountChanged;
    public Action<int> OnGameClear;
    public Action<int> OnGameOver;

    // 現在のアイテムスコア
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
        itemScore = 0;
        collectedItemCount = 0;

        isCleared = false;
        isGameOver = false;

        mazeGenerator.Generate();

        if (mazeCamera != null)
        {
            mazeCamera.AdjustCamera();
        }

        if (player != null)
        {
            player.position = mazeGenerator.StartWorldPosition;
        }

        if (mazeTimer != null)
        {
            mazeTimer.OnTimeUp += HandleTimeUp;
            mazeTimer.StartTimer();
        }

        OnScoreChanged?.Invoke(itemScore);
        OnItemCountChanged?.Invoke(collectedItemCount);
    }

    #endregion


    #region アイテム取得

    public void OnItemCollected(int value)
    {
        if (isCleared || isGameOver)
            return;

        collectedItemCount++;

        // 今回はアイテム1個につきitemScoreMultiplier点
        itemScore += itemScoreMultiplier;

        OnItemCountChanged?.Invoke(collectedItemCount);
        OnScoreChanged?.Invoke(itemScore);
    }

    #endregion


    #region ゴール

    public void OnGoalReached()
    {
        if (isCleared || isGameOver)
            return;

        isCleared = true;

        if (mazeTimer != null)
        {
            mazeTimer.StopTimer();
        }

        // スコア内訳を計算
        int timeScore = CalculateTimeScore();
        int finalScore = CalculateFinalScore();

        OnGameClear?.Invoke(finalScore);

        // 結果を保存
        if (MazeResultHolder.Instance != null)
        {
            MazeResultHolder.Instance.SetResult(
                finalScore,
                true,
                collectedItemCount,
                itemScore,
                timeScore,
                clearBonus,
                mazeTimer != null
                    ? Mathf.RoundToInt(mazeTimer.RemainingTime)
                    : 0
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

    private void HandleTimeUp()
    {
        if (isCleared || isGameOver)
            return;

        isGameOver = true;

        // 時間切れの場合は残り時間0
        int timeScore = 0;

        int finalScore = CalculateFinalScore();

        OnGameOver?.Invoke(finalScore);

        if (MazeResultHolder.Instance != null)
        {
            MazeResultHolder.Instance.SetResult(
                finalScore,
                false,
                collectedItemCount,
                itemScore,
                timeScore,
                0,
                0
            );
        }
    }

    #endregion


    #region スコア計算

    /// <summary>
    /// 残り時間によるスコアを計算する。
    /// </summary>
    private int CalculateTimeScore()
    {
        if (mazeTimer == null)
            return 0;

        return Mathf.RoundToInt(
            mazeTimer.RemainingTime
        ) * timeScoreMultiplier;
    }

    /// <summary>
    /// 最終スコアを計算する。
    /// </summary>
    private int CalculateFinalScore()
    {
        int score = itemScore;

        // 残り時間スコア
        score += CalculateTimeScore();

        // クリアボーナス
        if (isCleared)
        {
            score += clearBonus;
        }

        return score;
    }

    #endregion
}