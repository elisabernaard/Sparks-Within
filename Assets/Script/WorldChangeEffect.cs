using UnityEngine;


public class WorldChangeEffect : MonoBehaviour
{
    public Renderer landscapeRenderer;
    public float revealSpeed = 700f;
    public float maxRadius = 3000f;

    private float currentRadius = 0f;
    private bool isRevealing = false;
    private Material landscapeMaterial;

    private Color prevTopColor;
    private Color prevBottomColor;

    [Header("Default Gradient Colors")]
    public Color defaultTopColor;
    public Color defaultBottomColor;

    void Start()
    {
        if (landscapeRenderer != null) {
            landscapeMaterial = landscapeRenderer.material;

            // ✅ 기본 그라디언트 텍스처 생성 및 적용
            defaultTopColor = new Color(0.8f, 0.3f, 0.8f, 1f);
            defaultBottomColor = new Color(0f, 0f, 1f, 1f);
            Texture2D defaultTex = CreateGradientTexture(defaultBottomColor, defaultTopColor);
            landscapeMaterial.SetTexture("_MainTexture", defaultTex);
            landscapeMaterial.SetTexture("_SecondaryTexture", defaultTex);
            Debug.Log($"🌍 Default texture applied: {defaultTex}, {defaultTopColor}, {defaultBottomColor}");

            // 기본 색을 이전 값으로 저장해 둠
            prevTopColor = defaultTopColor;
            prevBottomColor = defaultBottomColor;
        }
    }

    void Update()
    {
        if (!isRevealing) return;

        currentRadius += Time.deltaTime * revealSpeed * 3f;
        Shader.SetGlobalFloat("_Radius", currentRadius);

        if (currentRadius >= maxRadius)
            isRevealing = false;
    }

    private Texture2D CreateGradientTexture(Color bottom, Color top, int height = 128)
    {
        Texture2D tex = new Texture2D(1, height, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        for (int y = 0; y < height; y++)
        {
            float t = (float)y / (height - 1);
            tex.SetPixel(0, y, Color.Lerp(bottom, top, t));
        }
        tex.Apply();
        return tex;
    }

    public void StartReveal(Vector3 position, Color newTopColor, Color newBottomColor)
    {
        currentRadius = 0f;
        isRevealing = true;

        Shader.SetGlobalVector("_Position", position);
        Shader.SetGlobalFloat("_Radius", currentRadius);

        Texture2D fromTex = CreateGradientTexture(prevBottomColor, prevTopColor);
        Texture2D toTex = CreateGradientTexture(newBottomColor, newTopColor);

        if (landscapeMaterial != null)
        {
            landscapeMaterial.SetTexture("_MainTexture", fromTex);
            landscapeMaterial.SetTexture("_SecondaryTexture", toTex);
        }

        prevTopColor = newTopColor;
        prevBottomColor = newBottomColor;
    }
}
