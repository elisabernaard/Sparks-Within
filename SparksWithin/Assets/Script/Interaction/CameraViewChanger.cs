using UnityEngine;

public class CameraViewChanger : MonoBehaviour
{
    public Transform xrRig;
    public Transform playerCamera;
    private Transform targetCube;
    private Vector3 offset;
    private bool isFollowing = false;
    private SoundMemoryManager soundMemoryManager;

    private GameObject lastFollowedCube; // 👈 마지막으로 따라간 큐브 저장

    void Start()
    {
        if (xrRig == null)
            xrRig = GameObject.Find("XR Origin")?.transform;

        if (playerCamera == null)
            playerCamera = Camera.main.transform;

        soundMemoryManager = SoundMemoryManager.Instance;
    }


    void LateUpdate()
    {
        if (isFollowing && targetCube != null)
        {
            xrRig.position = targetCube.position + offset;
        }
    }

    public void MoveToCube(GameObject cube)
    {
        // ✋ 이미 수집한 큐브는 무시
        var profile = cube.GetComponent<SoundProfile>();
        if (soundMemoryManager != null && profile != null && soundMemoryManager.HasBeenCollected(profile.beingName))
        {
            Debug.Log($"🚫 {profile.beingName} is already collected. Camera will not follow.");
            return;
        }

        if (cube != null && xrRig != null)
        {
            // ✅ 이전 큐브 다시 움직이게 함
            if (lastFollowedCube != null)
            {
                var prevFollower = lastFollowedCube.GetComponent<BirdPathFollower>();
                if (prevFollower != null)
                    prevFollower.enabled = true;
            }

            // ✅ 새 큐브 움직임 멈춤
            var follower = cube.GetComponent<BirdPathFollower>();
            if (follower != null)
                follower.enabled = false;

            lastFollowedCube = cube; // 현재 큐브 저장
            targetCube = cube.transform;
            offset = xrRig.position - playerCamera.position;
            offset.y = 0;

            xrRig.position = targetCube.position + offset;
            isFollowing = true;

            Debug.Log($"✅ Following {cube.name}, movement disabled.");
        }
    }

    public void StopFollowing()
    {
        isFollowing = false;
        targetCube = null;
        Debug.Log("✅ Stopped following cube");
    }
}
