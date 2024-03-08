using System;
using System.Collections;
using System.Reflection;
using Omnix.Extensions;
using Omnix.Utils;
using UnityEngine;
using UnityEngine.Events;


public class SimpleStateMachine : MonoBehaviour
{
    private enum StateMachineMode
    {
        Normal,
        SwitchingState,
        InvokeAnyExit,
        InvokeAnyEnter,
        AboutInvokeAnyEnter,
        AboutSwitchState
    }

    [SerializeField] private bool logging;
    [SerializeField] private bool isSingleton;
    [SerializeField] private string firstState;
    [SerializeField, Space(20)] private UnityEvent onAnyStateEnter;
    [SerializeField] private UnityEvent onAnyStateExit;
    [SerializeField, Space(20)] private SimpleState[] states;
    private SimpleState currentState;
    private SimpleState previousState;
    private float stateEntryTime = -1;
    private StateMachineMode _machineMode;
    private Action OnMachineSteteChanged;

    void Start()
    {
        _machineMode = StateMachineMode.Normal;
        if (states.Length == 0)
        {
            Debug.LogWarning($"{gameObject.name} is StateMachine with 0 states. Destroying component.");
            Destroy(this);
            return;
        }

        if (isSingleton) DontDestroyOnLoad(gameObject);

        currentState = null;
        previousState = null;
        SimpleState tempState = GetStateByGuid(firstState);
        if (tempState == null)
        {
            tempState = states[0];
            Debug.LogWarning($"State with guid ({firstState}) not found, switched to {tempState.name} instead.");
        }

        SwitchStateInner(tempState, false, true);
        Debug.Log($"Started with {tempState}");
    }

    void Update()
    {
        if (_machineMode != StateMachineMode.Normal) return;

        if (currentState == null)
        {
            return;
        }

        if (currentState.CompletionCheck.Invoke())
        {
            Debug.Log($"State Completed: {currentState}");
            _machineMode = StateMachineMode.AboutSwitchState;
            SwitchStateInner(null);
            return;
        }

        currentState.OnStateStay?.Invoke();
    }

    private int GetStateIndex(SimpleState state)
    {
        if (state == null) return -1;
        string stateName = state.name.ToLower();
        foreach (Element<SimpleState> element in Python.Enumerate(states))
        {
            if (element.Item.name.ToLower() == stateName)
            {
                return element.Index;
            }
        }

        return -1;
    }

    private void SwitchStateInner(SimpleState state, bool invokeAnyExit = true, bool invokeAnyEnter = true, Action OnCompleteAction = null)
    {
        StartCoroutine(SwitchStateCour(state, invokeAnyExit, invokeAnyEnter, OnCompleteAction));
    }

    private bool IsSwitchingState()
    {
        return _machineMode == StateMachineMode.SwitchingState || _machineMode == StateMachineMode.InvokeAnyEnter || _machineMode == StateMachineMode.InvokeAnyExit;
    }

