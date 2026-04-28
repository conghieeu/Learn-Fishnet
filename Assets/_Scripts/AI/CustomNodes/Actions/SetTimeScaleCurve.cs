using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace MellowAbelson.AI.CustomNodes.Actions
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
            Time.timeScale = timeCurve.value.Evaluate(0f);

            if (!waitUntilFinished)
                EndAction(true);
        }

        protected override void OnUpdate()
        {
            if (!waitUntilFinished) return;

            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / duration.value;
            Time.timeScale = timeCurve.value.Evaluate(t);

            if (elapsedTime >= duration.value)
                EndAction(true);
        }

        protected override void OnStop()
        {
            Time.timeScale = defaultTimeScaleOnStop;
        }
    }
}
