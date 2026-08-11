using UnityEngine;

/// <summary>
/// TitleSceneでタイトルBGMを再生する。
/// </summary>
public class TitleBGM : MonoBehaviour
{
    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayTitleBGM();
        }
    }
}