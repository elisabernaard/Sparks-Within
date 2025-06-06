using UnityEngine;

public class SkyboxColorChanger : MonoBehaviour
{
    public Material skyboxMaterial;
    public Color targetTopColor;
    public Color targetBottomColor;

    // 호출 함수
    public void ChangeSkyboxColor()
    {
        if (skyboxMaterial != null)
        {
            skyboxMaterial.SetColor("_TopColor", targetTopColor);
            skyboxMaterial.SetColor("_BottomColor", targetBottomColor);
        }
    }
}
