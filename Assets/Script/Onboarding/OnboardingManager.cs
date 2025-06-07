using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.VFX;

public class OnboardingManager : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public TextMeshProUGUI onboardingText;
    public GameObject particleEntity;
    public string nextSceneName = "CompleteGame";

    [Header("Hands")]
    public Transform leftHand;
    public Transform rightHand;

    [Header("Animation & Sound")]
    public Animator entityAnimator;
    public string animationTrigger1 = "Trigger1";
    public string animationTrigger2 = "Trigger2";
    public AudioSource entityAudio;

    [Header("Settings")]
    public float lookThreshold = 0.96f;
    public float handTouchThreshold = 0.05f;
    public float cubeAlignmentThreshold = 0.91f;

    private float timer = 0f;
    private bool hasShownFirstText = false;
    private bool hasShownSecondText = false;
    private bool hasTriggeredHands = false;
    private bool hasStartedReform = false;
    private bool finalTextShown = false;
    private float finalTextTimer = 0f;

    void Start()
    {
        particleEntity.SetActive(true);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (!hasShownFirstText && timer >= 2f)
        {
            onboardingText.text = "Look at me";
            hasShownFirstText = true;
        }

        HandleLookAndAudio();

        if (!hasShownSecondText && timer >= 10f)
        {
            onboardingText.text = "Join your hand\nto share your consciousness with me";
            hasShownSecondText = true;
        }

        if (hasShownSecondText && !hasTriggeredHands)
        {
            Debug.Log("👏 TryTriggerHandClap");
            TryTriggerHandClap();
        }

        if (finalTextShown)
        {
            finalTextTimer += Time.deltaTime;
            if (finalTextTimer >= 3f)
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }

    public void OnDissolveEnd()
    {
        Debug.Log("🌀 StartDissolve 애니메이션 끝");
        // entityAnimator.SetTrigger(animationTrigger2); // Reform 시작
        // onboardingText.text = "You are ready. Your journey begins now.";
        // hasStartedReform = true;
    }

    public void OnReformEnd()
    {
        Debug.Log("✨ Reform 애니메이션 끝");
        onboardingText.text = "You are ready.\nYour journey begins now.";
        finalTextShown = true;
        finalTextTimer = 0f;
    }

    void HandleLookAndAudio()
    {
        // 👇 collect 이후에는 항상 재생되게
        if (hasTriggeredHands)
        {
            if (!entityAudio.isPlaying)
                entityAudio.Play();
            return;
        }

        // 👀 collect 전에는 시선에 따라 재생
        Vector3 dirToEntity = (particleEntity.transform.position - playerCamera.transform.position).normalized;
        float dot = Vector3.Dot(playerCamera.transform.forward, dirToEntity);
        VisualEffect vfx = particleEntity.GetComponent<VisualEffect>();

        if (dot > lookThreshold)
        {
            if (!entityAudio.isPlaying)
                entityAudio.Play();

            Debug.Log($"in ParticleSize: {vfx.GetFloat("ParticleSize")}, Turbulence: {vfx.GetFloat("Turbulence")}");
            // vfx.SetFloat("ParticleSize", 0.5f);      // ← VFX Graph에서 이름 확인
            // vfx.SetFloat("Turbulence", 0.1f); // ← float3이면 SetVector3 사용
            entityAnimator.SetTrigger(animationTrigger1);
        }
        else
        {
            if (entityAudio.isPlaying)
                entityAudio.Pause();

            Debug.Log($"out ParticleSize: {vfx.GetFloat("ParticleSize")}, Turbulence: {vfx.GetFloat("Turbulence")}");
            // vfx.SetFloat("ParticleSize", 0.5f);      // ← VFX Graph에서 이름 확인
            // vfx.SetFloat("Turbulence", 0.5f); // 
        }
    }

    void TryTriggerHandClap()
    {
        if (leftHand == null || rightHand == null) return;

        float handDistance = Vector3.Distance(leftHand.position, rightHand.position);
        if (handDistance >= handTouchThreshold) return;

        // 👁️ 바라보고 있는지 확인
        Vector3 dirToEntity = (particleEntity.transform.position - playerCamera.transform.position).normalized;
        float dot = Vector3.Dot(playerCamera.transform.forward, dirToEntity);
        if (dot < lookThreshold) return;

        // ✅ 바라보면서 손만 모으면 성공
        Debug.Log("👏 Hands are clapped while looking at the entity!");
        hasTriggeredHands = true;

        // 👇 Trigger2 애니메이션 (Reform) 실행
        entityAnimator.SetTrigger(animationTrigger2);

        // onboardingText.text = "You shared your consciousness with other entities.";
    }

}
