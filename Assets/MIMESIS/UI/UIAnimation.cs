using System;
using UnityEngine;

public class UIAnimation : MonoBehaviour
{
	[SerializeField]
	private RectTransform target;

	[SerializeField]
	private bool position;

	[SerializeField]
	private bool rotation;

	[SerializeField]
	private bool scale;

	[SerializeField]
	private UI_Posotion positionAnimation;

	private Vector2 initialAnchoredPosition;

	private float positionElapsedTime;

	[SerializeField]
	private UI_Rotation rotationAnimation;

	private Vector3 initialEulerAngles;

	private float rotationElapsedTime;

	[SerializeField]
	private UI_Scale scaleAnimation;

	private Vector3 initialScale;

	private float scaleElapsedTime;

	private void Start()
	{
		if (target != null)
		{
			initialAnchoredPosition = target.anchoredPosition;
			initialEulerAngles = target.localEulerAngles;
			initialScale = target.localScale;
		}
	}

	private void Update()
	{
		if (!(target == null) && target.gameObject.activeSelf)
		{
			if (position)
			{
				AnimatePosition();
			}
			if (rotation)
			{
				AnimateRotation();
			}
			if (scale)
			{
				AnimateScale();
			}
		}
	}

	private void AnimatePosition()
	{
		positionElapsedTime += Time.deltaTime;
		Vector2 zero = Vector2.zero;
		if (!positionAnimation.Loop)
		{
			if (positionAnimation.xDistance != 0f && positionAnimation.xTime > 0f)
			{
				float t = Mathf.Clamp01(positionElapsedTime / positionAnimation.xTime);
				zero.x = Mathf.Lerp(0f, positionAnimation.xDistance, t);
			}
			if (positionAnimation.yDistance != 0f && positionAnimation.yTime > 0f)
			{
				float t2 = Mathf.Clamp01(positionElapsedTime / positionAnimation.yTime);
				zero.y = Mathf.Lerp(0f, positionAnimation.yDistance, t2);
			}
			target.anchoredPosition = initialAnchoredPosition + zero;
			bool num = positionAnimation.xDistance == 0f || positionAnimation.xTime <= 0f || positionElapsedTime >= positionAnimation.xTime;
			bool flag = positionAnimation.yDistance == 0f || positionAnimation.yTime <= 0f || positionElapsedTime >= positionAnimation.yTime;
			if (num && flag)
			{
				position = false;
			}
		}
		else
		{
			float x = 0f;
			float y = 0f;
			if (positionAnimation.xDistance != 0f && positionAnimation.xTime > 0f)
			{
				float num2 = Mathf.Sin(MathF.PI * 2f / positionAnimation.xTime * positionElapsedTime) * 0.5f + 0.5f;
				x = positionAnimation.xDistance * num2;
			}
			if (positionAnimation.yDistance != 0f && positionAnimation.yTime > 0f)
			{
				float num3 = Mathf.Sin(MathF.PI * 2f / positionAnimation.yTime * positionElapsedTime) * 0.5f + 0.5f;
				y = positionAnimation.yDistance * num3;
			}
			target.anchoredPosition = initialAnchoredPosition + new Vector2(x, y);
		}
	}

	private void AnimateRotation()
	{
		if (rotationAnimation.zAngle == 0f)
		{
			return;
		}
		rotationElapsedTime += Time.deltaTime;
		float num = 0f;
		if (rotationAnimation.repeat)
		{
			num = rotationAnimation.zAngle * rotationElapsedTime;
		}
		else if (!rotationAnimation.Loop)
		{
			if (rotationAnimation.zTime <= 0f)
			{
				return;
			}
			float t = Mathf.Clamp01(rotationElapsedTime / rotationAnimation.zTime);
			num = Mathf.Lerp(0f, rotationAnimation.zAngle, t);
		}
		else
		{
			if (rotationAnimation.zTime <= 0f)
			{
				return;
			}
			float num2 = Mathf.Sin(MathF.PI * 2f / rotationAnimation.zTime * rotationElapsedTime) * 0.5f + 0.5f;
			num = rotationAnimation.zAngle * num2;
		}
		target.localEulerAngles = new Vector3(initialEulerAngles.x, initialEulerAngles.y, initialEulerAngles.z + num);
		if (rotationAnimation.Loop)
		{
			return;
		}
		if (rotationAnimation.repeat)
		{
			if (rotationElapsedTime >= 1f)
			{
				rotation = false;
				target.localEulerAngles = initialEulerAngles;
			}
		}
		else if (rotationElapsedTime >= rotationAnimation.zTime)
		{
			rotation = false;
		}
	}

	private void AnimateScale()
	{
		if (!(scaleAnimation.scaleTime <= 0f))
		{
			scaleElapsedTime += Time.deltaTime;
			float num = 0f;
			Vector3 b = Vector3.Lerp(t: scaleAnimation.Loop ? (Mathf.Sin(MathF.PI * 2f / scaleAnimation.scaleTime * scaleElapsedTime) * 0.5f + 0.5f) : Mathf.Clamp01(scaleElapsedTime / scaleAnimation.scaleTime), a: Vector3.one, b: scaleAnimation.scaleAmount);
			target.localScale = Vector3.Scale(initialScale, b);
			if (!scaleAnimation.Loop && scaleElapsedTime >= scaleAnimation.scaleTime)
			{
				scale = false;
			}
		}
	}
}
