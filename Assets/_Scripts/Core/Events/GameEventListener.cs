using UnityEngine;
using UnityEngine.Events;

namespace MellowAbelson.Core.Events
{
    public class GameEventListener : MonoBehaviour
    {
        [SerializeField] private GameEventSO _event;
        [SerializeField] private UnityEvent _response;

        private void OnEnable()
        {
            if (_event != null)
                _event.OnRaised.AddListener(OnEventRaised);
        }

        private void OnDisable()
        {
            if (_event != null)
                _event.OnRaised.RemoveListener(OnEventRaised);
        }

        private void OnEventRaised()
        {
            _response?.Invoke();
        }
    }
}
