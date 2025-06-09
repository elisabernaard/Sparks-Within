using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.VFX;
using System.Collections;

public class OnboardingManager : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public TextMeshProUGUI onboardingText;
    public GameObject particleEntity;
    public string nextSceneName = "CompleteGame";

    [Header("UI")]
    public Image fadeImage;

    [Header("Hands")]
    public Transform leftHand;
    public Transform rightHand;

    [Header("Animation & Sound")]
    public Animator entityAnimator;
    public string animationTrigger1 = "Trigger1";
    public string animationTrigger2 = "Trigger2";
    public AudioSource entityAudio;
    public AudioSource transitionAudioSource;

    [Header("Settings")]
    public float lookThreshold = 0.96f;
    public float handTouchThreshold = 0.05f;

    private float timer = 0f;
    private bool hasShownFirstText = false;
    private bool hasShownSecondText = false;
    private bool hasTriggeredHands = false;

    void Start()
    {
        particleEntity.SetActive(true);
        if (fadeImage != null)
            fadeImage.color = new Color(0, 0, 0, 0);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (!hasShownFirstText && timer >= 2f)
        {
            onboardingText.text = "You can hear my sound";
            hasShownFirstText = true;
        }

        if (hasShownFirstText && !hasShownSecondText && timer >= 10f)
        {
            onboardingText.text = "Join your hands\nto share your consciousness with me";
            hasShownSecondText = true;
        }

        if (hasShownSecondText && !hasTriggeredHands)
        {
            // TryTriggerHandClap();
        }
    }

    public void OnReformEnd()
    {
        onboardingText.text = "You are ready.\nYour journey begins now.";

        if (transitionAudioSource != null)
            transitionAudioSource.Play();

        StartCoroutine(FadeAndLoadNextScene());
    }

    private IEnumerator FadeAndLoadNextScene()
    {
        float fadeDuration = 2.5f;
        float waitAfterFade = 2.5f;
        float elapsed = 0f;

        Color startColor = new Color(0, 0, 0, 0);
        Color endColor = new Color(0, 0, 0, 1);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            if (fadeImage != null)
                fadeImage.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        yield return new WaitForSeconds(waitAfterFade);
        SceneManager.LoadScene(nextSceneName);
    }

    // ✅ 에러 해결용 메서드
    public void OnDissolveEnd()
    {
        Debug.Log("🌀 OnDissolveEnd called from animation event.");
        // 여기에 필요하다면 후속 로직 추가
    }
}
