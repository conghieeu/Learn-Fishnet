using Mimic.Actors;
using UnityEngine;

public class OvelayText : MonoBehaviour
{
	public Transform characterHead;

	public RectTransform debugText;

	private float activeTimer;

	private void Start()
	{
	}

	private void Update()
	{
		GamePlayScene gamePlayScene = Hub.s.pdata.main as GamePlayScene;
		if (gamePlayScene == null || !gamePlayScene.DlHelper.ShowVoiceType)
		{
			return;
		}
		ProtoActor myAvatar = Hub.s.pdata.main.GetMyAvatar();
		ProtoActor component = characterHead.GetComponent<ProtoActor>();
		if (myAvatar != null && characterHead.gameObject == myAvatar.gameObject)
		{
			debugText.anchoredPosition = new Vector3((float)(-Screen.width) / 2f + debugText.sizeDelta.x / 2f, (float)(-Screen.height) / 2f + debugText.sizeDelta.y / 2f);
		}
		else if (component != null && Hub.s.cameraman.IsCurrentSpectatorTarget(component))
		{
			debugText.anchoredPosition = new Vector3((float)(-Screen.width) / 2f + debugText.sizeDelta.x / 2f, (float)(-Screen.height) / 2f + debugText.sizeDelta.y / 2f);
		}
		else
		{
			Vector3 position = Camera.main.WorldToScreenPoint(characterHead.position + new Vector3(0f, 1.5f, 0f));
			debugText.position = position;
		}
		if (debugText.gameObject.activeSelf)
		{
			activeTimer += Time.deltaTime;
			if (activeTimer >= 3f)
			{
				debugText.gameObject.SetActive(value: false);
				activeTimer = 0f;
			}
		}
		else
		{
			activeTimer = 0f;
		}
	}
}
