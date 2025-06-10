using UnityEngine;
using UnityEngine.InputSystem;

public class ProximityTrigger : MonoBehaviour
{
    public Transform leftHand;
    public Transform rightHand;
    public Transform playerCamera;
    public float handTouchThreshold = 0.05f;
    public float cubeAlignmentThreshold = 0.95f;

    public OnboardingManager onboardingManager; // ✅ 수동 연결용 public 필드

    private bool triggered = false;
    private GameObject lastTriggeredTarget = null;

    private SoundMemoryManager soundManager;
    private CameraViewChanger cameraViewChanger;
    private LookController lookController;
    private PlayerControls controls;

    private Renderer cubeRenderer;
    private Material cubeMaterial;
    private SoundProfile profile;

    private void Awake()
    {
        controls = new PlayerControls();
        profile = GetComponent<SoundProfile>();
        cubeRenderer = GetComponent<Renderer>();
        if (cubeRenderer != null) cubeMaterial = cubeRenderer.material;
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
        soundManager = FindFirstObjectByType<SoundMemoryManager>();

        if (cameraViewChanger == null)
            Debug.LogWarning("⚠️ CameraViewChanger not found");
        if (lookController == null)
            Debug.LogWarning("⚠️ LookController not found");
        if (soundManager == null)
            Debug.LogWarning("⚠️ SoundMemoryManager not found");
        if (onboardingManager == null)
            Debug.LogWarning("⚠️ OnboardingManager is not assigned in Inspector!");
        if (profile == null)
            Debug.LogWarning("⚠️ SoundProfile not found on cube");
    }

    void Update()
    {
        if (leftHand == null || rightHand == null || lookController == null || profile == null)
            return;

        bool clapHappened = controls.ClapSimulator.Clap.triggered;

        float handDistance = Vector3.Distance(leftHand.position, rightHand.position);
        if (clapHappened)
        {
            Debug.Log("👏 ClapSimulator triggered!");
            handDistance = 0f;
        }

        if (handDistance >= handTouchThreshold)
        {
            Debug.Log("📏 손이 너무 멀음 — TriggerEffect 중단");
            return;
        }

        GameObject target = lookController.currentLookTarget;
        if (target == null)
            return;

        // Debug.Log(triggered);
        // Debug.Log($"👀 현재 시선 타겟: {target.name}, 이전 타겟: {(lastTriggeredTarget != null ? lastTriggeredTarget.name : "null")}, 현재 오브젝트: {gameObject.name}");

        if (target == gameObject)
        {
            TriggerEffect();
        }
    }


    void TriggerEffect()
    {
        if (soundManager.HasBeenCollected(profile.beingName))
        {
            soundManager.RemoveSound(profile.beingName);
            RestoreMaterialColor();
            triggered = false;
            lastTriggeredTarget = null;
            Debug.Log("🔁 트리거 상태 초기화 완료 (Remove)");
            return;
        }

        if (triggered)
        {
            Debug.Log("⛔ 이미 Trigger됨 — 재실행 차단");
            return;
        }

        ChangeEnvironmentColors();

        if (cameraViewChanger != null)
            cameraViewChanger.MoveToCube(gameObject);

        if (soundManager != null)
        {
            soundManager.PlayTeleportSfx();
            soundManager.AddSound(profile);
        }

        ApplyMaterialEffect();
        triggered = true;
        lastTriggeredTarget = gameObject;
        Debug.Log("✅ Sound 추가 및 상태 갱신 완료");

        if (onboardingManager != null)
        {
            onboardingManager.OnReformEnd();
        }
    }

    void ChangeEnvironmentColors()
    {
        var skyboxChanger = FindFirstObjectByType<SkyboxColorChanger>();
        if (skyboxChanger != null && skyboxChanger.skyboxMaterial != null)
        {
            skyboxChanger.targetTopColor = profile.topColor;
            skyboxChanger.targetBottomColor = profile.bottomColor;
            skyboxChanger.ChangeSkyboxColor();
        }

        var buildingChanger = FindFirstObjectByType<BuildingColorChanger>();
        if (buildingChanger != null && buildingChanger.buildingMaterial != null)
        {
            buildingChanger.targetTopColor = profile.topColor;
            buildingChanger.targetBottomColor = profile.bottomColor;
            buildingChanger.ChangeBuildingColor();
        }

        var worldEffect = FindFirstObjectByType<WorldChangeEffect>();
        if (worldEffect != null)
        {
            worldEffect.StartReveal(
                transform.position,
                profile.topColor,
                profile.bottomColor
            );
        }
    }

    void ApplyMaterialEffect()
    {
        if (cubeMaterial == null) return;

        if (cubeMaterial.HasProperty("_PrimaryColor"))
            cubeMaterial.SetColor("_PrimaryColor", Color.black);
        if (cubeMaterial.HasProperty("_SecondaryColor"))
            cubeMaterial.SetColor("_SecondaryColor", Color.black);
    }

    void RestoreMaterialColor()
    {
        if (cubeMaterial == null || profile == null) return;

        if (cubeMaterial.HasProperty("_PrimaryColor"))
            cubeMaterial.SetColor("_PrimaryColor", profile.topColor);
        if (cubeMaterial.HasProperty("_SecondaryColor"))
            cubeMaterial.SetColor("_SecondaryColor", profile.bottomColor);

        Debug.Log($"🎨 Restored color for {profile.beingName}");
    }
}
