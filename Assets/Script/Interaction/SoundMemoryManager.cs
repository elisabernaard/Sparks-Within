using System.Collections.Generic;
using UnityEngine;

public class SoundMemoryManager : MonoBehaviour
{
    public static SoundMemoryManager Instance;
    private List<string> collectedNames = new();
    private List<AudioSource> activeSources = new();
    public AudioClip teleportSfx;
    private AudioSource sfxSource;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (sfxSource == null)
        {
            GameObject sfxObj = new GameObject("TeleportSFXSource");
            sfxObj.transform.SetParent(transform);
            sfxSource = sfxObj.AddComponent<AudioSource>();
            sfxSource.spatialBlend = 0f; // 2D 사운드
            sfxSource.playOnAwake = false;
        }
    }

    public void AddSound(SoundProfile profile)
    {
        if (profile == null || profile.soundClip == null)
        {
            Debug.LogWarning("⚠️ SoundProfile or soundClip is null — skipping");
            return;
        }

        // if (collectedNames.Contains(profile.beingName)) return;

        collectedNames.Add(profile.beingName);

        GameObject go = new GameObject("Sound_" + profile.beingName);
        go.transform.parent = transform;

        AudioSource src = go.AddComponent<AudioSource>();
        src.clip = profile.soundClip;
        src.loop = true;
        src.volume = 0.15f;
        src.spatialBlend = 0f;

        if (src.clip.loadState == AudioDataLoadState.Loaded)
        {
            src.Play();
            Debug.Log("✅ Playing sound: " + profile.beingName);
        }
        else
        {
            Debug.LogWarning("⚠️ Clip not fully loaded: " + profile.beingName);
        }

        activeSources.Add(src);
    }

    public void RemoveSound(string beingName)
    {
        if (collectedNames.Contains(beingName))
        {
            collectedNames.Remove(beingName);
            Debug.Log($"🧹 Sound profile for {beingName} removed from memory.");

            // ❗ 재생 중인 AudioSource 제거
            var source = activeSources.Find(src => src != null && src.gameObject.name == "Sound_" + beingName);
            if (source != null)
            {
                source.Stop();
                activeSources.Remove(source);
                Destroy(source.gameObject);
            }
        }
    }


    public bool HasBeenCollected(string beingName)
    {
        return collectedNames.Contains(beingName); // beingName은 SoundProfile.beingName
    }

    public void PlayTeleportSfx()
    {
        if (teleportSfx != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(teleportSfx, 1f);
        }
    }
}
