using UnityEngine;

public class CamSpring : MonoBehaviour
{
	public float rotStickiness = 0.5f;

	public float posStickiness = 0.5f;

	public Transform moundNode;

	private void Start()
	{
	}

	private void Update()
	{
		if (!(moundNode == null))
		{
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, moundNode.rotation, rotStickiness * Time.deltaTime);
			base.transform.position = Vector3.Lerp(base.transform.position, moundNode.position, posStickiness * Time.deltaTime);
		}
	}
}
