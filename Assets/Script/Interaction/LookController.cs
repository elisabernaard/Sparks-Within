using UnityEngine;
using System.Collections.Generic;

public class LookController : MonoBehaviour
{
    public Transform playerCamera;
    public List<GameObject> cubes;
    public float lookThreshold = 0.98f; // 약 10도 이내
    public float defaultVolume = 1.5f; // 기본 볼륨을 1.5배로 설정
    public float spatialBlend = 0f; // 3D 효과 정도 (0: 2D, 1: 3D)
    public float maxDistance = 100f; // 최대 거리

    public GameObject markerPrefab;
    private GameObject currentMarker;
    public GameObject currentLookTarget { get; private set; }

    private Dictionary<GameObject, AudioSource> audioSources = new();
    private Dictionary<GameObject, SoundProfile> profiles = new();
    
    void Start()
    {
        // 모든 큐브의 AudioSource 설정 초기화
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
            AudioSource audio = audioSources.ContainsKey(cube) ? audioSources[cube] : null;
            SoundProfile profile = profiles.ContainsKey(cube) ? profiles[cube] : null;

            Vector3 dirToCube = (cube.transform.position - playerCamera.position).normalized;
            float dot = Vector3.Dot(playerCamera.forward, dirToCube);

            bool alreadyCollected = profile != null &&
                                    SoundMemoryManager.Instance != null &&
                                    SoundMemoryManager.Instance.HasBeenCollected(profile.beingName);

            // 🔍 가장 정면에 가까운 오브젝트만 선택
            if (dot > lookThreshold && dot > bestDot)
            {
                bestDot = dot;
                bestCandidate = cube;
            }
        }

        currentLookTarget = bestCandidate;

        // 🔊 오디오 제어: 하나만 재생, 나머지는 멈춤
        foreach (var cube in cubes)
        {
            AudioSource audio = audioSources.ContainsKey(cube) ? audioSources[cube] : null;
            if (audio == null) continue;

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
                if (audio.isPlaying)
                    audio.Stop();
            }
        }

        UpdateCapsule(currentLookTarget);
    }



    void UpdateCapsule(GameObject previousTarget)
    {
        if (currentMarker == null) return;

        if (currentLookTarget != null)
        {
            currentMarker.SetActive(true);

            // 👇 부모가 달라졌을 때만 변경 (최적화)
            if (currentMarker.transform.parent != currentLookTarget.transform)
            {
                currentMarker.transform.SetParent(currentLookTarget.transform);
            }

            currentMarker.transform.localPosition = Vector3.zero;
            currentMarker.transform.localRotation = Quaternion.identity;
        }
        else
        {
            currentMarker.SetActive(false);

            // 👇 이전에 바라보던 오브젝트와의 관계만 제거
            if (currentMarker.transform.parent != null)
            {
                currentMarker.transform.SetParent(null);
            }
        }
    }


}
