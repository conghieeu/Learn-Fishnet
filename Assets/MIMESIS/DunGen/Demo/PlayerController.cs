using System.Linq;
using System.Text;
using UnityEngine;

namespace DunGen.Demo
{
	[RequireComponent(typeof(CharacterController))]
	public class PlayerController : MonoBehaviour
	{
		public float MinYaw = -360f;

		public float MaxYaw = 360f;

		public float MinPitch = -60f;

		public float MaxPitch = 60f;

		public float LookSensitivity = 1f;

		public float MoveSpeed = 10f;

		public float TurnSpeed = 90f;

		protected CharacterController movementController;

		protected Camera playerCamera;

		protected Camera overheadCamera;

		protected bool isControlling;

		protected float yaw;

		protected float pitch;

		protected Generator gen;

		protected Vector3 velocity;

		public bool IsControlling => isControlling;

		public Camera ActiveCamera
		{
			get
			{
				if (!isControlling)
				{
					return overheadCamera;
				}
				return playerCamera;
			}
		}

		protected virtual void Start()
		{
			movementController = GetComponent<CharacterController>();
			playerCamera = GetComponentInChildren<Camera>();
			gen = UnityUtil.FindObjectByType<Generator>();
			overheadCamera = GameObject.Find("Overhead Camera").GetComponent<Camera>();
			isControlling = true;
			ToggleControl();
			gen.DungeonGenerator.Generator.OnGenerationStatusChanged += OnGenerationStatusChanged;
			gen.GetAdditionalText = GetAdditionalScreenText;
		}

		protected virtual void OnDestroy()
		{
			gen.DungeonGenerator.Generator.OnGenerationStatusChanged -= OnGenerationStatusChanged;
			gen.GetAdditionalText = null;
		}

		private void GetAdditionalScreenText(StringBuilder infoText)
		{
			infoText.AppendLine("Press 'C' to switch between camera modes");
		}

		protected virtual void OnGenerationStatusChanged(DungeonGenerator generator, GenerationStatus status)
		{
			if (status == GenerationStatus.Complete)
			{
				FrameDungeonWithCamera();
				base.transform.position = new Vector3(0f, 1f, 7f);
				velocity = Vector3.zero;
			}
		}

		protected virtual void Update()
		{
			if (Input.GetKeyDown(KeyCode.C))
			{
				ToggleControl();
			}
			DungeonGenerator generator = gen.DungeonGenerator.Generator;
			if (generator.IsGenerating && generator.GenerateAsynchronously && generator.PauseBetweenRooms > 0f)
			{
				FrameDungeonWithCamera();
			}
			if (isControlling)
			{
				Vector3 zero = Vector3.zero;
				zero += base.transform.forward * Input.GetAxisRaw("Vertical");
				zero += base.transform.right * Input.GetAxisRaw("Horizontal");
				zero.Normalize();
				if (movementController.isGrounded)
				{
					velocity = Vector3.zero;
				}
				else
				{
					velocity += -base.transform.up * 98.100006f * Time.deltaTime;
				}
				zero += velocity * Time.deltaTime;
				movementController.Move(zero * Time.deltaTime * MoveSpeed);
				yaw += Input.GetAxisRaw("Mouse X") * LookSensitivity;
				pitch += Input.GetAxisRaw("Mouse Y") * LookSensitivity;
				yaw = ClampAngle(yaw, MinYaw, MaxYaw);
				pitch = ClampAngle(pitch, MinPitch, MaxPitch);
				base.transform.rotation = Quaternion.AngleAxis(yaw, Vector3.up);
				playerCamera.transform.localRotation = Quaternion.AngleAxis(pitch, -Vector3.right);
			}
		}

		protected float ClampAngle(float angle)
		{
			return ClampAngle(angle, 0f, 360f);
		}

		protected float ClampAngle(float angle, float min, float max)
		{
			if (angle < -360f)
			{
				angle += 360f;
			}
			if (angle > 360f)
			{
				angle -= 360f;
			}
			return Mathf.Clamp(angle, min, max);
		}

		protected void ToggleControl()
		{
			isControlling = !isControlling;
			overheadCamera.gameObject.SetActive(!isControlling);
			playerCamera.gameObject.SetActive(isControlling);
			overheadCamera.transform.position = new Vector3(base.transform.position.x, overheadCamera.transform.position.y, base.transform.position.z);
			Cursor.lockState = (isControlling ? CursorLockMode.Locked : CursorLockMode.None);
			Cursor.visible = !isControlling;
			if (!isControlling)
			{
				FrameDungeonWithCamera();
			}
		}

		protected void FrameDungeonWithCamera()
		{
			GameObject[] gameObjects = (from x in UnityUtil.FindObjectsByType<Dungeon>()
				select x.gameObject).ToArray();
			FrameObjectsWithCamera(gameObjects);
		}

		protected void FrameObjectsWithCamera(params GameObject[] gameObjects)
		{
			if (gameObjects == null || gameObjects.Length == 0)
			{
				return;
			}
			bool flag = false;
			Bounds bounds = default(Bounds);
			for (int i = 0; i < gameObjects.Length; i++)
			{
				Bounds bounds2 = UnityUtil.CalculateObjectBounds(gameObjects[i], includeInactive: false, ignoreSpriteRenderers: false);
				if (!flag)
				{
					bounds = bounds2;
					flag = true;
				}
				else
				{
					bounds.Encapsulate(bounds2);
				}
			}
			if (flag)
			{
				float f = Mathf.Max(bounds.size.x, bounds.size.z) / Mathf.Sin(overheadCamera.fieldOfView / 2f);
				f = Mathf.Abs(f);
				Vector3 position = new Vector3(bounds.center.x, bounds.center.y, bounds.center.z);
				position += gen.DungeonGenerator.Generator.UpVector * f;
				overheadCamera.transform.position = position;
			}
		}
	}
}
