/*using UnityEngine;
using UnityEditor;
using System.Linq;
using TMPro;
using static Unity.Burst.Intrinsics.X86.Avx;

public class ReplaceFontAssets : EditorWindow
{
    private TMP_FontAsset oldFontAsset;
    private TMP_FontAsset newFontAsset;

    [MenuItem("Tools/Replace TMP Font Assets")]
    public static void ShowWindow()
    {
        GetWindow<ReplaceFontAssets>("Replace TMP Font Assets");
    }

    private void OnGUI()
    {
        GUILayout.Label("Select Old and New Font Assets", EditorStyles.boldLabel);
        oldFontAsset = (TMP_FontAsset)EditorGUILayout.ObjectField("Old Font Asset:", oldFontAsset, typeof(TMP_FontAsset), false);
        newFontAsset = (TMP_FontAsset)EditorGUILayout.ObjectField("New Font Asset:", newFontAsset, typeof(TMP_FontAsset), false);

        if (GUILayout.Button("Replace Fonts"))
        {
            ReplaceFonts();
        }
    }

    private void ReplaceFonts()
    {
        if (oldFontAsset == null || newFontAsset == null)
        {
            Debug.LogError("Please select both old and new font assets.");
            return;
        }

        int replacedCount = 0;
        int prefabReplacedCount = 0;

        // ��ȡ���� TextMeshPro �� TextMeshProUGUI ������滻����
        replacedCount += ReplaceInSceneObjects(oldFontAsset, newFontAsset);
        //prefabReplacedCount += ReplaceInPrefabs(oldFontAsset, newFontAsset);

        Debug.Log($"Replaced font asset in {replacedCount} scene components and {prefabReplacedCount} prefab components.");

        // ǿ��ˢ�±༭������
        SceneView.RepaintAll();
        AssetDatabase.Refresh();
    }

    private int ReplaceInSceneObjects(TMP_FontAsset oldFontAsset, TMP_FontAsset newFontAsset)
    {
        int replacedCount = 0;

        var textMeshPros = Object.FindObjectsOfType<TextMeshPro>();
        foreach (var tmp in textMeshPros)
        {
            if (tmp.font == oldFontAsset)
            {
                tmp.font = newFontAsset;
                replacedCount++;
            }
            else if (tmp.font == null)
            {
                tmp.font = newFontAsset;
                replacedCount++;
            }
        }

        var textMeshProUGUIs = Object.FindObjectsOfType<TextMeshProUGUI>();
        foreach (var tmpUGUI in textMeshProUGUIs)
        {
            if (tmpUGUI.font == oldFontAsset)
            {
                tmpUGUI.font = newFontAsset;
                replacedCount++;
            }
            else if (tmpUGUI.font == null)
            {
                tmpUGUI.font = newFontAsset;
                replacedCount++;
            }
        }

        return replacedCount;
    }

    private int ReplaceInPrefabs(TMP_FontAsset oldFontAsset, TMP_FontAsset newFontAsset)
    {
        int replacedCount = 0;

        string[] guids = AssetDatabase.FindAssets("t:Prefab");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefabRoot = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefabRoot != null)
            {
                // ʹ�� PrefabUtility ������Ԥ�Ƽ�ʵ��
                GameObject prefabInstance = PrefabUtility.InstantiatePrefab(prefabRoot) as GameObject;
                if (prefabInstance != null)
                {
                    replacedCount += ReplaceInGameObject(prefabInstance, oldFontAsset, newFontAsset);

                    // ���޸�Ӧ�û�Ԥ�Ƽ�
                    PrefabUtility.SaveAsPrefabAsset(prefabInstance, path);
                    Object.DestroyImmediate(prefabInstance);
                }
            }
        }

        return replacedCount;
    }

    private int ReplaceInGameObject(GameObject go, TMP_FontAsset oldFontAsset, TMP_FontAsset newFontAsset)
    {
        int replacedCount = 0;

        // �滻��ǰ GameObject �ϵ����� TextMeshPro �� TextMeshProUGUI ���
        var textMeshPros = go.GetComponentsInChildren<TextMeshPro>(true);
        foreach (var tmp in textMeshPros)
        {
            if (tmp.font == oldFontAsset)
            {
                tmp.font = newFontAsset;
                replacedCount++;
            }
        }

        var textMeshProUGUIs = go.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (var tmpUGUI in textMeshProUGUIs)
        {
            if (tmpUGUI.font == oldFontAsset)
            {
                tmpUGUI.font = newFontAsset;
                replacedCount++;
            }
        }

        return replacedCount;
    }
}*/