using UnityEngine;

/// <summary>
/// MazeScene‚Å–À˜HBGM‚ğÄ¶‚·‚éB
/// </summary>
public class MazeBGM : MonoBehaviour
{
    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMazeBGM();
        }
    }
}