using UnityEngine;
using UnityEngine.Events;

namespace MellowAbelson.Core.Events
{
    [CreateAssetMenu(menuName = "MellowAbelson/Game Event", fileName = "NewGameEvent")]
    public class GameEventSO : ScriptableObject
    {
        public UnityEvent OnRaised { get; private set; } = new UnityEvent();

        public void Raise()
        {
            OnRaised?.Invoke();
        }
    }
}
