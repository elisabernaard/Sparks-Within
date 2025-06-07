using UnityEngine;
using System.Collections.Generic;

public class LookController : MonoBehaviour
{
    public Transform playerCamera;
    public List<GameObject> cubes;
    public float lookAcquireThreshold = 0.98f;
    public float lookReleaseThreshold = 0.96f;
    public float defaultVolume = 1.5f;
    public float spatialBlend = 0f;
    public float maxDistance = 100f;

    public GameObject markerPrefab;
    public LayerMask obstacleLayerMask; // 👈 오직 Obstacle만 체크!

    private GameObject currentMarker;
    public GameObject currentLookTarget { get; private set; }

    private Dictionary<GameObject, AudioSource> audioSources = new();
    private Dictionary<GameObject, SoundProfile> profiles = new();

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
        }
    }

    void Update()
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

        // 히스테리시스 적용
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

        // 오디오 제어
        foreach (var cube in cubes)
        {
            AudioSource audio = audioSources[cube];
            if (cube == currentLookTarget)
            {
                if (!audio.isPlaying)
                {
                    audio.volume = defaultVolume;
                    audio.Play();
                }
            }
            else
            {
                if (audio.isPlaying) audio.Stop();
            }
        }

        if (previousTarget != currentLookTarget)
        {
            UpdateMarker();
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
