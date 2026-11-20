using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "AudioData/AudioData")]
public class AudioData : ScriptableObject
{
    [Header("Audio")]
    [SerializeField] private AudioClip[] clips;

    [Header("Settings")]
    [SerializeField, Range(0f, 1f)] private float volume = 1f;
    [SerializeField, Range(0.8f, 1.2f)] private float pitch = 1f;

    public AudioClip GetRandomClip()
    {
        if (clips == null || clips.Length == 0)
            return null;

        return clips[Random.Range(0, clips.Length)];
    }

    public float Volume => volume;
    public float Pitch => pitch;
}
