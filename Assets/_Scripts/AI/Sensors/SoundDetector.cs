using FishNet.Object;
using UnityEngine;

namespace MellowAbelson.AI.Sensors
{
    public enum SoundType
    {
        Footstep,
        Voice,
        Gunshot,
        Door,
        ItemDrop,
        Explosion,
        Jump,
        Sprint
    }

    public class SoundDetector : NetworkBehaviour
    {
        [SerializeField] private float _hearingRange = 20f;

        public void EmitSound(Vector3 position, SoundType type, float intensity)
        {
            if (!IsServer) return;

            var colliders = Physics.OverlapSphere(position, _hearingRange * intensity);
            foreach (var col in colliders)
            {
                var monster = col.GetComponentInParent<MonsterController>();
                if (monster != null)
                {
                    float distance = Vector3.Distance(position, monster.transform.position);
                    if (distance <= _hearingRange * intensity)
                    {
                        monster.ReportSoundHeard(position, intensity);
                    }
                }
            }
        }
    }
}
