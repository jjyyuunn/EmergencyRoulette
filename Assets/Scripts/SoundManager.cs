using UnityEngine;
using UnityEngine.Rendering;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("bgm")]
    [SerializeField] private AudioClip bgmFirst;
    [SerializeField] private AudioClip bgmLoop;

    [Header("sfx")]
    [SerializeField] private AudioClip slotButtonClickSound;
    [SerializeField] private AudioClip slotSpinningSound;
    [SerializeField] private AudioClip slotResourceGainSound;
    [SerializeField] private AudioClip shopPurchaseSound;


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

    private void Start()
    {
        PlayIntroThenLoop();
    }

    public void PlayIntroThenLoop()
    {
        if (bgmSource == null || bgmFirst == null || bgmLoop == null)
        {
            Debug.LogError("BGM Source, Intro Clip, or Loop Clip is not assigned.");
            return;
        }

        bgmSource.clip = bgmFirst;
        bgmSource.loop = false; // 첫 클립은 루프하지 않음
        bgmSource.Play();

        // 첫 클립이 끝난 후 루프 클립 재생
        Invoke(nameof(PlayLoopClip), bgmFirst.length);
    }

    private void PlayLoopClip()
    {
        bgmSource.clip = bgmLoop;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }
    }

    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = Mathf.Clamp01(volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp01(volume);
    }

    public float GetBGMVolume()
    {
        return bgmSource != null ? bgmSource.volume : 1.0f;
    }

    public float GetSFXVolume()
    {
        return sfxSource != null ? sfxSource.volume : 1.0f;
    }

    // SFX 플레이 메서드들
    public void PlaySlotButtonClickSound()
    {
        PlaySFX(slotButtonClickSound);
    }

    public void PlaySlotSpinningSound()
    {
        PlaySFX(slotSpinningSound);
    }

    public void PlaySlotResourceGainSound()
    {
        PlaySFX(slotResourceGainSound);
    }

    public void PlayShopPurchaseSound()
    {
        PlaySFX(shopPurchaseSound);
    }


    // 공통 SFX 재생 메서드
    private void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null)
        {
            Debug.LogError("SFX Source or Audio Clip is not assigned.");
            return;
        }

        sfxSource.PlayOneShot(clip); // OneShot으로 효과음 재생
    }
}
