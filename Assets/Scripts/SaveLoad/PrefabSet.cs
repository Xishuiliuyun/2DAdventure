using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

//ÒÑÆúÓÃ
public class PrefabSet : MonoBehaviour
{
    public string PrefabName;
    public int num = 1;

    public PrefabData prefabData;


    private void Awake()
    {
        prefabData = new PrefabData { PrefabName = PrefabName, pos = new MyVector3(transform.position), scale = new MyVector3(transform.localScale),num = this.num };
    }
}
