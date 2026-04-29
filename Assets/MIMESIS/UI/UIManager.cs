using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Mimic.Actors;
using Mimic.Audio;
using Mimic.InputSystem;
using ModUtility;
using ReluReplay.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[HelpURL("https://wiki.krafton.com/pages/viewpage.action?pageId=4648685001")]
public class UIManager : MonoBehaviour
{
	public enum eUIEffectType
	{
		None = 0,
		FadeIn = 1,
		FadeOut = 2,
		PingPong = 3
	}

	[SerializeField]
	private Canvas canvas;

	[SerializeField]
	private Transform[] nodes;

	[SerializeField]
	private Image fadeInImage;

	[SerializeField]
	private Image grappledImage;

	[SerializeField]
	private Image underlayUIEffectImage;

	[SerializeField]
	private TMP_Text fpsText;

	[SerializeField]
	public GameObject prefab_ui_gametips;

	[SerializeField]
	private BlinkAnimation blinkAnimation;

	[SerializeField]
	private ScriptableUIUtility scriptableUIPanel;

	[SerializeField]
	private Transform anitiSickness;

	[SerializeField]
	private Transform rainScreenVFX;

	[Header("UI prefabs")]
	[SerializeField]
	private GameObject prefab_dialogueBox;

	[SerializeField]
	public GameObject prefab_titleBG;

	[SerializeField]
	public GameObject prefab_tos;

	[SerializeField]
	public GameObject prefab_pp;

	[SerializeField]
	public GameObject prefab_AgeWarnging;

	[SerializeField]
	public GameObject prefab_GameSettings;

	[SerializeField]
	public GameObject prefab_ChangeKeyBinding;

	[SerializeField]
	public GameObject prefab_ChangeKeyBindingGamepad;

	[SerializeField]
	public GameObject prefab_InGameMenu;

	[SerializeField]
	public GameObject prefab_WeathweForecast;

	[SerializeField]
	public GameObject prefab_RetrunToMainMenu;

	[SerializeField]
	public GameObject prefab_Invite_Loading;

	[SerializeField]
	public GameObject prefab_GamepadCursor;

	[SerializeField]
	public GameObject prefab_GamepadEmote;

	[SerializeField]
	private Transform videoPlayLayer;

	[SerializeField]
	private RawImage rawImageForVideoCutScene;

	[SerializeField]
	public GameObject prefab_CreatorCode;

	[SerializeField]
	public GameObject prefab_CheckAgeForVoiceChat;

	[SerializeField]
	public GameObject prefab_PublicRoomList;

	[SerializeField]
	public GameObject prefab_SteamInventory;

	[SerializeField]
	public float MaintenanceFadeInSec;

	[SerializeField]
	public float MaintenanceFadeOutSec;

	[SerializeField]
	public float WaitingRoomFadeInSec;

	[SerializeField]
	public float WaitingRoomFadeOutSec;

	[SerializeField]
	public float InGameFadeInSec;

	[SerializeField]
	public float InGameFadeOutSec;

	[SerializeField]
	public UIPrefab_Scene_Loading ui_sceneloading;

	private GameObject? currnetBG;

	[HideInInspector]
	public UIPrefab_GameTips gametipsui;

	public UnityAction<ProtoActor> OnSetupSpectatorCamera;

	public UnityAction<List<int>, List<int>> OnUpdateSpectatorPlayerList;

	private Dictionary<int, GameObject> dialogues = new Dictionary<int, GameObject>();

	[HideInInspector]
	public UIScreenEffectBase? screenEffect_conta;

	[HideInInspector]
	public bool tos;

	[HideInInspector]
	public bool pp;

	[HideInInspector]
	public int ageCheck;

	[HideInInspector]
	public UIPrefab_InGameMenu inGameMenu;

	[HideInInspector]
	public UIPrefabScript settingsPrevUI;

	[HideInInspector]
	public UIPrefab_GameSettings settingsUI;

	[HideInInspector]
	public UIPrefab_KeyBind keyBindUI;

	[HideInInspector]
	public UIPrefab_KeyBind keyBindGamepadUI;

	[HideInInspector]
	private float dTimeForAgeWarning;

