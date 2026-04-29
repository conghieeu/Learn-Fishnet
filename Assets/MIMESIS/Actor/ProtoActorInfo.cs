using Newtonsoft.Json;
using UnityEngine;

public struct ProtoActorInfo
{
	public string ActorName;

	public long UID;

	public bool UseLight;

	public int Area;

	public TargetActorInfo[] TargetActorArray;

	[JsonIgnore]
	public Vector3 position;

	[JsonIgnore]
	public Vector3 rotation;

	public RaySensorHitResult[] raySensorResults;

	public float distanceToTargetTooFar;

	public float distnaceToTargetTooNear;

	public Vector3 futureRot;

	public float[] muFromPrevOutput;

	public float[] logvarFromPrevOutput;

	[JsonProperty("Position")]
	public object PositionForSerialization => new { position.x, position.y, position.z };

	[JsonProperty("Rotation")]
	public object RotationForSerialization => new { rotation.x, rotation.y, rotation.z };
}
