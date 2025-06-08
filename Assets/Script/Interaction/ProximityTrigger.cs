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
            Debug.LogWarning("⚠️ CameraViewChanger component not found in scene!");
        if (lookController == null)
            Debug.LogWarning("⚠️ LookController not found!");
        if (soundManager == null)
            Debug.LogWarning("⚠️ SoundMemoryManager not found!");
        if (profile == null)
            Debug.LogWarning("⚠️ SoundProfile not found on cube!");
    }

    void Update()
    {
        if (leftHand == null || rightHand == null || lookController == null || profile == null)
            return;

        float handDistance = Vector3.Distance(leftHand.position, rightHand.position);

        // simulate clap
        if (controls.ClapSimulator.Clap.triggered)
            handDistance = 0f;

        if (handDistance >= handTouchThreshold)
            return;

        GameObject target = lookController.currentLookTarget;

        if (target != lastTriggeredTarget)
        {
            triggered = false;
        }

        if (!triggered && target == gameObject)
        {
            TriggerEffect();
            triggered = true;
            lastTriggeredTarget = target;
        }
    }

    void TriggerEffect()
    {
        if (soundManager.HasBeenCollected(profile.beingName))
        {
            soundManager.RemoveSound(profile.beingName);
            RestoreMaterialColor();
            Debug.Log($"🧹 Removed collected sound: {profile.beingName}");
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
