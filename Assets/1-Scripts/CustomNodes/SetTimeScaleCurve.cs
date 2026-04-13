using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace CustomNodes.Actions
{
    [Category("Time")]
    [Name("Set TimeScale Over Time")]
    [Description("Thay đổi Time.timeScale mượt mà theo thời gian dựa vào AnimationCurve. Rất hữu ích để làm hiệu ứng Slow-Motion (Bullet Time), Hit Stop hoặc Fast Forward.")]
    public class SetTimeScaleCurve : ActionTask
    {
        [Tooltip("Thời gian thực hiện quá trình thay đổi TimeScale (tính bằng thời gian thực unsclaled time).")]
        public BBParameter<float> duration = 1f;

        [Tooltip("Đường cong biểu diễn TimeScale. Trục X là thời gian (0-1), Trục Y là giá trị TimeScale (thường từ 0-1 nhưng có thể lớn hơn).")]
        public BBParameter<AnimationCurve> timeCurve = AnimationCurve.Linear(0, 1, 1, 1);

        [Tooltip("Có chờ cho đến khi thời gian chạy xong (duration) mới chuyển sang Node tiếp theo không?")]
        public bool waitUntilFinished = true;
        
        [Tooltip("Giá trị TimeScale sẽ set lại khi node bị ngắt giữa chừng (interrupt).")]
        public float defaultTimeScaleOnStop = 1f;

        private float elapsedTime = 0f;

        protected override string info => $"Curve TimeScale over {duration.value}s";

        protected override void OnExecute()
        {
            if (duration.value <= 0f)
            {
                EndAction(true);
                return;
            }
            elapsedTime = 0f;
            
            // Đặt TimeScale ngay frame đầu tiên
            Time.timeScale = timeCurve.value.Evaluate(0f);
            
            // Nếu không cần chờ thì hoàn thành node lập tức
            if (!waitUntilFinished)
            {
                EndAction(true);
            }
        }

        protected override void OnUpdate()
        {
            if (!waitUntilFinished) return; // Nếu không chờ thì Update sẽ không chạy vì node đã Ended

            // Dùng unscaledDeltaTime để thời gian trôi của effect không bị ảnh hưởng bởi chính TimeScale
            elapsedTime += Time.unscaledDeltaTime;
            
            float t = elapsedTime / duration.value;
            Time.timeScale = timeCurve.value.Evaluate(t);
            
            if (elapsedTime >= duration.value)
            {
                EndAction(true);
            }
        }

        // Khi node bị ngắt hoặc kết thúc
        protected override void OnStop()
        {
            // Reset TimeScale để game không bị lỗi kẹt Slow-motion nếu behavior tree bị chuyển state
            Time.timeScale = defaultTimeScaleOnStop;
        }
    }
}
