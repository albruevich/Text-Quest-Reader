using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSourceA;
    [SerializeField] private AudioSource musicSourceB;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private float musicFadeDuration = 0.5f;
    [SerializeField] private AudioClip clickClip;
    [SerializeField] private AudioClip hoverClip;

    public static AudioManager Instance;

    private Coroutine musicCoroutine;
    private string currentMusicName;
    private float defaultMusicVolume;

    private AudioSource activeMusicSource;
    private AudioSource inactiveMusicSource;
 
    public string CurrentMusicName => currentMusicName;

    private void Awake()
    {
        Instance = this;

        activeMusicSource = musicSourceA;
        inactiveMusicSource = musicSourceB;

        defaultMusicVolume = musicSourceA.volume;

        musicSourceA.playOnAwake = false;
        musicSourceB.playOnAwake = false;
        sfxSource.playOnAwake = false;

        musicSourceA.loop = true;
        musicSourceB.loop = true;

        musicSourceA.volume = 0f;
        musicSourceB.volume = 0f;
    }

    public void PlayMusic(string musicName, string questName, bool stoppable)
    {
        if (string.IsNullOrEmpty(musicName))
        {
            if(stoppable)
                StopMusic();

            return;
        }

        if (activeMusicSource.isPlaying && currentMusicName == musicName)
            return;      

        if (musicCoroutine != null)
            StopCoroutine(musicCoroutine);

        musicCoroutine = StartCoroutine(CrossfadeToMusic(musicName, questName));
    }

    private IEnumerator CrossfadeToMusic(string musicName, string questName)
    {
        AudioClip newClip = LoadMusicClip(musicName, questName);

        if (newClip == null)
        {
            musicCoroutine = null;
            yield break;
        }

        if (activeMusicSource.isPlaying && activeMusicSource.clip == newClip && currentMusicName == musicName)
        {
            musicCoroutine = null;
            yield break;
        }

        inactiveMusicSource.Stop();
        inactiveMusicSource.clip = newClip;
        inactiveMusicSource.loop = true;
        inactiveMusicSource.volume = 0f;
        inactiveMusicSource.Play();

        float fromActive = activeMusicSource.isPlaying ? activeMusicSource.volume : 0f;

        if (musicFadeDuration <= 0f)
        {
            if (activeMusicSource.isPlaying)
            {
                activeMusicSource.Stop();
                activeMusicSource.clip = null;
                activeMusicSource.volume = 0f;
            }

            inactiveMusicSource.volume = defaultMusicVolume;
        }
        else
        {
            float time = 0f;

            while (time < musicFadeDuration)
            {
                time += Time.deltaTime;
                float t = Mathf.Clamp01(time / musicFadeDuration);

                if (activeMusicSource.isPlaying)
                    activeMusicSource.volume = Mathf.Lerp(fromActive, 0f, t);

                inactiveMusicSource.volume = Mathf.Lerp(0f, defaultMusicVolume, t);

                yield return null;
            }

            if (activeMusicSource.isPlaying)
            {
                activeMusicSource.Stop();
                activeMusicSource.clip = null;
                activeMusicSource.volume = 0f;
            }

            inactiveMusicSource.volume = defaultMusicVolume;
        }

        SwapMusicSources();
        currentMusicName = musicName;
        musicCoroutine = null;
    }

    public void StopMusic()
    {
        if (musicCoroutine != null)
            StopCoroutine(musicCoroutine);

        musicCoroutine = StartCoroutine(StopMusicSmooth());
    }

    private IEnumerator StopMusicSmooth()
    {
        AudioSource sourceToStop = activeMusicSource;

        if (sourceToStop.isPlaying && sourceToStop.volume > 0f)
            yield return FadeVolume(sourceToStop, sourceToStop.volume, 0f, musicFadeDuration);

        sourceToStop.Stop();
        sourceToStop.clip = null;
        sourceToStop.volume = 0f;

        inactiveMusicSource.Stop();
        inactiveMusicSource.clip = null;
        inactiveMusicSource.volume = 0f;

        currentMusicName = null;
        musicCoroutine = null;
    }

    private void SwapMusicSources()
    {
        AudioSource temp = activeMusicSource;
        activeMusicSource = inactiveMusicSource;
        inactiveMusicSource = temp;
    }

    public void PlaySfx(string sfxName, string questName)
    {
        if (string.IsNullOrEmpty(sfxName))
            return;

        AudioClip clip = LoadSfxClip(sfxName, questName);

        if (clip == null)
            return;

        sfxSource.PlayOneShot(clip);
    }

    public void PlaySfx(SoundType soundType)
    {
        switch(soundType)
        {
            case SoundType.Click: sfxSource.PlayOneShot(clickClip); break;
            case SoundType.Hover: sfxSource.PlayOneShot(hoverClip); break;
            default: break;
        }
    }

    public void StopSfx()
    {
        sfxSource.Stop();
    }

    private AudioClip LoadMusicClip(string musicName, string questName)
    {
        string basePath = $"Quests/{questName}/Musics/{musicName}";
        AudioClip clip = Resources.Load<AudioClip>(basePath);

        if (clip == null)
            Debug.LogWarning($"Music not found in Resources: {basePath}");

        return clip;
    }

    private AudioClip LoadSfxClip(string sfxName, string questName)
    {
        string basePath = $"Quests/{questName}/Sounds/{sfxName}";
        AudioClip clip = Resources.Load<AudioClip>(basePath);

        if (clip == null)
            Debug.LogWarning($"Sound not found in Resources: {basePath}");

        return clip;
    }

    private IEnumerator FadeVolume(AudioSource source, float from, float to, float duration)
    {
        if (duration <= 0f)
        {
            source.volume = to;
            yield break;
        }

        float time = 0f;
        source.volume = from;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            source.volume = Mathf.Lerp(from, to, t);
            yield return null;
        }

        source.volume = to;
    }
}

public enum SoundType
{
    None,
    Click,
    Hover
}