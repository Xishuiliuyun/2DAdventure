using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;


//��ϷԤ������Դ(��ǰδ������)
[CreateAssetMenu(menuName ="GamePrefab/GamePrefabSO")]
public class GamePrefabSO : ScriptableObject
{
    public PrefabType prefabType;
    public AssetReference prefabReference;
}
