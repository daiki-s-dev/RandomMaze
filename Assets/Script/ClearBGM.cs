using UnityEngine;

/// <summary>
/// ClearScene‚É“ü‚Á‚½‚Æ‚«‚ÉBGM‚ğ’â~‚·‚éB
/// </summary>
public class StopBGM : MonoBehaviour
{
    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopBGM();
        }
    }
}