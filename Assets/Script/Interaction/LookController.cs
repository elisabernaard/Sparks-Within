using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class LookController : MonoBehaviour
{
    public Transform playerCamera;
    public List<GameObject> cubes;
    public float lookAcquireThreshold = 0.98f;
    public float lookReleaseThreshold = 0.96f;
    public float minVolume = 0.05f;
    public float maxVolume = 1.5f;
    public float focusTimeToMax = 3.0f;
    public float spatialBlend = 0f;
    public float maxDistance = 100f;

    public GameObject markerPrefab;
    public LayerMask obstacleLayerMask;

    public AudioMixerGroup mixerGroup; // 🎯 Unity에서 할당 필요
    private Material markerMaterial;

    private GameObject currentMarker;
    public GameObject currentLookTarget { get; private set; }

    private Dictionary<GameObject, AudioSource> audioSources = new();
    private Dictionary<GameObject, SoundProfile> profiles = new();

    private GameObject previousLookTarget = null;
    private float lookDuration = 0f;

    private float raycastCooldown = 0.1f;
    private float timeSinceLastRaycast = 0f;

    void Start()
    {
        foreach (var cube in cubes)
        {
            var audio = cube.GetComponent<AudioSource>();
            var profile = cube.GetComponent<SoundProfile>();

            if (audio != null)
            {
                audio.spatialBlend = spatialBlend;
                audio.maxDistance = maxDistance;
                audio.rolloffMode = AudioRolloffMode.Linear;
                audio.volume = minVolume;

                if (mixerGroup != null)
                    audio.outputAudioMixerGroup = mixerGroup;

                audioSources[cube] = audio;
            }

            if (profile != null)
            {
                profiles[cube] = profile;
            }
        }

        if (markerPrefab != null)
        {
            currentMarker = Instantiate(markerPrefab);
            currentMarker.SetActive(false);
            Renderer markerRenderer = currentMarker.GetComponentInChildren<Renderer>();
            if (markerRenderer != null)
                markerMaterial = markerRenderer.material;
        }
    }

    void Update()
    {
        timeSinceLastRaycast += Time.deltaTime;
        if (timeSinceLastRaycast >= raycastCooldown)
        {
            timeSinceLastRaycast = 0f;
            UpdateLookDetection();
        }

        UpdateAudioVolume();
    }

    void UpdateLookDetection()
    {
        GameObject bestCandidate = null;
        float bestDot = -1f;

        foreach (var cube in cubes)
        {
            if (!audioSources.ContainsKey(cube) || !profiles.ContainsKey(cube)) continue;

            Vector3 dirToCube = (cube.transform.position - playerCamera.position).normalized;
            float dot = Vector3.Dot(playerCamera.forward, dirToCube);

            bool blocked = Physics.Raycast(
                playerCamera.position,
                dirToCube,
                Vector3.Distance(playerCamera.position, cube.transform.position),
                obstacleLayerMask
            );

            bool alreadyCollected = SoundMemoryManager.Instance != null &&
                                    SoundMemoryManager.Instance.HasBeenCollected(profiles[cube].beingName);

            if (!blocked && !alreadyCollected && dot > lookAcquireThreshold && dot > bestDot)
            {
                bestDot = dot;
                bestCandidate = cube;
            }
        }

        GameObject previousTarget = currentLookTarget;

        if (currentLookTarget != null && bestCandidate == null)
        {
            Vector3 dirToCurrent = (currentLookTarget.transform.position - playerCamera.position).normalized;
            float currentDot = Vector3.Dot(playerCamera.forward, dirToCurrent);

            bool stillBlocked = Physics.Raycast(
                playerCamera.position,
                dirToCurrent,
                Vector3.Distance(playerCamera.position, currentLookTarget.transform.position),
                obstacleLayerMask
            );

            if (stillBlocked || currentDot < lookReleaseThreshold)
            {
                currentLookTarget = null;
            }
        }
        else
        {
            currentLookTarget = bestCandidate;
        }

        if (currentLookTarget != previousTarget)
        {
            lookDuration = 0f;
            previousLookTarget = currentLookTarget;
            UpdateMarker();
        }
    }

    void UpdateAudioVolume()
    {
        foreach (var cube in cubes)
        {
            AudioSource audio = audioSources[cube];
            if (cube == currentLookTarget)
            {
                lookDuration += Time.deltaTime;
                float t = Mathf.Clamp01(lookDuration / focusTimeToMax);
                float linearVolume = Mathf.Lerp(minVolume, maxVolume, t);

                // AudioSource volume 최대 1.0까지만 사용
                audio.volume = Mathf.Min(1f, linearVolume);

                if (!audio.isPlaying)
                    audio.Play();

                // Mixer를 통한 추가 볼륨 조절 (dB로)
                if (mixerGroup != null)
                {
                    float dB = Mathf.Log10(Mathf.Clamp(linearVolume, 0.01f, 1.5f)) * 20f; // 예: 0.3 → -10dB, 1.5 → +3.5dB
                    mixerGroup.audioMixer.SetFloat("Volume", dB);
                }

                // 마커 밝기 연동 (보일 때만)
                if (currentMarker != null && currentMarker.activeSelf && markerMaterial != null)
                {
                    float brightness = Mathf.InverseLerp(minVolume, maxVolume, linearVolume);
                    markerMaterial.SetFloat("_FresnelPower", Mathf.Lerp(1.5f, 4.5f, brightness));
                    markerMaterial.SetColor("_FresnelColor", Color.yellow * Mathf.Lerp(0.3f, 2.5f, brightness));

                    // Debug.Log($"🎯 {cube.name} | 🔊 Volume: {linearVolume:F2}, 💡 Brightness: {brightness:F2}");
                }
            }
            else
            {
                if (audio.isPlaying)
                    audio.Stop();
            }
        }
    }

    void UpdateMarker()
    {
        if (currentMarker == null) return;

        if (currentLookTarget != null)
        {
            if (!currentMarker.activeSelf)
                currentMarker.SetActive(true);

            if (currentMarker.transform.parent != currentLookTarget.transform)
                currentMarker.transform.SetParent(currentLookTarget.transform);

            currentMarker.transform.localPosition = Vector3.zero;
            currentMarker.transform.localRotation = Quaternion.identity;
        }
        else
        {
            if (currentMarker.activeSelf)
                currentMarker.SetActive(false);

            if (currentMarker.transform.parent != null)
                currentMarker.transform.SetParent(null);
        }
    }
}
