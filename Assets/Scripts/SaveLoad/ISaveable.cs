using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveable 
{
    DataDefinition GetDataID();
    void RegisterSaveData() => DataManager.instance.RegisterSaveData(this); //���б�ע������
    void UnRegisterSaveData() => DataManager.instance.UnRegisterSaveData(this);//���б��Ƴ�����

    void GetSaveData(Data data);//��������
    void LoadData(Data data);//��������
}