	[HideInInspector]
	public UIPrefab_GamepadEmote gamepadEmoteUI;

	[HideInInspector]
	public UIPrefab_CreatorCode creatorCodeUI;

	[HideInInspector]
	public UIPrefab_PublicRoomList publicRoomListUI;

	[HideInInspector]
	public UIPrefab_SteamInventory steamInventoryUI;

	public Color mouseOverTextColor = new Color(0f, 0f, 0f, 1f);

	private UIPrefabScript tosUI;

	private UIPrefabScript ppUI;

	[HideInInspector]
	public UIPrefab_dialogueBox inviteLoadingUI;

	[Header("Master Audio")]
	[SerializeField]
	private UISfxTable sfxTable;

	[HideInInspector]
	public Dictionary<string, float> tempPlayerVolumeDictionary = new Dictionary<string, float>();

	[HideInInspector]
	public Dictionary<string, bool> tempPlayerVolumeMuteDictionary = new Dictionary<string, bool>();

	private (string, GameObject, float) currentTimerDialog;

	private Queue<(string, GameObject, float)> openWaitingQueue = new Queue<(string, GameObject, float)>();

	private Queue<object[]> openWaitingQueueParams = new Queue<object[]>();

	private Dictionary<string, UIScreenEffectBase> uiScreenEffectDictionary = new Dictionary<string, UIScreenEffectBase>();

	[HideInInspector]
	public bool isGameMenuOpen;

	[HideInInspector]
	public UIPrefab_ReturnToMainMenu retrunToMainMenu;

	[HideInInspector]
	public bool isChangingKeyBind;

	public List<UIPrefabScript> ui_escapeStack = new List<UIPrefabScript>();

	public void PrepareVideoPlay(RenderTexture rt)
	{
		videoPlayLayer.gameObject.SetActive(value: true);
		rawImageForVideoCutScene.texture = rt;
	}

	public void ReleaseVideoPlay()
	{
		videoPlayLayer.gameObject.SetActive(value: false);
		rawImageForVideoCutScene.texture = null;
	}

	public void FadeOut(Color color, float duration)
	{
		_ = fadeInImage.color;
		fadeInImage.color = new Color(color.r, color.g, color.b, 0f);
		Color endValue = new Color(color.r, color.g, color.b, 1f);
		fadeInImage.gameObject.SetActive(value: true);
		fadeInImage.DOColor(endValue, duration).SetEase(Ease.InOutQuad);
	}

	public void FadeIn(float duration)
	{
		Color color = fadeInImage.color;
		DOTweenModuleUI.DOColor(endValue: new Color(color.r, color.g, color.b, 0f), target: fadeInImage, duration: duration).SetEase(Ease.InOutQuad).OnComplete(delegate
		{
			fadeInImage.gameObject.SetActive(value: false);
		});
	}

	public void FadeIn(Color startColor, float duration)
	{
		Color originalColor = fadeInImage.color;
		fadeInImage.color = startColor;
		fadeInImage.gameObject.SetActive(value: true);
		DOTweenModuleUI.DOColor(endValue: new Color(startColor.r, startColor.g, startColor.b, 0f), target: fadeInImage, duration: duration).SetEase(Ease.InOutQuad).OnComplete(delegate
		{
			fadeInImage.gameObject.SetActive(value: false);
			fadeInImage.color = originalColor;
		});
	}

	public void TriggerGrappledUIEffect(float duration, AnimationCurve easeCurve)
	{
		Color color = grappledImage.color;
		color.a = 0f;
		Color endValue = color;
		endValue.a = 1f;
		grappledImage.color = color;
		grappledImage.gameObject.SetActive(value: true);
		grappledImage.DOColor(endValue, duration).SetEase(easeCurve).OnComplete(delegate
		{
			grappledImage.gameObject.SetActive(value: false);
		});
	}

	public void TriggerUnderlayUIEffect(Action<Image> action)
	{
		action?.Invoke(underlayUIEffectImage);
	}

