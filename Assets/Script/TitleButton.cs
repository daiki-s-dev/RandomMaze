using UnityEngine;

/// <summary>
/// タイトル画面のボタン処理。
/// </summary>
public class TitleButton : MonoBehaviour
{
    /// <summary>
    /// ゲーム開始ボタンを押したときにMazeSceneへ移動する。
    /// </summary>
    public void StartGame()
    {
        if (SceneController.Instance != null)
        {
            SceneController.Instance.LoadMaze();
        }
        else
        {
            Debug.LogError("SceneController.Instance が見つかりません。");
        }
    }
}