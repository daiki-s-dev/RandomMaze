using UnityEngine;

/// <summary>
/// ゲーム内のBGMとSEを管理する。
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Source")]
    public AudioSource bgmSource;
    public AudioSource seSource;

    [Header("BGM")]
    public AudioClip titleBGM;
    public AudioClip mazeBGM;
    public AudioClip clearBGM;
    public AudioClip gameOverBGM;

    [Header("SE")]
    public AudioClip buttonHoverSE;
    public AudioClip buttonClickSE;
    public AudioClip itemGetSE;
    public AudioClip goalSE;
    public AudioClip gameOverSE;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// BGMを再生する。
    /// </summary>
    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource == null || clip == null)
            return;

        // 同じBGMなら再生し直さない
        if (bgmSource.clip == clip && bgmSource.isPlaying)
            return;

        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    /// <summary>
    /// BGMを停止する。
    /// </summary>
    public void StopBGM()
    {
        if (bgmSource == null)
            return;

        bgmSource.Stop();
    }

    /// <summary>
    /// SEを再生する。
    /// </summary>
    public void PlaySE(AudioClip clip)
    {
        if (seSource == null || clip == null)
            return;

        seSource.PlayOneShot(clip);
    }

    // -------------------------
    // BGM
    // -------------------------

    public void PlayTitleBGM()
    {
        PlayBGM(titleBGM);
    }

    public void PlayMazeBGM()
    {
        PlayBGM(mazeBGM);
    }

    public void PlayClearBGM()
    {
        PlayBGM(clearBGM);
    }

    public void PlayGameOverBGM()
    {
        PlayBGM(gameOverBGM);
    }

    // -------------------------
    // SE
    // -------------------------

    public void PlayButtonHover()
    {
        PlaySE(buttonHoverSE);
    }

    public void PlayButtonClick()
    {
        PlaySE(buttonClickSE);
    }

    public void PlayItemGet()
    {
        PlaySE(itemGetSE);
    }

    public void PlayGoal()
    {
        PlaySE(goalSE);
    }

    public void PlayGameOver()
    {
        PlaySE(gameOverSE);
    }
}