using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace CustomNodes.Conditions
{
    [Category("Physics")]
    [Name("Check Field Of View")]
    [Description("Kiểm tra xem mục tiêu có nằm trong tầm nhìn (khoảng cách, góc nhìn, và không bị vật cản che khuất) hay không. Rất hữu ích cho AI Stealth/Detection.")]
    public class CheckFieldOfView : ConditionTask<Transform>
    {
        [RequiredField]
        [Tooltip("Mục tiêu cần kiểm tra")]
        public BBParameter<GameObject> target;

        [Tooltip("Khoảng cách tối đa nhìn thấy")]
        public BBParameter<float> sightDistance = 10f;

        [Tooltip("Góc nhìn (độ). Ví dụ: 90 nghĩa là quét từ giữa sang trái/phải mỗi bên 45 độ.")]
        [SliderField(1f, 360f)]
        public BBParameter<float> viewAngle = 90f;

        [Tooltip("LayerMask của các vật cản che tầm nhìn (ví dụ: Tường, Vật thể cứng).")]
        public BBParameter<LayerMask> obstacleMask = (LayerMask)0;

        [Tooltip("Độ cao của mắt (tính từ pivot nhân vật).")]
        public BBParameter<float> eyeHeight = 1.5f;

        // Nội dung hiển thị ngắn gọn ngoài Node
        protected override string info => $"Nhìn thấy {target} trong FOV?";

        protected override bool OnCheck()
        {
            if (target.value == null || agent == null) return false;

            Vector3 eyePosition = agent.position + Vector3.up * eyeHeight.value;
            Vector3 targetPosition = target.value.transform.position + Vector3.up * eyeHeight.value; 

            Vector3 dirToTarget = (targetPosition - eyePosition);
            float distanceToTarget = dirToTarget.magnitude;

            // 1. Kiểm tra khoảng cách
            if (distanceToTarget > sightDistance.value)
            {
                return false;
            }

            // 2. Kiểm tra góc nhìn
            dirToTarget.Normalize();
            float angleToTarget = Vector3.Angle(agent.forward, dirToTarget);
            
            if (angleToTarget > viewAngle.value / 2f)
            {
                return false; // Nằm ngoài góc nhìn
            }

            // 3. Kiểm tra vật cản (Raycast)
            // Nếu có thiết lập LayerMask vật cản thì bắn ray
            if (obstacleMask.value != 0)
            {
                if (Physics.Raycast(eyePosition, dirToTarget, out RaycastHit hit, distanceToTarget, obstacleMask.value))
                {
                    // Nếu tia ray đập trúng vật cản trước khi chạm được target
                    if (hit.transform != target.value.transform && !hit.transform.IsChildOf(target.value.transform))
                    {
                        return false; 
                    }
                }
            }

            return true;
        }

        // Vẽ Gizmos để dễ canh chỉnh trong Editor
        public override void OnDrawGizmosSelected()
        {
            if (agent != null)
            {
                Vector3 eyePosition = agent.position + Vector3.up * eyeHeight.value;
                
                // Vẽ giới hạn góc nhìn
                Vector3 leftLimit = Quaternion.AngleAxis(-viewAngle.value / 2f, Vector3.up) * agent.forward;
                Vector3 rightLimit = Quaternion.AngleAxis(viewAngle.value / 2f, Vector3.up) * agent.forward;
                
                Gizmos.color = Color.cyan;
                Gizmos.DrawRay(eyePosition, leftLimit * sightDistance.value);
                Gizmos.DrawRay(eyePosition, rightLimit * sightDistance.value);

                // Vẽ cung nối 2 tia để tạo hình quạt
                Gizmos.DrawLine(eyePosition + leftLimit * sightDistance.value, eyePosition + rightLimit * sightDistance.value);
            }
        }
    }
}