    private IEnumerator SwitchStateCour(SimpleState state, bool invokeAnyExit, bool invokeAnyEnter, Action OnCompleteAction)
    {
        // Exit State
        if (logging) Debug.Log($"Switching to {state}");
        invokeAnyExit = invokeAnyExit && _machineMode != StateMachineMode.InvokeAnyExit;
        invokeAnyEnter = invokeAnyEnter && _machineMode != StateMachineMode.InvokeAnyEnter;

        if (IsSwitchingState() || _machineMode == StateMachineMode.AboutInvokeAnyEnter)
        {
            yield return new WaitWhile(() => IsSwitchingState() || _machineMode == StateMachineMode.AboutInvokeAnyEnter);
        }

        if (logging) Debug.Log($"Switching to {state} WaitComplete");
        if (invokeAnyExit)
        {
            if (logging) Debug.Log($"Switching to {state} Invoke AnyExit");
            _machineMode = StateMachineMode.InvokeAnyExit;
            onAnyStateExit?.Invoke();
            if (logging) Debug.Log($"Switching to {state} Invoke AnyExit Done");
        }

        if (logging) Debug.Log($"Switching to {state} Invoke StateExit");
        _machineMode = StateMachineMode.SwitchingState;
        if (state != null) state.OnStateExit?.Invoke();
        if (logging) Debug.Log($"Switching to {state} Invoke StateExit Done");

        // Update Variables
        if (logging) Debug.Log($"Switching to {state} Update Variables");
        previousState = currentState;
        currentState = state;
        stateEntryTime = Time.time;
        if (logging) Debug.Log($"Switching to {state} Update Variables Done");

        // Enter State
        if (logging) Debug.Log($"Switching to {state} Enter State Wait");
        _machineMode = StateMachineMode.AboutInvokeAnyEnter;

        if (IsSwitchingState())
        {
            yield return new WaitWhile(IsSwitchingState);
        }

        if (logging) Debug.Log($"Switching to {state} Enter State Wait Done");
        if (invokeAnyEnter)
        {
            if (logging) Debug.Log($"Switching to {state} Invoke AnyEnter");
            _machineMode = StateMachineMode.InvokeAnyEnter;
            onAnyStateEnter?.Invoke();
            if (logging) Debug.Log($"Switching to {state} Invoke AnyEnter Done");
        }

        if (logging) Debug.Log($"Switching to {state} Invoke StateEnter");
        if (state != null) state.OnStateEnter?.Invoke();
        _machineMode = StateMachineMode.Normal;
        if (logging) Debug.Log($"Switching to {state} Done");
        OnCompleteAction?.Invoke();
    }


    /// <summary>
    /// Returns state specified by guid.
    /// </summary>
    /// <param name="guid">
    /// A simple guid to any state is name of that state. But there are advanced guids.
    /// A list of such guids is as follows:
    ///     [next] or [Next] maps to next state in index
    ///     [prev] or [Prev] maps to previous state in index
    ///     [back] or [Back] maps to state that came before current state (while execution)
    ///     [this] or [This] maps to current state
    ///     [null] or [Null] maps to empty state
    ///     [any] or [Any] maps to a random state
    ///     [t+n] or [T+n] maps to state with index currentStateIndex + n
    ///     [t-n] or [T-n] maps to state with index currentStateIndex - n
    ///     [n] or [+n] maps to state with index n (indexing starts at 0)
    ///     [-n] maps to state with index NumberOfStates-n (indexing starts at 0)
    /// </param>
    public void SwitchToState(string guid, Action onCompleteAction)
    {
        SimpleState state = GetStateByGuid(guid);
        if (state != null) SwitchStateInner(state, OnCompleteAction: onCompleteAction);
        else Debug.LogWarning($"State with guid ({guid}) not found");
    }

    /// <summary>
    /// Switch to state specified by stateIndex. Indexing starts at 0
    /// </summary>
    /// <param name="stateIndex">
    /// if stateIndex >= NumberOfStates: do nothing
    /// else if NumberOfStates > stateIndex >= 0: load state with index stateIndex
    /// else: load state with index NumberOfStates-stateIndex
    /// </param>
    public void SwitchToState(int stateIndex, Action onCompleteAction)
    {
        SimpleState state = GetStateByIndex(stateIndex);
        if (state == null) Debug.LogWarning($"State with name index ({stateIndex}) not found");
        else SwitchStateInner(state, OnCompleteAction: onCompleteAction);
    }

    public void SwitchToRandomState(Action onCompleteAction)
    {
        SwitchStateInner(states.RandomElement(), OnCompleteAction: onCompleteAction);
    }

    public void SwitchToNextStateInIndex(Action onCompleteAction)
    {
        if (currentState == null) return;
        SwitchToState(GetStateIndex(currentState) + 1, onCompleteAction: onCompleteAction);
    }

    public void SwitchToPreviousStateInIndex(Action onCompleteAction)
    {
        SwitchToState(GetStateIndex(currentState) - 1, onCompleteAction: onCompleteAction);
    }

