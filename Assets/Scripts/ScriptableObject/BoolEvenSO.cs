using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/BoolEvenSO")]
public class BoolEvenSO : ScriptableObject
{
    public UnityAction<bool> OnEventRised;

    public void RaiseEvent(bool flag)
    {
        OnEventRised?.Invoke(flag);
    }
}
