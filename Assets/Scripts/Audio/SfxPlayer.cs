using System.Collections.Generic;
using UnityEngine;

public class SfxPlayer : MonoBehaviour
{
    public static SfxPlayer Instance { get; private set; }
    public static Dictionary<string, AudioClip> AudioClips { get; private set; } = new();

    [SerializeField] private AudioSource template;
    [SerializeField] private AudioClip[] clips;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        AudioClips.Clear();

        foreach (var clip in clips)
        {
            AudioClips.Add(clip.name.Trim().ToLowerInvariant(), clip);
        }

        DontDestroyOnLoad(gameObject);
    }

    public static AudioClip ClipFromName(string name)
    {
        if (AudioClips.TryGetValue(name.ToLowerInvariant(), out var clip))
        {
            return clip;
        }

        return null;
    }

    public static void PlaySfx(string name)
    {
        PlaySfx(ClipFromName(name), Random.Range(0.8f, 1.2f));
    }

    public static void PlaySfx(AudioClip clip)
    {
        PlaySfx(clip, Random.Range(0.8f, 1.2f));
    }

    public static void PlaySfx(string name, float pitch)
    {
        PlaySfx(ClipFromName(name), pitch);
    }

    public static void PlaySfx(AudioClip clip, float pitch)
    {
        Instance.PlaySfxLocal(clip, pitch);
    }

    private void PlaySfxLocal(AudioClip clip, float pitch)
    {
        if (clip == null) return;

        AudioSource audio = Instantiate(template, transform);

        audio.clip = clip;

        audio.pitch = pitch;

        audio.Play();

        Destroy(audio.gameObject, clip.length / pitch + 1f);
    }
}
