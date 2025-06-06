using UnityEngine;

public class BuildingColorChanger : MonoBehaviour
{
    public Material buildingMaterial;
    public Color targetTopColor;
    public Color targetBottomColor;

    void Start()
    {
        if (buildingMaterial == null)
        {
            Debug.LogError("❌ BuildingColorChanger: buildingMaterial is not assigned!");
        }
        else
        {
            Debug.Log($"✅ BuildingColorChanger: Material assigned - {buildingMaterial.name}");
            Debug.Log($"🎨 Current colors - Top: {buildingMaterial.GetColor("_Top_Color")}, Bottom: {buildingMaterial.GetColor("_Bottom_Color")}");
        }
    }

    // 호출 함수
    public void ChangeBuildingColor()
    {
        if (buildingMaterial != null)
        {
            Debug.Log($"🔄 Changing building colors to - Top: {targetTopColor}, Bottom: {targetBottomColor}");
            buildingMaterial.SetColor("_Top_Color", targetTopColor);
            buildingMaterial.SetColor("_Bottom_Color", targetBottomColor);
            Debug.Log($"✅ Colors changed - Top: {buildingMaterial.GetColor("_Top_Color")}, Bottom: {buildingMaterial.GetColor("_Bottom_Color")}");
        }
        else
        {
            Debug.LogError("❌ BuildingColorChanger: Cannot change colors - buildingMaterial is null!");
        }
    }
} 