	public void TriggerUnderlayUIEffect(eUIEffectType type, Color color, float duration1, float duration2, float sustain, Sprite? sprite = null)
	{
		underlayUIEffectImage.color = color;
		underlayUIEffectImage.sprite = sprite;
		underlayUIEffectImage.gameObject.SetActive(value: true);
		switch (type)
		{
		default:
			return;
		case eUIEffectType.FadeIn:
			underlayUIEffectImage.DOFade(1f, duration1).From(0f).SetEase(Ease.InOutQuad)
				.OnComplete(delegate
				{
					underlayUIEffectImage.DOFade(1f, sustain).OnComplete(delegate
					{
						underlayUIEffectImage.gameObject.SetActive(value: false);
					});
				});
			break;
		case eUIEffectType.FadeOut:
			underlayUIEffectImage.DOFade(0f, duration1).From(1f).SetEase(Ease.InOutQuad)
				.OnComplete(delegate
				{
					underlayUIEffectImage.DOFade(0f, sustain).OnComplete(delegate
					{
						underlayUIEffectImage.gameObject.SetActive(value: false);
					});
				});
			break;
		case eUIEffectType.PingPong:
			StartCoroutine(CorTriggerUnderlayUIEffect(duration1, duration2, sustain));
			break;
		}
		underlayUIEffectImage.sprite = null;
	}

	private IEnumerator CorTriggerUnderlayUIEffect(float duration1, float duration2, float sustain)
	{
		underlayUIEffectImage.DOFade(1f, duration1).From(0f).SetEase(Ease.InOutQuad);
		yield return new WaitForSeconds(duration1);
		yield return new WaitForSeconds(sustain);
		underlayUIEffectImage.DOFade(0f, duration2).From(1f).SetEase(Ease.InOutQuad)
			.OnComplete(delegate
			{
				underlayUIEffectImage.gameObject.SetActive(value: false);
			});
		yield return new WaitForSeconds(duration2);
	}

	public BlinkAnimation GetBlinkAnimation()
	{
		return blinkAnimation;
	}

	public T InstatiateUI<T>(GameObject prefab, eUIHeight height = eUIHeight.Main)
	{
		int num = (int)height;
		if (num < 0)
		{
			num = 0;
		}
		if (num >= nodes.Length)
		{
			num = nodes.Length - 1;
		}
		return UnityEngine.Object.Instantiate(prefab, nodes[num]).GetComponent<T>();
	}

