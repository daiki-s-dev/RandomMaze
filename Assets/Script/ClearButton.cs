using UnityEngine;

/// <summary>
/// クリア画面のボタン処理。
/// </summary>
public class ClearButton : MonoBehaviour
{
    /// <summary>
    /// タイトルボタンを押したときにTitleSceneへ移動する。
    /// </summary>
    public void GoToTitle()
    {
        if (SceneController.Instance != null)
        {
            SceneController.Instance.LoadTitle();
        }
        else
        {
            Debug.LogError("SceneController.Instance が見つかりません。");
        }
    }
}