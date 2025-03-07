using UnityEngine;
using TMPro;

public class FixTextMeshProComponents : MonoBehaviour
{
    public TMP_FontAsset fontAsset; // ָ��������Դ
    public Material fontMaterial;   // ָ��������ʣ���ѡ��

    [ContextMenu("Set All TextMeshPro Fonts")]
    public void SetAllTextMeshProFonts()
    {
        if (fontAsset == null)
        {
            Debug.LogError("Font Asset is not assigned!");
            return;
        }

        // ��ȡ���������е� TextMeshProUGUI ���
        TextMeshProUGUI[] textMeshProUGUIs = FindObjectsOfType<TextMeshProUGUI>(true);
        foreach (var tmp in textMeshProUGUIs)
        {
            tmp.font = fontAsset; // ��������
            if (fontMaterial != null)
            {
                tmp.fontSharedMaterial = fontMaterial; // ���ò���
            }
            Debug.Log("Set font for: " + tmp.gameObject.name);
        }

        // ��ȡ���������е� TextMeshPro ���
        TextMeshPro[] textMeshPros = FindObjectsOfType<TextMeshPro>(true);
        foreach (var tmp in textMeshPros)
        {
            tmp.font = fontAsset; // ��������
            if (fontMaterial != null)
            {
                tmp.fontSharedMaterial = fontMaterial; // ���ò���
            }
            Debug.Log("Set font for: " + tmp.gameObject.name);
        }

        Debug.Log("All TextMeshPro fonts have been set.");
    }
}