	public T InstatiateUIPrefab<T>(GameObject prefab, eUIHeight height = eUIHeight.Main) where T : UIPrefabScript
	{
		int num = (int)height;
		if (num < 0)
		{
			num = 0;
		}
		if (num >= nodes.Length)
		{
			num = nodes.Length - 1;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(prefab, nodes[num]);
		dialogues.Add(gameObject.GetInstanceID(), gameObject);
		return gameObject.GetComponent<T>();
	}

	public void ClearUIPrefab(GameObject ui)
	{
		dialogues.Remove(ui.GetInstanceID());
		UnityEngine.Object.Destroy(ui);
	}

	public IEnumerator CorHandleTimerDialogQueue()
	{
		while (true)
		{
			currentTimerDialog = default((string, GameObject, float));
			yield return new WaitUntil(() => openWaitingQueue.Count > 0);
			(string, GameObject, float) tuple = openWaitingQueue.Dequeue();
			if (!tuple.Item2)
			{
				continue;
			}
			UIPrefab_ClosableByTimeBase ui = InstatiateUIPrefab<UIPrefab_ClosableByTimeBase>(tuple.Item2);
			if ((bool)ui)
			{
				currentTimerDialog = tuple;
				if (tuple.Item3 > 0f)
				{
					ui.openingDurationSec = tuple.Item3;
				}
				float num = ui.openingDurationSec;
				if (num <= 0f)
				{
					num = 1f;
				}
				float delaySec = ui.delaySec;
				DateTime timestamp = Hub.s.timeutil.GetTimestamp();
				DateTime delaydt = timestamp.AddSeconds(delaySec);
				DateTime closedt = timestamp.AddSeconds(num);
				object[] parameters = openWaitingQueueParams.Dequeue();
				ui.PatchParameter(parameters);
				yield return new WaitUntil(() => Hub.s.timeutil.GetTimestamp() >= delaydt);
				ui.Show();
				yield return new WaitUntil(() => Hub.s.timeutil.GetTimestamp() >= closedt);
				if (ui != null && ui.gameObject != null)
				{
					UnityEngine.Object.Destroy(ui.gameObject);
				}
				yield return new WaitForSeconds(0.2f);
			}
		}
	}

	public void ShowTimerDialog(string id, GameObject prefab, bool onlyOneAtMoment, float overrideDurationSec = 0f, params object[] parameters)
	{
		if (onlyOneAtMoment)
		{
			var (text, gameObject, num) = currentTimerDialog;
			if (((text != null || gameObject != null || num != 0f) && currentTimerDialog.Item1 == id) || openWaitingQueue.Contains((id, prefab, overrideDurationSec)))
			{
				return;
			}
		}
		openWaitingQueue.Enqueue((id, prefab, overrideDurationSec));
		if (parameters == null || parameters.Length == 0)
		{
			openWaitingQueueParams.Enqueue(new object[0]);
		}
		else
		{
			openWaitingQueueParams.Enqueue(parameters);
		}
	}

	public void CloseAllDialogueBox()
	{
		foreach (KeyValuePair<int, GameObject> dialogue in dialogues)
		{
			if (!(dialogue.Value == null) && dialogue.Value.GetComponent<UIPrefabScript>().dialogue)
			{
				ui_escapeStack.Remove(dialogue.Value.GetComponent<UIPrefabScript>());
				UnityEngine.Object.Destroy(dialogue.Value);
			}
		}
		dialogues.Clear();
	}

	private void Start()
	{
		Logger.RLog("[AwakeLogs] UIManager.Start ->");
		CreateAllSoundGroupCreators();
		UpdateFpsAsync().Forget();
		StartCoroutine(CorHandleTimerDialogQueue());
		if (gametipsui == null)
		{
			gametipsui = Hub.s.uiman.InstatiateUIPrefab<UIPrefab_GameTips>(prefab_ui_gametips);
		}
		gametipsui.dialogue = false;
		gametipsui.Show();
		if (settingsUI == null)
		{
			settingsUI = Hub.s.uiman.InstatiateUIPrefab<UIPrefab_GameSettings>(prefab_GameSettings, eUIHeight.Top);
		}
		settingsUI.dialogue = false;
		if (keyBindUI == null)
		{
			keyBindUI = Hub.s.uiman.InstatiateUIPrefab<UIPrefab_KeyBind>(prefab_ChangeKeyBinding, eUIHeight.Top);
		}
		keyBindUI.dialogue = false;
		if (keyBindGamepadUI == null)
		{
			keyBindGamepadUI = Hub.s.uiman.InstatiateUIPrefab<UIPrefab_KeyBind>(prefab_ChangeKeyBindingGamepad, eUIHeight.Top);
		}
		keyBindGamepadUI.dialogue = false;
		if (ui_sceneloading != null)
		{
			ui_sceneloading.Hide();
		}
		if (fpsText != null)
		{
			fpsText.gameObject.SetActive(value: false);
		}
		Logger.RLog("[AwakeLogs] UIManager.Start <-");
	}

	private void OnDestroy()
	{
		if (gametipsui != null)
		{
			UnityEngine.Object.Destroy(gametipsui.gameObject);
			gametipsui = null;
		}
	}

	public void ShowGameTips(string tips)
	{
		if (gametipsui == null)
		{
			gametipsui = Hub.s.uiman.InstatiateUIPrefab<UIPrefab_GameTips>(prefab_ui_gametips);
		}
		gametipsui.SetTips(tips);
		gametipsui.Show();
	}

	public void ShowGameTips_v2(string tips)
	{
		if (gametipsui == null)
		{
			gametipsui = Hub.s.uiman.InstatiateUIPrefab<UIPrefab_GameTips>(prefab_ui_gametips);
		}
		gametipsui.SetTips_v2(tips);
		gametipsui.Show();
	}

	public void HideGameTips()
	{
		if (gametipsui != null)
		{
			gametipsui.OffTips();
			gametipsui.Hide();
		}
	}

	public void ToggleFps()
	{
		if (fpsText != null)
		{
			fpsText.gameObject.SetActive(!fpsText.gameObject.activeSelf);
		}
	}

	private async UniTaskVoid UpdateFpsAsync()
	{
		while (!base.destroyCancellationToken.IsCancellationRequested && !(Hub.s == null))
		{
			_ = fpsText != null;
			_ = Time.frameCount;
			await UniTask.WaitForSeconds(1f, ignoreTimeScale: false, PlayerLoopTiming.Update, base.destroyCancellationToken);
		}
	}

	public string PlayScreenEffect(GameObject prefab, float durationSec, int layerOrder)
	{
		string text = DateTime.UtcNow.Ticks.ToString();
		if (uiScreenEffectDictionary.ContainsKey(text))
		{
			Logger.RError("PlayLoopingScreenEffect : 이미 존재하는 id(" + text + ")입니다.");
			return "";
		}
		UIScreenEffectBase uIScreenEffectBase = InstatiateUIPrefab<UIScreenEffectBase>(prefab, eUIHeight.Bottom);
		if (uIScreenEffectBase != null)
		{
			uIScreenEffectBase.PlayScreenEffect(durationSec);
			uIScreenEffectBase.gameObject.SetActive(value: true);
			_ = uIScreenEffectBase.GetComponent<Renderer>() != null;
			uiScreenEffectDictionary.Add(text, uIScreenEffectBase);
		}
		return text;
	}

	public void StopScreenEffect(string effectKey)
	{
		if (!string.IsNullOrEmpty(effectKey) && uiScreenEffectDictionary.TryGetValue(effectKey, out UIScreenEffectBase value))
		{
			uiScreenEffectDictionary.Remove(effectKey);
			value.StopScreenEffect();
		}
	}

	public string PlayLoopingScreenEffect(GameObject prefab)
	{
		string text = DateTime.UtcNow.Ticks.ToString();
		if (uiScreenEffectDictionary.ContainsKey(text))
		{
			Logger.RError("PlayLoopingScreenEffect : 이미 존재하는 id(" + text + ")입니다.");
			return "";
		}
		UIScreenEffectBase uIScreenEffectBase = InstatiateUIPrefab<UIScreenEffectBase>(prefab, eUIHeight.Bottom);
		if (uIScreenEffectBase != null)
		{
			if (prefab.name.Contains("Conta"))
			{
				screenEffect_conta = uIScreenEffectBase;
			}
			else
			{
				screenEffect_conta = null;
			}
			uIScreenEffectBase.PlayLoopingScreenEffect();
			uIScreenEffectBase.gameObject.SetActive(value: true);
			_ = uIScreenEffectBase.GetComponent<Renderer>() != null;
			uiScreenEffectDictionary.Add(text, uIScreenEffectBase);
		}
		return text;
	}

	public void ShowTitleBGPrecess()
	{
		StartCoroutine(ShowTitleBG());
	}

	public IEnumerator ShowTitleBG()
	{
		if (!(currnetBG != null))
		{
			FadeIn(Color.black, 2f);
			UIPrefab_titleBG titleBG = InstatiateUIPrefab<UIPrefab_titleBG>(prefab_titleBG, eUIHeight.Background);
			currnetBG = titleBG.gameObject;
			titleBG.Show();
			string titleSfxId = ((sfxTable != null) ? sfxTable.titleSfxId : string.Empty);
			if (Hub.s != null && Hub.s.audioman != null)
			{
				Hub.s.audioman.PlaySfx(titleSfxId);
			}
			yield return new WaitUntil(() => SceneManager.GetActiveScene().name.Equals("MaintenanceScene"));
			if (Hub.s != null && Hub.s.audioman != null)
			{
				Hub.s.audioman.StopSfx(titleSfxId);
			}
			currnetBG = null;
			UnityEngine.Object.Destroy(titleBG.gameObject);
		}
	}

	public IEnumerator ChangeTitleBG()
	{
		yield break;
	}

	public void ShowTermsOfService()
	{
		if (tosUI != null)
		{
			tosUI.Show();
		}
		else
		{
			(tosUI = InstatiateUIPrefab<UIPrefab_TermsOfService>(prefab_tos, eUIHeight.Top)).Show();
		}
	}

	public void ShowPrivacyPolicy()
	{
		if (ppUI != null)
		{
			ppUI.Show();
		}
		else
		{
			(ppUI = InstatiateUIPrefab<UIPrefab_PrivacyPolicy>(prefab_pp, eUIHeight.Top)).Show();
		}
	}

	public void ShowAgeWarningNOW()
	{
		dTimeForAgeWarning = 3600f;
	}

	private IEnumerator ShowAgeWarningLoop()
	{
		UIPrefab_AgeWarning warning = InstatiateUIPrefab<UIPrefab_AgeWarning>(prefab_AgeWarnging, eUIHeight.OverTheTop);
		warning.dialogue = false;
		while (true)
		{
			dTimeForAgeWarning += Time.deltaTime;
			yield return new WaitForEndOfFrame();
			if (dTimeForAgeWarning > 3600f)
			{
				warning.Show();
				yield return new WaitForSeconds(3f);
				warning.Hide();
				dTimeForAgeWarning = 0f;
			}
		}
	}

	public void OpenSteamInventory()
	{
		if (steamInventoryUI == null)
		{
			steamInventoryUI = InstatiateUIPrefab<UIPrefab_SteamInventory>(prefab_SteamInventory, eUIHeight.Top);
		}
		ui_escapeStack.Add(steamInventoryUI);
		steamInventoryUI.Show();
	}

	public void CloseSteamInventory()
	{
		if (steamInventoryUI != null)
		{
			steamInventoryUI.Hide();
		}
	}

	public void OpenGameSettings(UIPrefabScript prevUI, bool inGame = false)
	{
		settingsPrevUI = prevUI;
		if (settingsUI == null)
		{
			settingsUI = InstatiateUIPrefab<UIPrefab_GameSettings>(prefab_GameSettings, eUIHeight.Top);
		}
		settingsPrevUI.Hide();
		settingsUI.Show();
	}

	public void CloseGameSettings()
	{
		settingsUI.Hide();
		if (settingsPrevUI != null)
		{
			settingsPrevUI.Show();
		}
	}

	public void OpenInGameMenu()
	{
		Logger.RLog("OpenInGameMenu");
		if (inGameMenu == null)
		{
			inGameMenu = InstatiateUIPrefab<UIPrefab_InGameMenu>(prefab_InGameMenu, eUIHeight.Top);
		}
		inGameMenu.Show();
		isGameMenuOpen = true;
	}

	public void CloseInGameMenu()
	{
		Logger.RLog("CloseInGameMenu");
		inGameMenu.Hide();
		Cursor.lockState = CursorLockMode.Locked;
		isGameMenuOpen = false;
	}

	public void OpenChangeKeyBinding(bool gamepad = false)
	{
		if (gamepad)
		{
			if (keyBindGamepadUI == null)
			{
				keyBindGamepadUI = InstatiateUIPrefab<UIPrefab_KeyBind>(prefab_ChangeKeyBindingGamepad);
			}
			keyBindGamepadUI.Show();
		}
		else
		{
			if (keyBindUI == null)
			{
				keyBindUI = InstatiateUIPrefab<UIPrefab_KeyBind>(prefab_ChangeKeyBinding);
			}
			keyBindUI.Show();
		}
		settingsUI.Hide();
	}

	public void CloseChangeKeyBinding(bool gamepad = false)
	{
		if (gamepad)
		{
			keyBindGamepadUI.Hide();
		}
		else
		{
			keyBindUI.Hide();
		}
		settingsUI.Show();
	}

	public void OpenAgreeUI(string name)
	{
		settingsUI.Hide();
		if (name == "tos")
		{
			ShowTermsOfService();
		}
		else if (name == "pp")
		{
			ShowPrivacyPolicy();
		}
	}

	public void CloseAgreeUI(UIPrefabScript ui)
	{
		ui.Hide();
		settingsUI.Show();
	}

	public void OpenRetrunToMainMenu()
	{
		if (retrunToMainMenu == null)
		{
			retrunToMainMenu = Hub.s.uiman.InstatiateUIPrefab<UIPrefab_ReturnToMainMenu>(prefab_RetrunToMainMenu, eUIHeight.Top);
		}
		retrunToMainMenu.inGameMenuUIObject = base.gameObject;
		retrunToMainMenu.Show();
		inGameMenu.Hide();
	}

	public void CloseRetrunToMainMenu()
	{
		retrunToMainMenu.Hide();
		if (inGameMenu != null)
		{
			inGameMenu.Show();
		}
	}

	public void HideIngameMenu()
	{
		if (inGameMenu != null)
		{
			inGameMenu.Hide();
			isGameMenuOpen = false;
		}
		if (settingsUI != null)
		{
			settingsUI.Hide();
		}
		if (keyBindUI != null)
		{
			keyBindUI.Hide();
		}
		if (retrunToMainMenu != null)
		{
			retrunToMainMenu.Hide();
		}
	}

	public void ShowConnectionFailed(string l10nKey, string[] args)
	{
		if (Hub.s.uiman.inviteLoadingUI != null)
		{
			Hub.s.uiman.inviteLoadingUI?.Hide();
		}
		Hub.s.tableman.uiprefabs.ShowDialog("ConnectionFailed", delegate(eUIDialogueBoxResult _response)
		{
			Cursor.lockState = CursorLockMode.None;
			if (_response == eUIDialogueBoxResult.OK)
			{
				Hub.LoadScene("MainMenuScene");
			}
		}, null, "INVITE_DO_NOT_ENTER_TITLE", l10nKey, args);
	}

	public void ShowEnterPublicRoomFailed(string l10nKey, string[] args)
	{
		if (Hub.s.uiman.inviteLoadingUI != null)
		{
			Hub.s.uiman.inviteLoadingUI?.Hide();
		}
		Hub.s.tableman.uiprefabs.ShowDialog("ConnectionFailed", delegate
		{
			Cursor.lockState = CursorLockMode.None;
		}, null, "STRING_CANT_ENTER_PUBLICROOM_TITLE", l10nKey, args);
	}

	private void CreateAllSoundGroupCreators()
	{
		if (!(sfxTable == null) && sfxTable.uiSoundGroupCreator != null)
		{
			UnityEngine.Object.Instantiate(sfxTable.uiSoundGroupCreator, base.transform);
		}
	}

	public void ShowGamepadEmote()
	{
		if (gamepadEmoteUI == null)
		{
			gamepadEmoteUI = InstatiateUIPrefab<UIPrefab_GamepadEmote>(prefab_GamepadEmote, eUIHeight.Top);
		}
		gamepadEmoteUI.Show();
	}

	public void OpenCreatorCodeUI(TMP_Text lastCreatorCodeTextUI)
	{
		if (creatorCodeUI == null)
		{
			creatorCodeUI = InstatiateUIPrefab<UIPrefab_CreatorCode>(prefab_CreatorCode, eUIHeight.Top);
		}
		creatorCodeUI.lastCreatorCodeTextUI = lastCreatorCodeTextUI;
		ui_escapeStack.Add(creatorCodeUI);
		creatorCodeUI.Show();
	}

	public void CloseCreatorCodeUI()
	{
		if (creatorCodeUI != null)
		{
			creatorCodeUI.InputCancel();
			creatorCodeUI.Hide();
		}
	}

	public void OpenCheckAge()
	{
		ageCheck = PlayerPrefs.GetInt("ageCheck", 0);
		if (ageCheck == 0)
		{
			InstatiateUIPrefab<UIPrefab_CheckAgeForVoiceChat>(prefab_CheckAgeForVoiceChat, eUIHeight.Top).Show();
		}
		else
		{
			ageCheck = 1;
		}
	}

	public void OpenPublicRoomList()
	{
		if (publicRoomListUI == null)
		{
			publicRoomListUI = InstatiateUIPrefab<UIPrefab_PublicRoomList>(prefab_PublicRoomList, eUIHeight.Top);
		}
		ui_escapeStack.Add(publicRoomListUI);
		publicRoomListUI.Show();
	}

	public ScriptableUIUtility GetScriptableUI()
	{
		return scriptableUIPanel;
	}

	private void Update()
	{
		int num = 0;
		if (Hub.s == null || Hub.s.inputman == null || (!Hub.s.inputman.wasPressedThisFrame(Mimic.InputSystem.InputAction.Escape) && (Gamepad.current == null || !Hub.s.inputman.isPressedGamepadButton("p_b") || isChangingKeyBind)))
		{
			return;
		}
		ui_escapeStack.RemoveAll((UIPrefabScript item) => item == null);
		ui_escapeStack.RemoveAll((UIPrefabScript item) => !item.gameObject.activeSelf);
		try
		{
			if (ui_escapeStack.Count > 0)
			{
				if (ui_escapeStack[ui_escapeStack.Count - 1].GetComponent<UIPrefab_dialogueBox>() != null)
				{
					ui_escapeStack[ui_escapeStack.Count - 1].GetComponent<UIPrefab_dialogueBox>().UE_OK_button.onClick.Invoke();
					return;
				}
				ui_escapeStack[ui_escapeStack.Count - 1].Hide();
				ui_escapeStack.RemoveAt(ui_escapeStack.Count - 1);
				return;
			}
		}
		catch (Exception)
		{
			Debug.LogError($" UI Manager Error : {num}");
		}
		num++;
		try
		{
			if (keyBindUI != null && keyBindUI.gameObject.activeSelf)
			{
				CloseChangeKeyBinding();
				return;
			}
		}
		catch (Exception)
		{
			Debug.LogError($" UI Manager Error : {num}");
		}
		num++;
		try
		{
			if (keyBindGamepadUI != null && keyBindGamepadUI.gameObject.activeSelf)
			{
				CloseChangeKeyBinding(gamepad: true);
				return;
			}
		}
		catch (Exception)
		{
			Debug.LogError($" UI Manager Error : {num}");
		}
		num++;
		try
		{
			if (settingsUI != null && settingsUI.gameObject.activeSelf)
			{
				CloseGameSettings();
				return;
			}
		}
		catch (Exception)
		{
			Debug.LogError($" UI Manager Error : {num}");
		}
		num++;
		try
		{
			if (retrunToMainMenu != null && retrunToMainMenu.gameObject.activeSelf)
			{
				CloseRetrunToMainMenu();
				return;
			}
		}
		catch (Exception)
		{
			Debug.LogError($" UI Manager Error : {num}");
		}
		num++;
		try
		{
			if (ReplaySharedData.IsReplayPlayMode)
			{
				return;
			}
		}
		catch (Exception)
		{
			Debug.LogError($" UI Manager Error : {num}");
		}
		num++;
		try
		{
			if (!(Hub.s.pdata.main != null))
			{
				return;
			}
			if (inGameMenu == null)
			{
				if (Hub.s.inputman.wasPressedThisFrame(Mimic.InputSystem.InputAction.Escape))
				{
					OpenInGameMenu();
				}
			}
			else if (inGameMenu.gameObject.activeSelf)
			{
				CloseInGameMenu();
			}
			else if (Hub.s.inputman.wasPressedThisFrame(Mimic.InputSystem.InputAction.Escape))
			{
				OpenInGameMenu();
			}
		}
		catch (Exception)
		{
			Debug.LogError($" UI Manager Error : {num}");
		}
	}

	public void TurnOnAntiSicknessOverlay()
	{
		anitiSickness.gameObject.SetActive(value: true);
	}

	public void TurnOffAntiSicknessOverlay()
	{
		anitiSickness.gameObject.SetActive(value: false);
	}

	public bool IsAntiSicknessOverlayVisible()
	{
		return anitiSickness.gameObject.activeSelf;
	}

	public Transform GetRainScreenVFXNode()
	{
		return rainScreenVFX;
	}

	public void OnNewSceneAwake()
	{
		if (!(rainScreenVFX != null))
		{
			return;
		}
		ParticleSystem[] componentsInChildren = rainScreenVFX.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem particleSystem in componentsInChildren)
		{
			if (particleSystem != null)
			{
				particleSystem.Stop();
			}
		}
	}
}
