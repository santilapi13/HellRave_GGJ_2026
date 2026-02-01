using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    public Sound[] musicSounds;
    public Sound[] sfxSounds;
    public AudioSource musicSource, sfxSource;
    public int currentMusic = 2;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void PlayMusic(int name) {
        if(currentMusic == name) return;
        
        Sound s = musicSounds[name];
        currentMusic = name;
        musicSource.loop = true;
        musicSource.clip = s.clip;
        musicSource.Play();
    }

    public void PlayMusic(string name) {
        Sound s = Array.Find(musicSounds, x => x.name == name);
        if (currentMusic == Array.IndexOf(musicSounds, s) && musicSource.isPlaying)
            return;
        
        currentMusic = Array.IndexOf(musicSounds, s);
        musicSource.loop = true;
        musicSource.clip = s.clip;
        musicSource.Play();
    }

    public void StopMusic() {
        musicSource.Stop();
    }

    public void PlayIntroAndLoop(int intro, int loop) {
        if (currentMusic == intro && musicSource.isPlaying)
            return;
        
        Sound introSound = musicSounds[intro];
        currentMusic = intro;
        musicSource.loop = false;
        musicSource.clip = introSound.clip;
        musicSource.Play();
        
        StartCoroutine(PlayLoopAfterIntro(introSound.clip.length, loop));
    }

    private IEnumerator PlayLoopAfterIntro(float introLength, int loop) {
        yield return new WaitForSeconds(introLength);
        
        Sound loopSound = musicSounds[loop];
        currentMusic = loop;
        musicSource.loop = true;
        musicSource.clip = loopSound.clip;
        musicSource.Play();
    }

    public void PlaySFX(string name, bool loop) {
        Sound s = Array.Find(sfxSounds, x => x.name == name);
        if (loop) {
            sfxSource.loop = true;
            sfxSource.clip = s.clip;
            sfxSource.Play();
        } else
            sfxSource.PlayOneShot(s.clip);
    }
}