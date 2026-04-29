using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "_Mimic/GameConfig", order = 0)]
[HelpURL("https://wiki.krafton.com/pages/viewpage.action?pageId=4955177063")]
public class GameConfig : ScriptableObject
{
	[Serializable]
	public class PlayerActor
	{
		[Header("최대 체력")]
		public float maxHP = 100f;

		public float mouseSensitivity = 0.05f;

		[Header("캐릭터의 시야 제한 - 올려다보기\n값이 작을 수록 많이올라감")]
		public float rotXMin = -80f;

		[Header("캐릭터의 시야 제한 - 내려다보기\n값이 클 수록 많이내려감")]
		public float rotXMax = 70f;

		[Header("허리 꺾기 기능 사용 여부")]
		public bool enableSpineRot;

		[Header("캐릭터의 허리 꺾임 제한 - 올려다보기\n값이 작을 수록 많이올라감")]
		[Range(0f, -90f)]
		public float spineRotXMin = -90f;

		[Header("캐릭터의 허리 꺾임 제한 - 내려다보기\n값이 클 수록 많이내려감")]
		[Range(0f, 90f)]
		public float spineRotXMax = 20f;

		[SerializeField]
		public bool btDebugEnable;

		public float rotXInterpolationSpeed = 4f;

		public float rotYInterpolationSpeed = 4f;

		[Header("내 player actor 의 좌표를 서버로 보내는 빈도수. 초당 보내는 횟수")]
		public float sendPositionFrequency = 10f;

		[Header("inventory 관련")]
		public int maxGenericInventorySlot = 4;

		public float maxMapObjectInteractionDistance = 2f;

		[Header("죽었을 때 카메라 전환에 사용되는 시네머신 카메라가 위치한 소켓 이름")]
		public string deadCameraSocketName = "socket_dead_camera";

		[Header("죽었을 때 카메라 전환 후 DIED 팝업이 뜨기까지 대기 시간")]
		public float deadCameraDuration = 5f;

		[Header("죽었을 때 카메라 전환에 소요되는 블렌딩 시간")]
		public float deadCameraBlendTime = 0.1f;

		[Header("오염도 사망 연출시 페이드 아웃 전환 시간(초)")]
		public float deadByContaFadeOutTime = 0.1f;

		[Header("오염도 사망 연출시 페이드 아웃이 완전히 끝난 후 다시 페이드 인을 시작하기 까지 시간(초)")]
		public float deadByContaFadeInterval = 2.9f;

		[Header("오염도 사망 연출시 페이드 인 전환 시간(초)")]
		public float deadByContaFadeInTime = 0.3f;

		public bool BTDebugEnable => btDebugEnable;

		public float deadCameraTotalDuration => deadCameraDuration + deadCameraBlendTime + deadByContaFadeOutTime + deadByContaFadeInterval + deadByContaFadeInTime;
	}

	[Serializable]
	public class GameSetting
	{
		[Header("볼륨 기본값")]
		public float defaultMasterVolume = 0.5f;

		[Header("볼륨 최소값")]
		public float minMasterVolume;

		[Header("볼륨 최대값")]
		public float maxMasterVolume = 1f;

		[Header("마이크 볼륨 기본값")]
		public float defaultMicVolume = 0.5f;

		[Header("마이크 볼륨 최소값")]
		public float minMicVolume;

		[Header("마이크 볼륨 최대값")]
		public float maxMicVolume = 1f;

		[Header("마우스 감도 기본값")]
		public float defaultMouseSensitivity = 0.5f;

		[Header("마우스 감도 최소값")]
		public float minMouseSensitivity;

		[Header("마우스 감도 최대값")]
		public float maxMouseSensitivity = 1f;

		[Header("밝기 기본값")]
		public float defaultBrightness = 0.5f;

		[Header("밝기 최소값")]
		public float minBrightness;

		[Header("밝기 최대값")]
		public float maxBrightness = 1f;

		[Header("감마 기본값")]
		public float defaultGamma = 0.5f;

		[Header("감마 최소값")]
		public float minGamma;

		[Header("감마 최대값")]
		public float maxGamma = 1f;

		[Header("FrameRate 기본값")]
		public int defaultFrameRate = 60;

		[Header("디스플레이 모드 기본값")]
		public FullScreenMode defaultFullScreenMode;

		[Header("호스트 서버 포트")]
		public int ServerPort = 22220;
	}

	public PlayerActor playerActor;

	public GameSetting gameSetting;
}
