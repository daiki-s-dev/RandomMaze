using UnityEngine;

/// <summary>
/// タイトル画面のボタン処理。
/// </summary>
public class TitleButton : MonoBehaviour
{
    [Header("遊び方パネル")]
    public GameObject howToPlayPanel;

    private void Start()
    {
        // ゲーム開始時は遊び方パネルを閉じる
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
        }
    }

    /// <summary>
    /// ゲーム開始。
    /// </summary>
    public void StartGame()
    {
        if (SceneController.Instance != null)
        {
            SceneController.Instance.LoadMaze();
        }
    }

    /// <summary>
    /// 遊び方を開く。
    /// </summary>
    public void OpenHowToPlay()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(true);
        }
    }

    /// <summary>
    /// 遊び方を閉じる。
    /// </summary>
    public void CloseHowToPlay()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
        }
    }
}