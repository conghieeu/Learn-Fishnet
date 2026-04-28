using FishNet.Object;
using FishNet.Object.Synchronizing;
using MellowAbelson.Core.Save;
using UnityEngine;

namespace MellowAbelson.Player.Health
{
    public class PlayerHealth : NetworkBehaviour, ISaveable
    {
        [SyncVar] private float _currentHealth;
        [SyncVar] private float _maxHealth = 100f;
        [SyncVar] private bool _isDead;

        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _maxHealth;
        public bool IsDead => _isDead;
        public float HealthPercent => _maxHealth > 0 ? _currentHealth / _maxHealth : 0f;
        public string SaveKey => $"PlayerHealth_{OwnerId}";

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();
            if (IsServerInitialized)
                _currentHealth = _maxHealth;
        }

        [ServerRpc]
        public void ServerTakeDamage(DamageInfo damage)
        {
            if (_isDead) return;

            _currentHealth = Mathf.Max(0, _currentHealth - damage.Amount);
            ObserversOnDamageTaken(damage);

            if (_currentHealth <= 0)
            {
                _isDead = true;
                ObserversOnDeath(transform.position);
            }
        }

        [ServerRpc]
        public void ServerHeal(float amount)
        {
            if (_isDead) return;
            _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
        }

        [ObserversRpc]
        private void ObserversOnDamageTaken(DamageInfo damage)
        {
            // Client-side effects (VFX, UI, sound)
        }

        [ObserversRpc]
        private void ObserversOnDeath(Vector3 deathPosition)
        {
            // Client-side death effects
        }

        public object CaptureState() => new HealthSaveData
        {
            CurrentHealth = _currentHealth,
            IsDead = _isDead
        };

        public void RestoreState(object state)
        {
            if (state is HealthSaveData data)
            {
                _currentHealth = data.CurrentHealth;
                _isDead = data.IsDead;
            }
        }

        [System.Serializable]
        private struct HealthSaveData
        {
            public float CurrentHealth;
            public bool IsDead;
        }
    }
}
