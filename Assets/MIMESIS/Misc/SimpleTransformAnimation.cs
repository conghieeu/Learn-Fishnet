using UnityEngine;

[ExecuteInEditMode]
public class SimpleTransformAnimation : MonoBehaviour
{
	[SerializeField]
	private bool testMode;

	[SerializeField]
	private float rotY;

	[SerializeField]
	private float sinMoveYFreq = 1f;

	[SerializeField]
	private float sinMoveYAmp;

	private Vector3 initialPosition;

	private Quaternion initialRotation;

	private float startTime;

	private void Awake()
	{
		initialPosition = base.transform.localPosition;
		initialRotation = base.transform.localRotation;
		startTime = Time.time;
	}

	private void Update()
	{
		Animate();
	}

	private void Animate()
	{
		base.transform.localRotation = initialRotation * Quaternion.Euler(0f, rotY * (Time.time - startTime), 0f);
		if (sinMoveYAmp != 0f)
		{
			float y = Mathf.Sin((Time.time - startTime) * sinMoveYFreq) * sinMoveYAmp;
			base.transform.localPosition = initialPosition + new Vector3(0f, y, 0f);
		}
	}
}
