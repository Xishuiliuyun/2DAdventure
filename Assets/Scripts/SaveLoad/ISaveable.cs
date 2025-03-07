using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveable 
{
    DataDefinition GetDataID();
    void RegisterSaveData() => DataManager.instance.RegisterSaveData(this); //向列表注册数据
    void UnRegisterSaveData() => DataManager.instance.UnRegisterSaveData(this);//在列表移除数据

    void GetSaveData(Data data);//保存数据
    void LoadData(Data data);//加载数据
}
