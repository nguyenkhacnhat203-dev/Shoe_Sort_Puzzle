using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Audio Sources")]
    public AudioSource backgroundMusicSource;
    public AudioSource soundEffectSource;

    [Header("Clips")]
    public AudioClip backgroundMusicClip;
    public List<AudioClip> soundEffectsClips;

    private void Start()
    {
        PlayBackgroundMusic();
    }

    #region Play

    public void PlayBackgroundMusic()
    {
        if (backgroundMusicClip == null) return;

        backgroundMusicSource.clip = backgroundMusicClip;
        backgroundMusicSource.loop = true;

        if (!backgroundMusicSource.isPlaying)
            backgroundMusicSource.Play();
    }

    public void PlaySoundEffect(int index)
    {
        if (index < 0 || index >= soundEffectsClips.Count) return;

        soundEffectSource.PlayOneShot(soundEffectsClips[index]);
    }

    public void BtnClick() => PlaySoundEffect(0);
    public void Match() => PlaySoundEffect(1);
    public void GameWin() => PlaySoundEffect(2);
    public void GameOver() => PlaySoundEffect(3);
    public void Move() => PlaySoundEffect(4);

    #endregion
    public void AdjustBackgroundMusicVolume(float volume)
    {
        backgroundMusicSource.volume = volume;
    }

    public void AdjustSoundEffectsVolume(float volume)
    {
        soundEffectSource.volume = volume;
    }
}