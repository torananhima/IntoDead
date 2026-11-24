using UnityEngine;

public class WorldAudio : MonoBehaviour, IPoolable
{
    [SerializeField] private AudioSource source;

    private void Awake()
    {
        if (source == null) source = GetComponent<AudioSource>();
    }

    public void Play(AudioClip clip, float volume = 1)
    {
        if(source == null) source = GetComponent<AudioSource>();
        source.volume = volume;
        source.clip = clip;
        source.Play();
    }

    private void Update()
    {
        if (!source.isPlaying && source.clip != null)
        {
            ObjectPoolManager.Instance.Despawn(gameObject);
        }
    }

    public void OnPoolSpawn()
    {
        if (source == null) source = GetComponent<AudioSource>();
        source.Stop();
        source.clip = null;
    }

    public void OnPoolDespawn()
    {
        if (source == null) source = GetComponent<AudioSource>();
        source.Stop();
        source.clip = null;
    }
}
