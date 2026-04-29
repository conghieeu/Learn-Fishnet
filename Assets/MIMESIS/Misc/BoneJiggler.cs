using UnityEngine;

public class BoneJiggler : MonoBehaviour
{
	[Header("노드들 지정. 최소 2개. 연속일 필요 없음. 반드시 0번 bone 의 하위여야 함")]
	public Transform[] nodes;

	[Header("물리 파라미터")]
	[Range(-50f, 50f)]
	public float scaleStart = 1f;

	[Range(-50f, 50f)]
	public float scaleEnd = 1f;

	[Range(0.01f, 200f)]
	public float speed = 1f;

	[Header("이 속도 이상일 때 발동")]
	public float triggerVelocityStart = 1f;

	[Header("이 속도 이상은 같은 움직임")]
	public float triggerVelocityEnd = 1f;

	[Header("지속시간")]
	[Range(0.1f, 10f)]
	public float sustaine = 1f;

	private float strength;

	private Vector3[] prevPositions;

	private float[] lengths;

	private Quaternion[] initialLocalRotations;

	private Vector3 axis;

	private Vector3 newAxis;

	private float startTime;

	private void Start()
	{
		if (nodes == null || nodes.Length < 2)
		{
			Logger.RError("ReluBoneJiggler: 최소 2개 노드를 지정하세요.");
			base.enabled = false;
			return;
		}
		prevPositions = new Vector3[2];
		initialLocalRotations = new Quaternion[nodes.Length];
		lengths = new float[nodes.Length - 1];
		for (int i = 0; i < nodes.Length; i++)
		{
			if (nodes[i] != null)
			{
				if (i < 2)
				{
					prevPositions[i] = nodes[i].position;
				}
				initialLocalRotations[i] = nodes[i].localRotation;
				continue;
			}
			Logger.RError($"ReluBoneJiggler: 노드 {i}가 null입니다.");
			base.enabled = false;
			return;
		}
		for (int j = 0; j < nodes.Length - 1; j++)
		{
			lengths[j] = nodes[j + 1].localPosition.magnitude;
		}
	}

	private void LateUpdate()
	{
		if (nodes == null || nodes.Length < 2)
		{
			return;
		}
		Vector3 vector = nodes[0].position - prevPositions[0];
		if (vector.magnitude / Time.deltaTime > triggerVelocityStart)
		{
			if (strength <= 0f)
			{
				startTime = Time.time;
			}
			float b = Mathf.Clamp01((vector.magnitude / Time.deltaTime - triggerVelocityStart) / (triggerVelocityEnd - triggerVelocityStart));
			strength = Mathf.Max(strength, b);
			Vector3 position = nodes[0].position;
			Vector3 position2 = nodes[1].position;
			Vector3 vector2 = prevPositions[1];
			if (Vector3.Distance(position2, vector2) > 0.01f)
			{
				newAxis = Vector3.Cross(position2 - position, vector2 - position);
			}
		}
		else
		{
			strength = Mathf.Max(0f, strength - Time.deltaTime / sustaine);
		}
		prevPositions[0] = nodes[0].position;
		prevPositions[1] = nodes[1].position;
		if (strength > 0f)
		{
			axis = Vector3.Slerp(axis, newAxis, 0.2f);
			float num = Mathf.Sin((Time.time - startTime) * speed) * strength;
			for (int i = 1; i < nodes.Length; i++)
			{
				nodes[i].localRotation = initialLocalRotations[i];
				float num2 = Mathf.Lerp(scaleStart, scaleEnd, (float)i / (float)(nodes.Length - 1));
				float angle = num * num2;
				nodes[i].Rotate(axis.normalized, angle);
			}
		}
	}
}
