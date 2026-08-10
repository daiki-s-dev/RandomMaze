using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲーム内のシーン遷移を管理する。
/// </summary>
public class SceneController : MonoBehaviour
{
    public static SceneController Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// タイトル画面へ移動
    /// </summary>
    public void LoadTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }

    /// <summary>
    /// 迷路ゲームへ移動
    /// </summary>
    public void LoadMaze()
    {
        SceneManager.LoadScene("MazeScene");
    }

    /// <summary>
    /// クリア画面へ移動
    /// </summary>
    public void LoadClear()
    {
        SceneManager.LoadScene("ClearScene");
    }
}