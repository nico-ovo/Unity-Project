using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/CharacterEventSO")]
public class CharacterEventSO : ScriptableObject
{
    //Character的调用
    public UnityAction<Character> OnEventRaised;

    //如果有则将其Character传递进来
    public void RaiseEvent(Character character)
    {
        OnEventRaised?.Invoke(character);
    }
}
