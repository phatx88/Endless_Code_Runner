using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioController : SingletonPersistent<AudioController>
{
    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

    private void Start()
    {
        PlayRandomBackgroundMusic();
    }
    public void PlayMusic(string name, float volume = 0.4f)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound not Found");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.volume = volume;
            musicSource.Play();
        }
    }

    public void PlaySFX(string name, float volume = 1.0f)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound not Found");
        }
        else
        {
            sfxSource.PlayOneShot(s.clip, volume);
        }
    }

    private void PlayRandomBackgroundMusic()
    {
        if (musicSounds.Length > 0)
        {
            int index = UnityEngine.Random.Range(0, musicSounds.Length);
            musicSource.clip = musicSounds[index].clip;
            musicSource.loop = true; // Ensure the music loops
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("No background tracks to play.");
        }
    }
}