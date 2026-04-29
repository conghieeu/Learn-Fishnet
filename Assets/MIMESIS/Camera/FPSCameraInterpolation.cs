using UnityEngine;

public class FPSCameraInterpolation : MonoBehaviour
{
	[SerializeField]
	private AnimationCurve yShiftCurve;

	[SerializeField]
	private AnimationCurve zShiftCurve;

	[SerializeField]
	private GameObject cameraRoot;

	private float yOrigin;

	private float zOrigin;

	private float prevXRotation;

	private float maxXRotation = 80f;

	private float prevRatio;

	private void Start()
	{
		if (cameraRoot != null)
		{
			yOrigin = cameraRoot.transform.localPosition.y;
			zOrigin = cameraRoot.transform.localPosition.z;
			prevXRotation = cameraRoot.transform.localEulerAngles.x;
		}
	}

	private void Update()
	{
		if (!(cameraRoot != null))
		{
			return;
		}
		float num = cameraRoot.transform.localEulerAngles.x % 360f;
		if (num > 180f)
		{
			num -= 360f;
		}
		if (num != prevXRotation)
		{
			float num2 = num / maxXRotation;
			if (prevRatio != num2)
			{
				num2 = Mathf.Clamp(num2, 0f, 1f);
				float y = yOrigin + yShiftCurve.Evaluate(num2);
				float z = zOrigin + zShiftCurve.Evaluate(num2);
				cameraRoot.transform.localPosition = new Vector3(cameraRoot.transform.localPosition.x, y, z);
				prevRatio = num2;
			}
			prevXRotation = num;
		}
	}
}
