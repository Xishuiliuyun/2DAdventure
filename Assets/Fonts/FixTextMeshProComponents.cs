using UnityEngine;
using TMPro;

public class FixTextMeshProComponents : MonoBehaviour
{
    public TMP_FontAsset fontAsset; // 指定字体资源
    public Material fontMaterial;   // 指定字体材质（可选）

    [ContextMenu("Set All TextMeshPro Fonts")]
    public void SetAllTextMeshProFonts()
    {
        if (fontAsset == null)
        {
            Debug.LogError("Font Asset is not assigned!");
            return;
        }

        // 获取场景中所有的 TextMeshProUGUI 组件
        TextMeshProUGUI[] textMeshProUGUIs = FindObjectsOfType<TextMeshProUGUI>(true);
        foreach (var tmp in textMeshProUGUIs)
        {
            tmp.font = fontAsset; // 设置字体
            if (fontMaterial != null)
            {
                tmp.fontSharedMaterial = fontMaterial; // 设置材质
            }
            Debug.Log("Set font for: " + tmp.gameObject.name);
        }

        // 获取场景中所有的 TextMeshPro 组件
        TextMeshPro[] textMeshPros = FindObjectsOfType<TextMeshPro>(true);
        foreach (var tmp in textMeshPros)
        {
            tmp.font = fontAsset; // 设置字体
            if (fontMaterial != null)
            {
                tmp.fontSharedMaterial = fontMaterial; // 设置材质
            }
            Debug.Log("Set font for: " + tmp.gameObject.name);
        }

        Debug.Log("All TextMeshPro fonts have been set.");
    }
}