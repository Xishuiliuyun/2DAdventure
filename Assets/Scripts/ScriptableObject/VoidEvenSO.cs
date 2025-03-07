using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/VoidEvenSO")]
public class VoidEvenSO : ScriptableObject
{
    public UnityAction OnEventRised;

    public void RaiseEvent()
    {
        OnEventRised?.Invoke();
    }

}
