using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class GrowBodyManager : MonoBehaviour
{
    public float growSpeed = 1f;
    [Range(0f, 1f)] public float minGrow = 0.2f;
    [Range(0f, 1f)] public float maxGrow = 1f;
    public int maxCollectedCount = 20;

    private Material material;
    private float currentGrow = 0f;

    void Start()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            material = renderer.material; // ⚠️ 인스턴스 복사본 (runtime에 변경 가능)
            if (material.HasProperty("_Grow"))
            {
                material.SetFloat("_Grow", minGrow);
            }
            else
            {
                Debug.LogWarning("❌ Material has no _Grow property!");
            }
        }
    }

    void Update()
    {
        if (material == null) return;

        int collectedCount = SoundMemoryManager.Instance?.GetCollectedCount() ?? 0;
        float t = Mathf.Clamp01((float)collectedCount / maxCollectedCount);
        float targetGrow = Mathf.Lerp(minGrow, maxGrow, t);
        currentGrow = Mathf.MoveTowards(currentGrow, targetGrow, Time.deltaTime * growSpeed);

        material.SetFloat("_Grow", currentGrow);

        // Debug.Log($"🌱 Collected: {collectedCount} / {maxCollectedCount} | t = {t:F2} | Grow = {currentGrow:F2}");
    }
}
