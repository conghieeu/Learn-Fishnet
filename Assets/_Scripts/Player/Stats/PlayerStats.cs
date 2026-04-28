using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using UnityEngine;

namespace MellowAbelson.Player.Stats
{
    public class PlayerStats : NetworkBehaviour
    {
        [SyncVar] private float _baseMoveSpeed = 5f;
        [SyncVar] private float _moveSpeedMultiplier = 1f;
        [SyncVar] private float _damageMultiplier = 1f;
        [SyncVar] private float _defense = 0f;

        private readonly List<StatModifier> _modifiers = new();

        public float MoveSpeed => _baseMoveSpeed * _moveSpeedMultiplier;
        public float DamageMultiplier => _damageMultiplier;
        public float Defense => _defense;

        public void AddModifier(StatModifier modifier)
        {
            _modifiers.Add(modifier);
            Recalculate();
        }

        public void RemoveModifier(StatModifier modifier)
        {
            _modifiers.Remove(modifier);
            Recalculate();
        }

        [Server]
        private void Recalculate()
        {
            _moveSpeedMultiplier = 1f;
            _damageMultiplier = 1f;
            _defense = 0f;

            foreach (var mod in _modifiers)
            {
                switch (mod.Type)
                {
                    case StatType.MoveSpeed:
                        _moveSpeedMultiplier = mod.IsMultiplier
                            ? _moveSpeedMultiplier * mod.Value
                            : _moveSpeedMultiplier + mod.Value;
                        break;
                    case StatType.Damage:
                        _damageMultiplier = mod.IsMultiplier
                            ? _damageMultiplier * mod.Value
                            : _damageMultiplier + mod.Value;
                        break;
                    case StatType.Defense:
                        _defense += mod.Value;
                        break;
                }
            }
        }
    }
}
