using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TeleportPoint : MonoBehaviour, IInteractable
{
    public SceneLoadEventSO loadEventSO;
    public GameSceneSO sceneToGo;
    public Vector3 positionToGo;

    //������ʾ������ť���������ٽ��д��͵����ͣ�TagҪ���ó�Interactable
    public void TriggerAction()
    {
        Debug.Log("teleport");
        loadEventSO.RaiseLoadRequestEvent(sceneToGo, positionToGo, true);
    }

    //���ڲ���ʾ������ť����������򼴽��д��͵����ͣ�TagҪ���ó�Teleport
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && this.gameObject.CompareTag("Teleport"))
        {
            TriggerAction();
        }
    }
}
