using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// ボタンのカーソル・クリック時のSEを再生する。
/// </summary>
public class ButtonSE : MonoBehaviour,
    IPointerEnterHandler,
    IPointerClickHandler
{
    /// <summary>
    /// ボタンにカーソルを合わせたとき。
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonHover();
        }
    }

    /// <summary>
    /// ボタンをクリックしたとき。
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }
    }
}