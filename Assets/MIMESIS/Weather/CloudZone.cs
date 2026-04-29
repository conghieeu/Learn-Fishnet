using UnityEngine;

public class CloudZone : MonoBehaviour
{
	public enum eType
	{
		Linear = 0,
		Radial = 1,
		SingleMesh = 2
	}

	[SerializeField]
	public eType type;

	[SerializeField]
	public Bounds zone;

	[SerializeField]
	public GameObject[] cloudPrefab;

	[SerializeField]
	public float cloudScale = 10f;

	[SerializeField]
	public int cloudCount = 10;

	[SerializeField]
	public GameObject[] clouds;

	[SerializeField]
	private float windSpeed = 10f;

	[SerializeField]
	private float rotationSpeed = 10f;

	[ContextMenu("Create Cloud")]
	public void CreateCloud()
	{
		if (clouds != null && clouds.Length != 0)
		{
			GameObject[] array = clouds;
			foreach (GameObject gameObject in array)
			{
				if (gameObject != null)
				{
					Object.DestroyImmediate(gameObject);
				}
			}
		}
		clouds = new GameObject[cloudCount];
		for (int j = 0; j < cloudCount; j++)
		{
			if (cloudPrefab == null || cloudPrefab.Length == 0)
			{
				Debug.LogWarning("CloudZone: cloudPrefab is empty or null!");
				return;
			}
			GameObject original = cloudPrefab[Random.Range(0, cloudPrefab.Length)];
			Vector3 vector = new Vector3(Random.Range(0f - zone.extents.x, zone.extents.x), Random.Range(0f - zone.extents.y, zone.extents.y), Random.Range(0f - zone.extents.z, zone.extents.z));
			Vector3 position = zone.center + vector;
			Vector3 position2 = base.transform.TransformPoint(position);
			GameObject gameObject2 = Object.Instantiate(original, position2, Quaternion.identity, base.transform);
			gameObject2.transform.localRotation = Quaternion.Euler(0f, Random.Range(0, 360), 0f);
			gameObject2.transform.localScale = Vector3.one * cloudScale;
			gameObject2.name = $"Cloud_{j}";
			clouds[j] = gameObject2;
		}
		Shader shader = clouds[0].GetComponent<Renderer>().sharedMaterial.shader;
		for (int k = 0; k < shader.GetPropertyCount(); k++)
		{
			Debug.Log($"Property [{k}]: {shader.GetPropertyName(k)} - {shader.GetPropertyType(k)}");
		}
		Debug.Log($"{cloudCount} clouds created within the zone.");
	}

	[ContextMenu("Clear")]
	public void Clear()
	{
		if (clouds != null && clouds.Length != 0)
		{
			GameObject[] array = clouds;
			foreach (GameObject gameObject in array)
			{
				if (gameObject != null)
				{
					Object.DestroyImmediate(gameObject);
				}
			}
		}
		clouds = null;
	}

	private void Update()
	{
		if (clouds != null && clouds.Length != 0)
		{
			if (type == eType.Linear)
			{
				UpdateLinearCloud();
			}
			else if (type == eType.Radial)
			{
				UpdateRadialCloud();
			}
			else if (type == eType.SingleMesh)
			{
				UpdateSingleMeshCloud();
			}
		}
	}

	private void UpdateLinearCloud()
	{
		Vector3 vector = base.transform.TransformDirection(Vector3.forward);
		GameObject[] array = clouds;
		foreach (GameObject gameObject in array)
		{
			if (!(gameObject == null))
			{
				gameObject.transform.position += vector * windSpeed * Time.deltaTime;
				Vector3 vector2 = base.transform.InverseTransformPoint(gameObject.transform.position) - zone.center;
				if (vector2.z > zone.extents.z)
				{
					Vector3 vector3 = new Vector3(vector2.x, vector2.y, 0f - zone.extents.z);
					gameObject.transform.position = base.transform.TransformPoint(zone.center + vector3);
				}
				else if (vector2.z < 0f - zone.extents.z)
				{
					Vector3 vector4 = new Vector3(vector2.x, vector2.y, zone.extents.z);
					gameObject.transform.position = base.transform.TransformPoint(zone.center + vector4);
				}
			}
		}
	}

	private void UpdateRadialCloud()
	{
		base.transform.localRotation = Quaternion.AngleAxis(rotationSpeed * Time.time, Vector3.up);
	}

	private void UpdateSingleMeshCloud()
	{
	}

	public void ApplySkyAndWeather(Color skycolor, Color fakeAmbientColor)
	{
		if (clouds == null || clouds.Length == 0)
		{
			return;
		}
		GameObject[] array = clouds;
		foreach (GameObject gameObject in array)
		{
			if (gameObject == null)
			{
				continue;
			}
			Material material = null;
			Renderer component = gameObject.GetComponent<Renderer>();
			if (component != null)
			{
				Material sharedMaterial = component.sharedMaterial;
				if (sharedMaterial != null && material != sharedMaterial)
				{
					sharedMaterial.SetColor("_fakeAmbient", fakeAmbientColor);
				}
				material = sharedMaterial;
			}
		}
	}
}
