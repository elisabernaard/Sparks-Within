using UnityEngine;
using UnityEngine.InputSystem;

public class ProximityTrigger : MonoBehaviour
{
    public Transform leftHand;
    public Transform rightHand;
    public Transform playerCamera;
    public float handTouchThreshold = 0.05f;
    public float cubeAlignmentThreshold = 0.95f;

    private bool triggered = false;
    private GameObject lastTriggeredTarget = null; // 👈 추가!
    private CameraViewChanger cameraViewChanger;
    private LookController lookController;

    private PlayerControls controls;


    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.ClapSimulator.Enable();
    }

    private void OnDisable()
    {
        controls.ClapSimulator.Disable();
    }

    void Start()
    {
        cameraViewChanger = FindFirstObjectByType<CameraViewChanger>();
        lookController = FindFirstObjectByType<LookController>();

        if (cameraViewChanger == null)
            Debug.LogWarning("⚠️ CameraViewChanger component not found in scene!");
        if (lookController == null)
            Debug.LogWarning("⚠️ LookController not found!");
    }

    void Update()
    {
        if (leftHand == null || rightHand == null || lookController == null) return;

        float handDistance = Vector3.Distance(leftHand.position, rightHand.position);

        // simulate the clap
        if (controls.ClapSimulator.Clap.triggered /* clap simulator is pressed */)
        {
            handDistance = 0.0f; // 👈 클랩 시뮬레이션을 위해 거리 조정
        }

        if (handDistance >= handTouchThreshold) return;

        GameObject target = lookController.currentLookTarget;

        if (target != lastTriggeredTarget)
        {
            triggered = false; // 👈 새로운 큐브일 경우 다시 트리거 가능
        }

        if (!triggered && target == gameObject)
        {
            TriggerEffect();
            triggered = true;
            lastTriggeredTarget = target; // 👈 마지막으로 트리거한 큐브 저장
        }
    }



    void TriggerEffect()
    {
        var profile = GetComponent<SoundProfile>();
        if (profile == null)
        {
            Debug.LogWarning("⚠️ SoundProfile not found on cube.");
            return;
        }

        // 🔄 Skybox는 항상 바꿈
        var skyboxChanger = FindFirstObjectByType<SkyboxColorChanger>();
        if (skyboxChanger != null && skyboxChanger.skyboxMaterial != null)
        {
            skyboxChanger.targetTopColor = profile.topColor;
            skyboxChanger.targetBottomColor = profile.bottomColor;
            skyboxChanger.ChangeSkyboxColor();
            Debug.Log($"🌈 Skybox changed due to interaction with: {profile.beingName}");
        }

        // 🏢 빌딩 색상도 변경
        var buildingChanger = FindFirstObjectByType<BuildingColorChanger>();
        if (buildingChanger != null && buildingChanger.buildingMaterial != null)
        {
            buildingChanger.targetTopColor = profile.topColor;
            buildingChanger.targetBottomColor = profile.bottomColor;
            buildingChanger.ChangeBuildingColor();
            Debug.Log($"🏢 Building color changed due to interaction with: {profile.beingName}");
        }

        // 🌍 월드 효과 트리거
        var worldEffect = FindFirstObjectByType<WorldChangeEffect>();
        if (worldEffect != null)
        {
            worldEffect.StartReveal(
                transform.position,
                profile.topColor,
                profile.bottomColor
            );
        }

        // 👀 카메라가 큐브로 즉시 이동
        if (cameraViewChanger != null)
        {
            cameraViewChanger.MoveToCube(gameObject);
            Debug.Log($"👀 Camera instantly moved to cube: {profile.beingName}");
        }

        // ❌ 이미 수집된 경우 소리나 등록은 안함
        if (SoundMemoryManager.Instance != null)
        {
            if (SoundMemoryManager.Instance.HasBeenCollected(profile.beingName))
            {
                Debug.Log($"🔁 {profile.beingName} already collected. Skipping sound.");
                return;
            }

            Debug.Log($"🎵 Playing sound: {profile.beingName}");
            SoundMemoryManager.Instance.AddSound(profile);
            UpdateMaterialEffect();
        }
    }

    // void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         var profile = GetComponent<SoundProfile>();
    //         if (profile == null)
    //         {
    //             Debug.LogWarning("⚠️ SoundProfile not found on cube.");
    //             return;
    //         }

    //         // 🔄 Skybox는 항상 바꿈
    //         var skyboxChanger = FindFirstObjectByType<SkyboxColorChanger>();
    //         if (skyboxChanger != null && skyboxChanger.skyboxMaterial != null)
    //         {
    //             skyboxChanger.targetTopColor = profile.topColor;
    //             skyboxChanger.targetBottomColor = profile.bottomColor;
    //             skyboxChanger.ChangeSkyboxColor();
    //             Debug.Log($"🌈 Skybox changed due to interaction with: {profile.beingName}");
    //         }

    //         // 🏢 빌딩 색상도 변경
    //         var buildingChanger = FindFirstObjectByType<BuildingColorChanger>();
    //         if (buildingChanger != null && buildingChanger.buildingMaterial != null)
    //         {
    //             buildingChanger.targetTopColor = profile.topColor;
    //             buildingChanger.targetBottomColor = profile.bottomColor;
    //             buildingChanger.ChangeBuildingColor();
    //             Debug.Log($"🏢 Building color changed due to interaction with: {profile.beingName}");
    //         }

    //         // ❌ 이미 수집된 경우 소리나 등록은 안함
    //         if (SoundMemoryManager.Instance != null)
    //         {
    //             if (SoundMemoryManager.Instance.HasBeenCollected(profile.beingName))
    //             {
    //                 Debug.Log($"🔁 {profile.beingName} already collected. Skipping sound.");
    //                 return;
    //             }

    //             Debug.Log($"🎵 Playing sound: {profile.beingName}");
    //             SoundMemoryManager.Instance.AddSound(profile);
    //             UpdateMaterialEffect();
    //         }
    //         else
    //         {
    //             Debug.LogError("❌ SoundMemoryManager.Instance is NULL!");
    //         }
    //     }
    // }

    private void UpdateMaterialEffect()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend == null || rend.material == null) return;

        Material mat = rend.material;

        if (mat.HasProperty("_PrimaryColor"))
        {
            mat.SetColor("_PrimaryColor", Color.black);
        }

        if (mat.HasProperty("_SecondaryColor"))
        {
            mat.SetColor("_SecondaryColor", Color.black);
        }
    }
}
