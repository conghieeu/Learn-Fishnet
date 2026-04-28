using System;

namespace MellowAbelson.Player.Stats
{
    [Serializable]
    public struct StatModifier
    {
        public StatType Type;
        public float Value;
        public bool IsMultiplier;

        public StatModifier(StatType type, float value, bool isMultiplier = false)
        {
            Type = type;
            Value = value;
            IsMultiplier = isMultiplier;
        }
    }
}
