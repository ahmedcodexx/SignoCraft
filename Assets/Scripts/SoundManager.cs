using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource audioSource;   // للبكجراوند ميوزيك + أصوات الصح والغلط

    [Header("Audio Clips")]
    public AudioClip correctSound;
    public AudioClip wrongSound;

    // ========================= Background Music =========================
    public void PlayBackground()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }

    public void StopBackground()
    {
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
    }

    public bool IsPlaying()
    {
        return audioSource != null && audioSource.isPlaying;
    }

    // ========================= Sound Effects =========================
    public void PlayCorrect()
    {
        if (audioSource != null && correctSound != null)
            audioSource.PlayOneShot(correctSound);
    }

    public void PlayWrong()
    {
        if (audioSource != null && wrongSound != null)
            audioSource.PlayOneShot(wrongSound);
    }
}