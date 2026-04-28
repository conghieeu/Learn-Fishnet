using UnityEngine;

namespace MellowAbelson.AI.Utilities
{
    public static class CheckFieldOfView
    {
        /// <summary>
        /// Kiểm tra target có nằm trong tầm nhìn không (khoảng cách, góc, vật cản).
        /// </summary>
        public static bool IsTargetInSight(
            Vector3 eyePosition,
            Vector3 forwardDirection,
            GameObject target,
            float sightDistance,
            float viewAngle,
            LayerMask obstacleMask,
            float eyeHeight = 1.5f)
        {
            if (target == null) return false;

            Vector3 eyePos = eyePosition + Vector3.up * eyeHeight;
            Vector3 targetPos = target.transform.position + Vector3.up * eyeHeight;
            Vector3 dirToTarget = targetPos - eyePos;
            float distance = dirToTarget.magnitude;

            if (distance > sightDistance) return false;

            dirToTarget.Normalize();
            float angle = Vector3.Angle(forwardDirection, dirToTarget);

            if (angle > viewAngle * 0.5f) return false;

            if (obstacleMask != 0)
            {
                if (Physics.Raycast(eyePos, dirToTarget, out RaycastHit hit, distance, obstacleMask))
                {
                    if (hit.transform != target.transform && !hit.transform.IsChildOf(target.transform))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Kiểm tra target trong tầm nhìn từ transform agent.
        /// </summary>
        public static bool IsTargetInSight(
            Transform agent,
            GameObject target,
            float sightDistance = 10f,
            float viewAngle = 90f,
            LayerMask obstacleMask = default,
            float eyeHeight = 1.5f)
        {
            return IsTargetInSight(
                agent.position,
                agent.forward,
                target,
                sightDistance,
                viewAngle,
                obstacleMask,
                eyeHeight);
        }
    }
}
