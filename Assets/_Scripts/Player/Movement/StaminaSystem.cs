using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace MellowAbelson.Player.Movement
{
    public class StaminaSystem : NetworkBehaviour
    {
        [SyncVar] private float _currentStamina;
        [SerializeField] private float _maxStamina = 100f;
        [SerializeField] private float _sprintDrainRate = 20f;
        [SerializeField] private float _regenerationRate = 15f;
        [SerializeField] private float _exhaustionDelay = 0.5f;

        private float _exhaustionTimer;
        private bool _isExhausted;

        public float StaminaPercent => _maxStamina > 0 ? _currentStamina / _maxStamina : 0f;
        public bool IsExhausted => _isExhausted;
        public bool HasStamina => _currentStamina > 0;

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();
            if (IsServerInitialized)
                _currentStamina = _maxStamina;
        }

        public void Update() { if (!IsOwner) return; }

        [Server]
        public void ConsumeSprint(float deltaTime)
        {
            if (_isExhausted) return;

            _currentStamina = Mathf.Max(0, _currentStamina - _sprintDrainRate * deltaTime);
            if (_currentStamina <= 0)
                _isExhausted = true;
        }

        [Server]
        public void Regenerate(float deltaTime)
        {
            if (_isExhausted)
            {
                _exhaustionTimer += deltaTime;
                if (_exhaustionTimer >= _exhaustionDelay)
                {
                    _isExhausted = false;
                    _exhaustionTimer = 0;
                }
                return;
            }

            _currentStamina = Mathf.Min(_maxStamina, _currentStamina + _regenerationRate * deltaTime);
        }
    }
}
