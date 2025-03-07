using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName ="Event/FloatEvenSO")]
public class FloatEvenSO : ScriptableObject
{
    public UnityAction<float> OnEventRised;

    public void RaiseEvent(float val)
    {
        OnEventRised?.Invoke(val);
    }
}