    public void SwitchToPreviousState(Action onCompleteAction)
    {
        SwitchStateInner(previousState, OnCompleteAction: onCompleteAction);
    }

    public void RestartCurrentState(Action onCompleteAction)
    {
        SwitchStateInner(currentState, OnCompleteAction: onCompleteAction);
    }

    public void ExitCurrentState(Action onCompleteAction)
    {
        SwitchStateInner(null, OnCompleteAction: onCompleteAction);
    }


    /// <summary>
    /// Returns state specified by guid.
    /// </summary>
    /// <param name="guid">
    /// A simple guid to any state is name of that state. But there are advanced guids.
    /// A list of such guids is as follows:
    ///     [next] or [Next] maps to next state in index
    ///     [prev] or [Prev] maps to previous state in index
    ///     [back] or [Back] maps to state that came before current state (while execution)
    ///     [this] or [This] maps to current state
    ///     [null] or [Null] maps to empty state
    ///     [any] or [Any] maps to a random state
    ///     [t+n] or [T+n] maps to state with index currentStateIndex + n
    ///     [t-n] or [T-n] maps to state with index currentStateIndex - n
    ///     [n] or [+n] maps to state with index n (indexing starts at 0)
    ///     [-n] maps to state with index NumberOfStates-n (indexing starts at 0)
    /// </param>
    public void SwitchToState(string guid) => SwitchToState(guid, null);

    /// <summary>
    /// Switch to state specified by stateIndex. Indexing starts at 0
    /// </summary>
    /// <param name="stateIndex">
    /// if stateIndex >= NumberOfStates: do nothing
    /// else if NumberOfStates > stateIndex >= 0: load state with index stateIndex
    /// else: load state with index NumberOfStates-stateIndex
    /// </param>
    public void SwitchToState(int stateIndex) => SwitchToState(stateIndex, null);

    public void SwitchToRandomState() => SwitchToRandomState(null);
    public void SwitchToNextStateInIndex() => SwitchToNextStateInIndex(null);
    public void SwitchToPreviousStateInIndex() => SwitchToPreviousStateInIndex(null);
    public void SwitchToPreviousState() => SwitchToPreviousState(null);
    public void RestartCurrentState() => RestartCurrentState(null);
    public void ExitCurrentState() => ExitCurrentState(null);

    public SimpleState GetStateByGuid(string stateGuid)
    {
        string nameLower = stateGuid.ToLower();
        if (nameLower.StartsWith("[") && nameLower.EndsWith("]"))
        {
            if (nameLower == "[next]") return GetStateByIndex(GetStateIndex(currentState) - 1);
            if (nameLower == "[prev]") return GetStateByIndex(GetStateIndex(currentState) - 1);
            if (nameLower == "[back]") return previousState;
            if (nameLower == "[this]") return currentState;
            if (nameLower == "[null]") return null;
            if (nameLower == "[any]") return states.RandomElement();

            int index;
            string substring = nameLower.Substring(1, nameLower.Length - 2);
            Debug.Log($"sub: {substring}");
            if (int.TryParse(substring, out index)) return GetStateByIndex(index);
            if (nameLower.StartsWith("[t+") && int.TryParse(nameLower.Substring(3, nameLower.Length - 4), out index))
            {
                return GetStateByIndex(GetStateIndex(currentState) + index);
            }

            if (nameLower.StartsWith("[t-") && int.TryParse(nameLower.Substring(3, nameLower.Length - 4), out index))
            {
                return GetStateByIndex(GetStateIndex(currentState) - index);
            }
        }

        foreach (SimpleState state in states)
        {
            if (state.name.ToLower() == nameLower)
            {
                return state;
            }
        }

        return null;
    }

    public SimpleState GetStateByIndex(int stateIndex)
    {
        if (stateIndex < 0) return states[states.Length + stateIndex];
        if (stateIndex < states.Length) return states[stateIndex];
        return null;
    }

    public bool HasStateStayedFor(float duration)
    {
        return (stateEntryTime + duration) < Time.time;
    }
}