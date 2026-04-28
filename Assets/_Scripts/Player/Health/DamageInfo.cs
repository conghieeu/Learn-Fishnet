using System;
using UnityEngine;

namespace MellowAbelson.Player.Health
{
    [Serializable]
    public struct DamageInfo
    {
        public float Amount;
        public DamageType Type;
        public int SourceNetId;
        public Vector3 HitPoint;
        public Vector3 HitDirection;

        public DamageInfo(float amount, DamageType type = DamageType.Physical,
            int sourceNetId = 0, Vector3 hitPoint = default, Vector3 hitDirection = default)
        {
            Amount = amount;
            Type = type;
            SourceNetId = sourceNetId;
            HitPoint = hitPoint;
            HitDirection = hitDirection;
        }
    }

    public enum DamageType
    {
        Physical,
        Fall,
        Fire,
        Poison,
        Explosion,
        Crush,
        Drowning
    }
}
