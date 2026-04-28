using System;
using System.Collections.Generic;

namespace MellowAbelson.AI.State
{
    /// <summary>
    /// State machine đơn giản cho AI, thay thế NodeCanvas FSM.
    /// </summary>
    public class MonsterStateMachine
    {
        private readonly Dictionary<MonsterState, Action> _stateActions;
        private MonsterState _currentState;

        public MonsterState CurrentState => _currentState;

        public event Action<MonsterState, MonsterState> OnStateChanged;

        public MonsterStateMachine()
        {
            _stateActions = new Dictionary<MonsterState, Action>();
            _currentState = MonsterState.Idle;
        }

        public void RegisterState(MonsterState state, Action action)
        {
            _stateActions[state] = action;
        }

        public void SetState(MonsterState newState)
        {
            if (_currentState == newState) return;
            var oldState = _currentState;
            _currentState = newState;
            OnStateChanged?.Invoke(oldState, newState);
        }

        public void Update()
        {
            if (_stateActions.TryGetValue(_currentState, out var action))
                action?.Invoke();
        }

        public void Reset()
        {
            _currentState = MonsterState.Idle;
        }
    }
}
