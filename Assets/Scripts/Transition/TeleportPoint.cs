using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TeleportPoint : MonoBehaviour, IInteractable
{
    public SceneLoadEventSO loadEventSO;
    public GameSceneSO sceneToGo;
    public Vector3 positionToGo;

    //用于显示交互按钮，交互后再进行传送的类型，Tag要设置成Interactable
    public void TriggerAction()
    {
        Debug.Log("teleport");
        loadEventSO.RaiseLoadRequestEvent(sceneToGo, positionToGo, true);
    }

    //用于不显示交互按钮，进入该区域即进行传送的类型，Tag要设置成Teleport
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && this.gameObject.CompareTag("Teleport"))
        {
            TriggerAction();
        }
    }
}
