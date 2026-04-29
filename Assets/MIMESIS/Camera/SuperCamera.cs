using UnityEngine;

public class SuperCamera : MonoBehaviour
{
	public GameObject pivot;

	public KeyCode resetShortcut = KeyCode.Space;

	[Range(0f, 100f)]
	public float rotationSensibility = 10f;

	public bool invertRotationX;

	public bool invertRotationY;

	[Range(0f, 100f)]
	public float translationSensibility = 10f;

	public bool invertTranslationX;

	public bool invertTranslationY;

	public float zoomMax = 2f;

	public float zoomMin = 20f;

	[Range(0f, 100f)]
	public float wheelSensibility = 10f;

	private float delayDoubleClic = 0.2f;

	private Vector3 oldCamPos;

	private Quaternion oldCamRot;

	private Vector3 oldMousePos;

	private float timeDoubleClic;

	private bool firstClic;

	private Vector3 pivotPos;

	private void Start()
	{
		pivotPos = pivot.transform.position;
		oldCamPos = Camera.main.transform.position;
		oldCamRot = Camera.main.transform.rotation;
	}

	private void Update()
	{
		Debug.DrawRay(pivotPos, Vector3.up, Color.red);
		Debug.DrawRay(pivotPos, Camera.main.transform.right, Color.green);
		if (Input.GetKeyDown(resetShortcut))
		{
			Camera.main.transform.position = oldCamPos;
			Camera.main.transform.rotation = oldCamRot;
		}
		float axis = Input.GetAxis("Mouse ScrollWheel");
		if (axis != 0f)
		{
			Vector3 vector = pivotPos - Camera.main.transform.position;
			vector.Normalize();
			vector *= axis / 20f * wheelSensibility;
			Vector3 vector2 = Camera.main.transform.position + vector;
			if ((vector2 - pivotPos).magnitude >= zoomMax && (vector2 - pivotPos).magnitude <= zoomMin)
			{
				Camera.main.transform.position = vector2;
			}
		}
		bool flag = false;
		if (Input.GetMouseButtonDown(0))
		{
			if (firstClic)
			{
				flag = true;
				firstClic = false;
			}
			else
			{
				firstClic = true;
				timeDoubleClic = Time.time;
			}
		}
		if (firstClic && Time.time - timeDoubleClic > delayDoubleClic)
		{
			firstClic = false;
		}
		if (flag)
		{
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hitInfo))
			{
				pivotPos = hitInfo.point;
				Debug.Log(hitInfo.point);
			}
			else
			{
				pivotPos = pivot.transform.position;
				Debug.Log("reset Pivot");
			}
		}
		if (!Input.GetMouseButton(0) && !Input.GetMouseButton(2))
		{
			Cursor.visible = true;
			return;
		}
		Cursor.visible = false;
		Vector3 mousePosition = Input.mousePosition;
		if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(2))
		{
			oldMousePos = mousePosition;
			return;
		}
		if (Input.GetMouseButton(2))
		{
			int num = -1;
			if (invertTranslationX)
			{
				num = 1;
			}
			base.gameObject.transform.Translate(new Vector3((float)num * translationSensibility * (mousePosition.x - oldMousePos.x) / 100f, 0f, 0f));
			num = -1;
			if (invertTranslationY)
			{
				num = 1;
			}
			base.gameObject.transform.Translate(new Vector3(0f, (float)num * translationSensibility * (mousePosition.y - oldMousePos.y) / 100f, 0f));
		}
		else
		{
			int num2 = 1;
			if (invertRotationX)
			{
				num2 = -1;
			}
			base.gameObject.transform.RotateAround(pivotPos, Vector3.up, (float)num2 * rotationSensibility * (mousePosition.x - oldMousePos.x) / 100f);
			num2 = 1;
			if (invertRotationY)
			{
				num2 = -1;
			}
			base.gameObject.transform.RotateAround(pivotPos, Camera.main.transform.right, (float)num2 * (0f - rotationSensibility) * (mousePosition.y - oldMousePos.y) / 100f);
		}
		oldMousePos = mousePosition;
	}
}
