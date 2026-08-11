using TMPro;
using UnityEngine;

/// <summary>
/// MazeSceneのUIを管理する。
/// アイテム数、残り時間、ポーズメニュー、ゲームオーバー画面を管理する。
/// </summary>
public class MazeUIController : MonoBehaviour
{
    [Header("常時表示UI")]
    public TextMeshProUGUI itemCountText;
    public TextMeshProUGUI timerText;

    [Header("メニュー")]
    public GameObject pausePanel;

    [Header("ゲームオーバー")]
    public GameObject gameOverPanel;

    private bool isPaused = false;
    private bool isGameOver = false;

    private void Start()
    {
        // メニューを閉じる
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        // ゲームオーバー画面を閉じる
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        Time.timeScale = 1f;

        if (MazeGameManager.Instance != null)
        {
            // アイテム数の更新を登録
            MazeGameManager.Instance.OnItemCountChanged
                += UpdateItemCount;

            // ゲームオーバーの通知を登録
            MazeGameManager.Instance.OnGameOver
                += ShowGameOver;

            // 初期値を表示
            UpdateItemCount(
                MazeGameManager.Instance.CollectedItemCount
            );
        }
    }

    private void Update()
    {
        // ゲームオーバー中はESCによるメニュー操作をしない
        if (!isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }

        UpdateTimer();
    }

    private void OnDestroy()
    {
        if (MazeGameManager.Instance != null)
        {
            MazeGameManager.Instance.OnItemCountChanged
                -= UpdateItemCount;

            MazeGameManager.Instance.OnGameOver
                -= ShowGameOver;
        }

        // シーン移動時などに時間が止まったままにならないようにする
        Time.timeScale = 1f;
    }

    /// <summary>
    /// アイテム数を表示する。
    /// </summary>
    private void UpdateItemCount(int count)
    {
        if (itemCountText == null)
            return;

        itemCountText.text = $"コイン : {count}枚";
    }

    /// <summary>
    /// 残り時間を表示する。
    /// </summary>
    private void UpdateTimer()
    {
        if (timerText == null)
            return;

        if (MazeGameManager.Instance == null)
            return;

        if (MazeGameManager.Instance.mazeTimer == null)
            return;

        float remainingTime =
            MazeGameManager.Instance.mazeTimer.RemainingTime;

        int seconds = Mathf.Max(
            0,
            Mathf.CeilToInt(remainingTime)
        );

        timerText.text = $"残り時間 : {seconds}秒";
    }

    #region ポーズメニュー

    private void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    /// <summary>
    /// ポーズメニューを開く。
    /// </summary>
    public void PauseGame()
    {
        if (isPaused || isGameOver)
            return;

        isPaused = true;

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    /// <summary>
    /// ポーズメニューを閉じる。
    /// </summary>
    public void ResumeGame()
    {
        if (!isPaused || isGameOver)
            return;

        isPaused = false;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        Time.timeScale = 1f;
    }

    #endregion


    #region ゲームオーバー

    /// <summary>
    /// 制限時間切れ時にゲームオーバー画面を表示する。
    /// MazeGameManagerから呼ばれる。
    /// </summary>
    private void ShowGameOver(int finalScore)
    {
        isGameOver = true;

        // ポーズメニューが開いていた場合は閉じる
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        isPaused = false;

        // ゲームオーバー画面を表示
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }


        // ゲームを完全に停止
        Time.timeScale = 0f;
    }

    #endregion


    #region タイトルへ戻る

    /// <summary>
    /// タイトルシーンへ戻る。
    /// </summary>
    public void ReturnToTitle()
    {
        // 時間を必ず元に戻す
        Time.timeScale = 1f;

        if (SceneController.Instance != null)
        {
            SceneController.Instance.LoadTitle();
        }
        else
        {
            Debug.LogError(
                "SceneController.Instance が見つかりません。"
            );
        }
    }

    #endregion
}