using UnityEngine;

namespace MellowAbelson.AI.Utilities
{
    /// <summary>
    /// Thay đổi Time.timeScale mượt mà theo AnimationCurve.
    /// Dùng cho slow-motion, bullet time, hit stop.
    /// </summary>
    public class SetTimeScaleCurve
    {
        private readonly AnimationCurve _curve;
        private readonly float _duration;
        private readonly float _defaultTimeScale;
        private float _elapsedTime;

        public bool IsFinished => _elapsedTime >= _duration;

        public float CurrentTimeScale { get; private set; }

        public SetTimeScaleCurve(AnimationCurve curve, float duration, float defaultTimeScale = 1f)
        {
            _curve = curve ?? AnimationCurve.Linear(0, 1, 1, 1);
            _duration = Mathf.Max(0.01f, duration);
            _defaultTimeScale = defaultTimeScale;
            _elapsedTime = 0f;
            CurrentTimeScale = 1f;
        }

        /// <summary>
        /// Gọi mỗi frame. Trả về true nếu còn chạy, false nếu đã xong.
        /// </summary>
        public bool Update()
        {
            _elapsedTime += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(_elapsedTime / _duration);
            CurrentTimeScale = _curve.Evaluate(t);
            Time.timeScale = CurrentTimeScale;

            if (_elapsedTime >= _duration)
            {
                Stop();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Reset TimeScale về mặc định.
        /// </summary>
        public void Stop()
        {
            Time.timeScale = _defaultTimeScale;
            CurrentTimeScale = _defaultTimeScale;
        }

        /// <summary>
        /// Reset effect về đầu.
        /// </summary>
        public void Restart()
        {
            _elapsedTime = 0f;
        }
    }
}
