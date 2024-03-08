using System;
using UnityEngine;
using UnityEngine.Events;
using Omnix.Serializables;

[Serializable]
public class SimpleState
{
    [SerializeField] internal string name;
    [SerializeField] internal UnityFunc<bool> CompletionCheck;
    [SerializeField, Space(10)] internal UnityEvent OnStateEnter;
    [SerializeField] internal UnityEvent OnStateStay;
    [SerializeField] internal UnityEvent OnStateExit;

    public override string ToString()
    {
        if (string.IsNullOrEmpty(name)) return $"SimpleState(null)";
        return $"SimpleState({name})";
    }
}