using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject worldSource;
    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    public static SoundManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void PlaySFX(AudioData audioData)
    {
        if (audioData == null || sfxSource == null)
            return;

        AudioClip clip = audioData.GetRandomClip();

        if (clip == null)
            return;

        sfxSource.PlayOneShot(clip, audioData.Volume);
    }

    public void PlaySFX(AudioData audioData, AudioSource source)
    {
        if (audioData == null || source == null)
            return;

        AudioClip clip = audioData.GetRandomClip();

        if (clip == null)
            return;

        source.PlayOneShot(clip, audioData.Volume);
    }

    public void PlaySFX(AudioData audioData, Vector3 position)
    {
        if (audioData == null)
            return;

        AudioClip clip = audioData.GetRandomClip();

        if (clip == null)
            return;

        GameObject sourceObj = ObjectPoolManager.Instance.Spawn(worldSource, position, Quaternion.identity);
        sourceObj.transform.SetParent(transform);
        WorldAudio source = sourceObj.GetComponent<WorldAudio>();
        source.Play(clip, audioData.Volume);
    }

    public void PlayBGM(AudioData audioData)
    {
        if (audioData == null || bgmSource == null)
            return;

        AudioClip clip = audioData.GetRandomClip();

        if (clip == null)
            return;

        if (bgmSource.clip == clip && bgmSource.isPlaying)
            return;

        bgmSource.clip = clip;
        bgmSource.volume = audioData.Volume;
        bgmSource.pitch = audioData.Pitch;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource == null)
            return;

        bgmSource.Stop();
        bgmSource.clip = null;
    }
}