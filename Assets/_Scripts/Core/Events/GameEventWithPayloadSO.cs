using UnityEngine;
using UnityEngine.Events;

namespace MellowAbelson.Core.Events
{
    public abstract class GameEventWithPayloadSO<T> : ScriptableObject
    {
        public UnityEvent<T> OnRaised { get; private set; } = new UnityEvent<T>();

        public void Raise(T payload)
        {
            OnRaised?.Invoke(payload);
        }
    }
}
