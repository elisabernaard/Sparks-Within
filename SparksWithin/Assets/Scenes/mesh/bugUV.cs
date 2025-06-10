using UnityEngine;

public class CheckUVs : MonoBehaviour
{
    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>()?.mesh;
        if (mesh != null)
        {
            if (mesh.uv != null && mesh.uv.Length > 0)
            {
                Debug.Log("✅ UVs exist! Count: " + mesh.uv.Length);
            }
            else
            {
                Debug.LogWarning("❌ No UVs found on this mesh!");
            }
        }
    }
}