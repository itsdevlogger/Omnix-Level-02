using System;

namespace Omnix.BaseClasses
{
    #region State = Method Callback
    /// <summary> State machine where each state of the machine is a method callback with 0 input-argument </summary>
    public class SimpleStateMachine
    {
        private Action _activeState;
        private readonly Action _baseState;

        /// <summary> Construct a simple state machine </summary>
        /// <param name="baseState"> idle state of the machine </param>
        protected SimpleStateMachine(Action baseState)
        {
            _baseState = baseState;
            _activeState = baseState;
        }

        /// <summary> Switch to idle state </summary>
        public void SwitchToBaseState() => _activeState = _baseState;

        /// <summary> Switch to given state </summary>
        public void SwitchTo(Action newState) => _activeState = newState;

        /// <summary> Invoke active state </summary>
        public void Call() => _activeState?.Invoke();
    }

    /// <summary> State machine where each state of the machine is a method callback with 1 input-argument </summary>
    public class SimpleStateMachine<T0>
    {
        private Action<T0> _activeState;
        private Action<T0> _baseState;

        /// <summary> Construct a simple state machine </summary>
        /// <param name="baseState"> idle state of the machine </param>
        protected SimpleStateMachine(Action<T0> baseState)
        {
            _baseState = baseState;
            _activeState = baseState;
        }

        /// <summary> Switch to idle state </summary>
        public void SwitchToBaseState() => _activeState = _baseState;

        /// <summary> Switch to given state </summary>
        public void SwitchTo(Action<T0> newState) => _activeState = newState;

        /// <summary> Invoke active state </summary>
        public void Call(T0 arg0) => _activeState?.Invoke(arg0);
    }

    /// <summary> State machine where each state of the machine is a method callback with 2 input-argument </summary>
    public class SimpleStateMachine<T0, T1>
    {
        private Action<T0, T1> _activeState;
        private Action<T0, T1> _baseState;

        /// <summary> Construct a simple state machine </summary>
        /// <param name="baseState"> idle state of the machine </param>
        protected SimpleStateMachine(Action<T0, T1> baseState)
        {
            _baseState = baseState;
            _activeState = baseState;
        }

        /// <summary> Switch to idle state </summary>
        public void SwitchToBaseState() => _activeState = _baseState;

        /// <summary> Switch to given state </summary>
        public void SwitchTo(Action<T0, T1> newState) => _activeState = newState;

        /// <summary> Invoke active state </summary>
        public void Call(T0 arg0, T1 arg1) => _activeState?.Invoke(arg0, arg1);
    }

    /// <summary> State machine where each state of the machine is a method callback with 3 input-argument </summary>
    public class SimpleStateMachine<T0, T1, T2>
    {
        private Action<T0, T1, T2> _activeState;
        private Action<T0, T1, T2> _baseState;

        /// <summary> Construct a simple state machine </summary>
        /// <param name="baseState"> idle state of the machine </param>
        protected SimpleStateMachine(Action<T0, T1, T2> baseState)
        {
            _baseState = baseState;
            _activeState = baseState;
        }

        /// <summary> Switch to idle state </summary>
        public void SwitchToBaseState() => _activeState = _baseState;

        /// <summary> Switch to given state </summary>
        public void SwitchTo(Action<T0, T1, T2> newState) => _activeState = newState;

        /// <summary> Invoke active state </summary>
        public void Call(T0 arg0, T1 arg1, T2 arg2) => _activeState?.Invoke(arg0, arg1, arg2);
    }

    /// <summary> State machine where each state of the machine is a method callback with 4 input-argument </summary>
    public class SimpleStateMachine<T0, T1, T2, T3>
    {
        private Action<T0, T1, T2, T3> _activeState;
        private Action<T0, T1, T2, T3> _baseState;

        /// <summary> Construct a simple state machine </summary>
        /// <param name="baseState"> idle state of the machine </param>
        protected SimpleStateMachine(Action<T0, T1, T2, T3> baseState)
        {
            _baseState = baseState;
            _activeState = baseState;
        }

        /// <summary> Switch to idle state </summary>
        public void SwitchToBaseState() => _activeState = _baseState;

