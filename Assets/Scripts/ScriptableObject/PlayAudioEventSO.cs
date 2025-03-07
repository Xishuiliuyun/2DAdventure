using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/PlayAudioEventSO")]
public class PlayAudioEventSO : ScriptableObject
{
    public UnityAction<AudioClip> OnEventRised;

    public void RaiseEvent(AudioClip audioClip)
    {
        OnEventRised?.Invoke(audioClip);
    }
}
