using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;


//游戏预制体资源(当前未有作用)
[CreateAssetMenu(menuName ="GamePrefab/GamePrefabSO")]
public class GamePrefabSO : ScriptableObject
{
    public PrefabType prefabType;
    public AssetReference prefabReference;
}
