using UnityEngine;

namespace DLAgent
{
	public class DLAgentMovementOutput
	{
		public Vector3? worldPosDiff;

		public float? yawDiff;

		public float? pitchDiff;

		public bool? isRunning;

		public bool? calcAngle;

		public bool isConsumed;

		public void Reset()
		{
			worldPosDiff = Vector3.zero;
			yawDiff = 0f;
			pitchDiff = 0f;
			isRunning = false;
			calcAngle = false;
			isConsumed = false;
		}
	}
}
