using UnityEngine;
using System.Collections.Generic;

namespace Vozon.Audio
{
    public class AudioSystem : MonoBehaviour
    {
        private static AudioSystem instance;
        public static AudioSystem Instance => instance;

        private Dictionary<string, AudioClip> audioClips;
        private Dictionary<string, AudioSource> audioSources;
        private AudioSource musicSource;
        private AudioSource sfxSource;
        private float masterVolume = 1f;
        private float musicVolume = 1f;
        private float sfxVolume = 1f;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSystem();
        }

        private void InitializeAudioSystem()
        {
            Debug.Log("Initializing VOZON Audio System...");
            audioClips = new Dictionary<string, AudioClip>();
            audioSources = new Dictionary<string, AudioSource>();
            
            SetupAudioSources();
            LoadDefaultAudioClips();
        }

        private void SetupAudioSources()
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = true;

            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }

        private void LoadDefaultAudioClips()
        {
            // Hier würden Standard-Audioclips geladen werden
            Debug.Log("Loading default audio clips...");
        }

        public void PlayMusic(string clipName, float fadeInDuration = 0f)
        {
            if (audioClips.TryGetValue(clipName, out AudioClip clip))
            {
                musicSource.clip = clip;
                musicSource.volume = musicVolume * masterVolume;
                
                if (fadeInDuration > 0)
                {
                    StartCoroutine(FadeInMusic(fadeInDuration));
                }
                else
                {
                    musicSource.Play();
                }
            }
        }

        public void PlaySFX(string clipName)
        {
            if (audioClips.TryGetValue(clipName, out AudioClip clip))
            {
                sfxSource.PlayOneShot(clip, sfxVolume * masterVolume);
            }
        }

        public void RegisterAudioClip(string name, AudioClip clip)
        {
            if (!audioClips.ContainsKey(name))
            {
                audioClips.Add(name, clip);
                Debug.Log($"Registered audio clip: {name}");
            }
        }

        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }

        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }

        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }

        private void UpdateVolumes()
        {
            if (musicSource != null)
                musicSource.volume = musicVolume * masterVolume;
            
            if (sfxSource != null)
                sfxSource.volume = sfxVolume * masterVolume;
        }

        private System.Collections.IEnumerator FadeInMusic(float duration)
        {
            float startVolume = 0f;
            float targetVolume = musicVolume * masterVolume;
            float currentTime = 0f;

            musicSource.volume = startVolume;
            musicSource.Play();

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                musicSource.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
                yield return null;
            }

            musicSource.volume = targetVolume;
        }
    }
} 