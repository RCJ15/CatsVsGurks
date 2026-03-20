using System.Collections.Generic;
using UnityEngine;

public class SfxPlayer : MonoBehaviour
{
    public static SfxPlayer Instance { get; private set; }
    public static Dictionary<string, AudioClip> AudioClips { get; private set; } = new();
    public static Dictionary<string, List<AudioClip>> AudioClipGroups { get; private set; } = new();

    private static readonly char[] _indexingChars = new char[] { '-', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

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

        foreach (AudioClip clip in clips)
        {
            string name = clip.name.Trim().ToLowerInvariant();
            AudioClips.Add(name, clip);

            bool endsWithNumber = false;

            foreach (char number in _indexingChars)
            {
                if (name.EndsWith(number))
                {
                    endsWithNumber = true;
                    break;
                }
            }

            if (endsWithNumber)
            {
                name = name.TrimEnd(_indexingChars);

                if (AudioClipGroups.TryGetValue(name, out List<AudioClip> groups))
                {
                    groups.Add(clip);
                }
                else
                {
                    groups = new List<AudioClip>() { clip };
                    AudioClipGroups[name] = groups;
                }
            }
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

    public static List<AudioClip> ClipGroupFromName(string name)
    {
        if (AudioClipGroups.TryGetValue(name.ToLowerInvariant(), out var clip))
        {
            return clip;
        }

        return null;
    }

    private static float RandomPitch() => Random.Range(0.8f, 1.2f);

    // Sound from name
    public static void PlaySfx(string name)
    {
        AudioClip clip = ClipFromName(name);

        if (clip == null)
        {
            PlaySfx(ClipGroupFromName(name));
        }
        else
        {
            PlaySfx(clip);
        }
    }

    public static void PlaySfx(string name, Vector3 pos)
    {
        AudioClip clip = ClipFromName(name);

        if (clip == null)
        {
            PlaySfx(ClipGroupFromName(name), pos);
        }
        else
        {
            PlaySfx(clip, pos);
        }
    }

    public static void PlaySfx(string name, float volume)
    {
        AudioClip clip = ClipFromName(name);

        if (clip == null)
        {
            PlaySfx(ClipGroupFromName(name), volume);
        }
        else
        {
            PlaySfx(clip, volume);
        }
    }

    public static void PlaySfx(string name, float volume, float pitch)
    {
        AudioClip clip = ClipFromName(name);

        if (clip == null)
        {
            PlaySfx(ClipGroupFromName(name), volume, pitch);
        }
        else
        {
            PlaySfx(clip, volume, pitch);
        }
    }

    public static void PlaySfx(string name, Vector3 pos, float volume)
    {
        AudioClip clip = ClipFromName(name);

        if (clip == null)
        {
            PlaySfx(ClipGroupFromName(name), pos, volume);
        }
        else
        {
            PlaySfx(clip, pos, volume);
        }
    }

    public static void PlaySfx(string name, Vector3 pos, float volume, float pitch)
    {
        AudioClip clip = ClipFromName(name);

        if (clip == null)
        {
            PlaySfx(ClipGroupFromName(name), pos, volume, pitch);
        }
        else
        {
            PlaySfx(clip, pos, volume, pitch);
        }
    }

    // Sound from clip
    public static void PlaySfx(AudioClip clip)
    {
        PlaySfx(clip, RandomPitch());
    }

    public static void PlaySfx(AudioClip clip, Vector3 pos)
    {
        PlaySfx(clip, pos, RandomPitch());
    }

    public static void PlaySfx(AudioClip clip, float volume)
    {
        Instance.PlaySfxLocal(clip, null, volume, RandomPitch());
    }

    public static void PlaySfx(AudioClip clip, float volume, float pitch)
    {
        Instance.PlaySfxLocal(clip, null, volume, pitch);
    }

    public static void PlaySfx(AudioClip clip, Vector3 pos, float volume)
    {
        Instance.PlaySfxLocal(clip, pos, volume, RandomPitch());
    }

    public static void PlaySfx(AudioClip clip, Vector3 pos, float volume, float pitch)
    {
        Instance.PlaySfxLocal(clip, pos, volume, pitch);
    }

    // Sound from clips (plural)
    public static void PlaySfx(List<AudioClip> clips)
    {
        PlaySfx(clips, RandomPitch());
    }

    public static void PlaySfx(List<AudioClip> clips, Vector3 pos)
    {
        PlaySfx(clips, pos, RandomPitch());
    }

    public static void PlaySfx(List<AudioClip> clips, float volume)
    {
        Instance.PlaySfxLocal(clips, null, volume, RandomPitch());
    }

    public static void PlaySfx(List<AudioClip> clips, float volume, float pitch)
    {
        Instance.PlaySfxLocal(clips, null, volume, pitch);
    }

    public static void PlaySfx(List<AudioClip> clips, Vector3 pos, float volume)
    {
        Instance.PlaySfxLocal(clips, pos, volume, RandomPitch());
    }

    public static void PlaySfx(List<AudioClip> clips, Vector3 pos, float volume, float pitch)
    {
        Instance.PlaySfxLocal(clips, pos, volume, pitch);
    }

    // Local
    private void PlaySfxLocal(List<AudioClip> clips, Vector3? pos, float volume, float pitch)
    {
        if (clips == null) return;

        PlaySfxLocal(clips[Random.Range(0, clips.Count)], pos, volume, pitch);
    }

    private void PlaySfxLocal(AudioClip clip, Vector3? pos, float volume, float pitch)
    {
        if (clip == null) return;

        AudioSource audio = Instantiate(template, transform);

        if (pos.HasValue)
        {
            audio.transform.position = pos.Value;

            Vector3 scale = VisualsPlane.Transform.localScale;
            float size = (scale.x + scale.y + scale.z) / 3;

            audio.minDistance *= size;
            audio.maxDistance *= size;

            audio.spatialBlend = 1;
        }

        audio.clip = clip;

        audio.volume = volume;
        audio.pitch = pitch;

        audio.Play();

        Destroy(audio.gameObject, clip.length / pitch + 1f);
    }
}