        /// <summary> Switch to given state </summary>
        public void SwitchTo(Action<T0, T1, T2, T3> newState) => _activeState = newState;

        /// <summary> Invoke active state </summary>
        public void Call(T0 arg0, T1 arg1, T2 arg2, T3 arg3) => _activeState?.Invoke(arg0, arg1, arg2, arg3);
    }

    /// <summary> State machine where each state of the machine is a method callback with 4 input-argument </summary>
    public class SimpleStateMachine<T0, T1, T2, T3, T4>
    {
        private Action<T0, T1, T2, T3, T4> _activeState;
        private Action<T0, T1, T2, T3, T4> _baseState;

        /// <summary> Construct a simple state machine </summary>
        /// <param name="baseState"> idle state of the machine </param>
        protected SimpleStateMachine(Action<T0, T1, T2, T3, T4> baseState)
        {
            _baseState = baseState;
            _activeState = baseState;
        }

        /// <summary> Switch to idle state </summary>
        public void SwitchToBaseState() => _activeState = _baseState;

        /// <summary> Switch to given state </summary>
        public void SwitchTo(Action<T0, T1, T2, T3, T4> newState) => _activeState = newState;

        /// <summary> Invoke active state </summary>
        public void Call(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => _activeState?.Invoke(arg0, arg1, arg2, arg3, arg4);
    }
    #endregion

    #region State = Class (State don't know master)
    /// <summary> Base interface for all state-interfaces </summary>
    public interface IState
    {
        /// <summary> Called when this state is activated </summary>
        public void OnEnter();

        /// <summary> Called when this state is active </summary>
        public void OnStay();

        /// <summary> Called when this state is deactivated </summary>
        public void OnExit();
    }
    
    /// <summary> State machine where each state of the machine is a separate class that implements certain interface </summary>
    public class StateMachine
    {
        private IState _activeState;
        private readonly IState _baseState;

        public StateMachine(IState baseState)
        {
            _activeState = baseState;
            _baseState = baseState;
        }

        /// <summary> Switch to idle state </summary>
        public void SwitchToBaseState() => SwitchTo(_baseState);
        
        /// <summary> Switch to given state </summary>
        public void SwitchTo(IState newState)
        {
            if (_activeState != null)
            {
                _activeState.OnExit();
            }

            _activeState = newState;
            if (_activeState != null)
            {
                _activeState.OnEnter();
            }
        }
        
        /// <summary> Invoke OnStay method of active state </summary>
        public void Call() => _activeState?.OnStay();
    }
    #endregion
    
    #region State = Class (State knows master)
    /// <summary> Base interface for all state-interfaces </summary>
    /// <typeparam name="TMaster"> Type of object that is controlling this machine. </typeparam>
    public interface IState<in TMaster>
    {
        /// <summary> Called when this state is activated </summary>
        public void OnEnter(TMaster master);
       
        /// <summary> Called when this state is active </summary>
        public void OnStay(TMaster master);
       
        /// <summary> Called when this state is deactivated </summary>
        public void OnExit(TMaster master);
    }
    
    /// <summary> State machine where each state of the machine is a separate class that implements certain interface </summary>
    /// <typeparam name="TMaster"> Type parameter for IState interface </typeparam>
    public class StateMachine<TMaster>
    {
        private IState<TMaster> _activeState;
        private readonly IState<TMaster> _baseState;
        private readonly TMaster _master;

        public StateMachine(IState<TMaster> baseState, TMaster master)
        {
            _master = master;
            _activeState = baseState;
            _baseState = baseState;
        }
        
        /// <summary> Switch to idle state </summary>
        public void SwitchToBaseState() => SwitchTo(_baseState);
        
        /// <summary> Switch to given state </summary>
        public void SwitchTo(IState<TMaster> newState)
        {
            if (_activeState != null)
            {
                _activeState.OnExit(_master);
            }

            _activeState = newState;
            if (_activeState != null)
            {
                _activeState.OnEnter(_master);
            }
        }
            
        /// <summary> Invoke OnStay method of active state </summary>
        public void Call() => _activeState?.OnStay(_master);}
    #endregion
}