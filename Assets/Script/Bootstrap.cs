using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲーム起動時に最初に実行されるエントリーポイント。
/// タイトルシーンを読み込む。
/// </summary>
public class Bootstrap : MonoBehaviour
{
    private void Start()
    {
        // タイムスケールをリセット
        Time.timeScale = 1f;

        // TitleSceneだけを読み込む
        SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
    }
}