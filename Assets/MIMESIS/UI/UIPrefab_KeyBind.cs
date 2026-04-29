using System;
using System.Collections;
using System.Collections.Generic;
using Mimic.InputSystem;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPrefab_KeyBind : UIPrefabScript
{
	public const string UEID_WalkFoward = "WalkFoward";

	public const string UEID_ButtonWalkFoward = "ButtonWalkFoward";

	public const string UEID_WalkBack = "WalkBack";

	public const string UEID_ButtonWalkBack = "ButtonWalkBack";

	public const string UEID_WalkLeft = "WalkLeft";

	public const string UEID_ButtonWalkLeft = "ButtonWalkLeft";

	public const string UEID_WalkRight = "WalkRight";

	public const string UEID_ButtonWalkRight = "ButtonWalkRight";

	public const string UEID_Run = "Run";

	public const string UEID_ButtonRun = "ButtonRun";

	public const string UEID_Interact = "Interact";

	public const string UEID_ButtonInteract = "ButtonInteract";

	public const string UEID_PutDown = "PutDown";

	public const string UEID_ButtonPutDown = "ButtonPutDown";

	public const string UEID_Reload = "Reload";

	public const string UEID_ButtonReload = "ButtonReload";

	public const string UEID_SelectPreviousItem = "SelectPreviousItem";

	public const string UEID_ButtonSelectPreviousItem = "ButtonSelectPreviousItem";

	public const string UEID_SelectNectItem = "SelectNectItem";

	public const string UEID_ButtonSelectNectItem = "ButtonSelectNectItem";

	public const string UEID_Use = "Use";

	public const string UEID_ButtonUse = "ButtonUse";

	public const string UEID_Aim = "Aim";

	public const string UEID_ButtonAim = "ButtonAim";

	public const string UEID_Voice = "Voice";

	public const string UEID_ButtonVoice = "ButtonVoice";

	public const string UEID_Panel = "Panel";

	public const string UEID_ButtonBack = "ButtonBack";

	public const string UEID_ButtonResetKeys = "ButtonResetKeys";

	public const string UEID_Content = "Content";

	private Transform _UE_WalkFoward;

	private Button _UE_ButtonWalkFoward;

	private Action<string> _OnButtonWalkFoward;

	private Transform _UE_WalkBack;

	private Button _UE_ButtonWalkBack;

	private Action<string> _OnButtonWalkBack;

	private Transform _UE_WalkLeft;

	private Button _UE_ButtonWalkLeft;

	private Action<string> _OnButtonWalkLeft;

	private Transform _UE_WalkRight;

	private Button _UE_ButtonWalkRight;

	private Action<string> _OnButtonWalkRight;

	private Transform _UE_Run;

	private Button _UE_ButtonRun;

	private Action<string> _OnButtonRun;

	private Transform _UE_Interact;

	private Button _UE_ButtonInteract;

	private Action<string> _OnButtonInteract;

	private Transform _UE_PutDown;

	private Button _UE_ButtonPutDown;

	private Action<string> _OnButtonPutDown;

	private Transform _UE_Reload;

	private Button _UE_ButtonReload;

	private Action<string> _OnButtonReload;

	private Transform _UE_SelectPreviousItem;

	private Button _UE_ButtonSelectPreviousItem;

	private Action<string> _OnButtonSelectPreviousItem;

	private Transform _UE_SelectNectItem;

	private Button _UE_ButtonSelectNectItem;

	private Action<string> _OnButtonSelectNectItem;

	private Transform _UE_Use;

	private Button _UE_ButtonUse;

	private Action<string> _OnButtonUse;

	private Transform _UE_Aim;

	private Button _UE_ButtonAim;

	private Action<string> _OnButtonAim;

	private Transform _UE_Voice;

	private Button _UE_ButtonVoice;

	private Action<string> _OnButtonVoice;

	private Image _UE_Panel;

	private Button _UE_ButtonBack;

	private Action<string> _OnButtonBack;

	private Button _UE_ButtonResetKeys;

	private Action<string> _OnButtonResetKeys;

	private Transform _UE_Content;

	public bool gamepad;

	private List<GameObject> keyBindUis = new List<GameObject>();

	public GameObject keyTemplate;

	private List<InputManagerData.Mapping> mappings;

	private bool isFirst = true;

	[SerializeField]
	private ScrollRect scrollRect;

	public Transform UE_WalkFoward => _UE_WalkFoward ?? (_UE_WalkFoward = PickTransform("WalkFoward"));

	public Button UE_ButtonWalkFoward => _UE_ButtonWalkFoward ?? (_UE_ButtonWalkFoward = PickButton("ButtonWalkFoward"));

	public Action<string> OnButtonWalkFoward
	{
		get
		{
			return _OnButtonWalkFoward;
		}
		set
		{
			_OnButtonWalkFoward = value;
			SetOnButtonClick("ButtonWalkFoward", value);
		}
	}

	public Transform UE_WalkBack => _UE_WalkBack ?? (_UE_WalkBack = PickTransform("WalkBack"));

	public Button UE_ButtonWalkBack => _UE_ButtonWalkBack ?? (_UE_ButtonWalkBack = PickButton("ButtonWalkBack"));

	public Action<string> OnButtonWalkBack
	{
		get
		{
			return _OnButtonWalkBack;
		}
		set
		{
			_OnButtonWalkBack = value;
			SetOnButtonClick("ButtonWalkBack", value);
		}
	}

	public Transform UE_WalkLeft => _UE_WalkLeft ?? (_UE_WalkLeft = PickTransform("WalkLeft"));

	public Button UE_ButtonWalkLeft => _UE_ButtonWalkLeft ?? (_UE_ButtonWalkLeft = PickButton("ButtonWalkLeft"));

	public Action<string> OnButtonWalkLeft
	{
		get
		{
			return _OnButtonWalkLeft;
		}
		set
		{
			_OnButtonWalkLeft = value;
			SetOnButtonClick("ButtonWalkLeft", value);
		}
	}

	public Transform UE_WalkRight => _UE_WalkRight ?? (_UE_WalkRight = PickTransform("WalkRight"));

	public Button UE_ButtonWalkRight => _UE_ButtonWalkRight ?? (_UE_ButtonWalkRight = PickButton("ButtonWalkRight"));

	public Action<string> OnButtonWalkRight
	{
		get
		{
			return _OnButtonWalkRight;
		}
		set
		{
			_OnButtonWalkRight = value;
			SetOnButtonClick("ButtonWalkRight", value);
		}
	}

	public Transform UE_Run => _UE_Run ?? (_UE_Run = PickTransform("Run"));

	public Button UE_ButtonRun => _UE_ButtonRun ?? (_UE_ButtonRun = PickButton("ButtonRun"));

	public Action<string> OnButtonRun
	{
		get
		{
			return _OnButtonRun;
		}
		set
		{
			_OnButtonRun = value;
			SetOnButtonClick("ButtonRun", value);
		}
	}

	public Transform UE_Interact => _UE_Interact ?? (_UE_Interact = PickTransform("Interact"));

	public Button UE_ButtonInteract => _UE_ButtonInteract ?? (_UE_ButtonInteract = PickButton("ButtonInteract"));

	public Action<string> OnButtonInteract
	{
		get
		{
			return _OnButtonInteract;
		}
		set
		{
			_OnButtonInteract = value;
			SetOnButtonClick("ButtonInteract", value);
		}
	}

	public Transform UE_PutDown => _UE_PutDown ?? (_UE_PutDown = PickTransform("PutDown"));

	public Button UE_ButtonPutDown => _UE_ButtonPutDown ?? (_UE_ButtonPutDown = PickButton("ButtonPutDown"));

	public Action<string> OnButtonPutDown
	{
		get
		{
			return _OnButtonPutDown;
		}
		set
		{
			_OnButtonPutDown = value;
			SetOnButtonClick("ButtonPutDown", value);
		}
	}

	public Transform UE_Reload => _UE_Reload ?? (_UE_Reload = PickTransform("Reload"));

	public Button UE_ButtonReload => _UE_ButtonReload ?? (_UE_ButtonReload = PickButton("ButtonReload"));

	public Action<string> OnButtonReload
	{
		get
		{
			return _OnButtonReload;
		}
		set
		{
			_OnButtonReload = value;
			SetOnButtonClick("ButtonReload", value);
		}
	}

	public Transform UE_SelectPreviousItem => _UE_SelectPreviousItem ?? (_UE_SelectPreviousItem = PickTransform("SelectPreviousItem"));

	public Button UE_ButtonSelectPreviousItem => _UE_ButtonSelectPreviousItem ?? (_UE_ButtonSelectPreviousItem = PickButton("ButtonSelectPreviousItem"));

	public Action<string> OnButtonSelectPreviousItem
	{
		get
		{
			return _OnButtonSelectPreviousItem;
		}
		set
		{
			_OnButtonSelectPreviousItem = value;
			SetOnButtonClick("ButtonSelectPreviousItem", value);
		}
	}

	public Transform UE_SelectNectItem => _UE_SelectNectItem ?? (_UE_SelectNectItem = PickTransform("SelectNectItem"));

	public Button UE_ButtonSelectNectItem => _UE_ButtonSelectNectItem ?? (_UE_ButtonSelectNectItem = PickButton("ButtonSelectNectItem"));

	public Action<string> OnButtonSelectNectItem
	{
		get
		{
			return _OnButtonSelectNectItem;
		}
		set
		{
			_OnButtonSelectNectItem = value;
			SetOnButtonClick("ButtonSelectNectItem", value);
		}
	}

	public Transform UE_Use => _UE_Use ?? (_UE_Use = PickTransform("Use"));

	public Button UE_ButtonUse => _UE_ButtonUse ?? (_UE_ButtonUse = PickButton("ButtonUse"));

	public Action<string> OnButtonUse
	{
		get
		{
			return _OnButtonUse;
		}
		set
		{
			_OnButtonUse = value;
			SetOnButtonClick("ButtonUse", value);
		}
	}

	public Transform UE_Aim => _UE_Aim ?? (_UE_Aim = PickTransform("Aim"));

	public Button UE_ButtonAim => _UE_ButtonAim ?? (_UE_ButtonAim = PickButton("ButtonAim"));

	public Action<string> OnButtonAim
	{
		get
		{
			return _OnButtonAim;
		}
		set
		{
			_OnButtonAim = value;
			SetOnButtonClick("ButtonAim", value);
		}
	}

	public Transform UE_Voice => _UE_Voice ?? (_UE_Voice = PickTransform("Voice"));

	public Button UE_ButtonVoice => _UE_ButtonVoice ?? (_UE_ButtonVoice = PickButton("ButtonVoice"));

	public Action<string> OnButtonVoice
	{
		get
		{
			return _OnButtonVoice;
		}
		set
		{
			_OnButtonVoice = value;
			SetOnButtonClick("ButtonVoice", value);
		}
	}

	public Image UE_Panel => _UE_Panel ?? (_UE_Panel = PickImage("Panel"));

	public Button UE_ButtonBack => _UE_ButtonBack ?? (_UE_ButtonBack = PickButton("ButtonBack"));

	public Action<string> OnButtonBack
	{
		get
		{
			return _OnButtonBack;
		}
		set
		{
			_OnButtonBack = value;
			SetOnButtonClick("ButtonBack", value);
		}
	}

	public Button UE_ButtonResetKeys => _UE_ButtonResetKeys ?? (_UE_ButtonResetKeys = PickButton("ButtonResetKeys"));

	public Action<string> OnButtonResetKeys
	{
		get
		{
			return _OnButtonResetKeys;
		}
		set
		{
			_OnButtonResetKeys = value;
			SetOnButtonClick("ButtonResetKeys", value);
		}
	}

	public Transform UE_Content => _UE_Content ?? (_UE_Content = PickTransform("Content"));

	private void Start()
	{
		AddMouseOverEnterEvent(UE_ButtonBack.gameObject, UE_ButtonBack.GetComponentInChildren<TMP_Text>());
		AddMouseOverExitEvent(UE_ButtonBack.gameObject, UE_ButtonBack.GetComponentInChildren<TMP_Text>());
		AddMouseOverEnterEvent(UE_ButtonResetKeys.gameObject, UE_ButtonResetKeys.GetComponentInChildren<TMP_Text>());
		AddMouseOverExitEvent(UE_ButtonResetKeys.gameObject, UE_ButtonResetKeys.GetComponentInChildren<TMP_Text>());
	}

	private void OnEnable()
	{
		SetBtn_v2();
		OnButtonResetKeys = delegate
		{
			Hub.s.inputman.ResetKeys();
			SetBtn_v2();
			EventSystem.current.SetSelectedGameObject(null);
		};
		if (Hub.s != null)
		{
			Hub.s.inputman.onLastInputDeviceChanged += SetBtn_v2;
		}
		OnButtonBack = delegate
		{
			base.uiman.CloseChangeKeyBinding(gamepad);
		};
		UE_ButtonBack.GetComponentInChildren<TMP_Text>().color = Color.white;
		EventSystem.current.SetSelectedGameObject(null);
	}

	private void OnDisable()
	{
		if (Hub.s != null)
		{
			Hub.s.inputman.onLastInputDeviceChanged -= SetBtn_v2;
		}
	}

	private void SetBtn_v2()
	{
		mappings = new List<InputManagerData.Mapping>();
		foreach (InputManagerData.Mapping datum in Hub.s.inputman.data.data)
		{
			mappings.Add(new InputManagerData.Mapping(datum));
		}
		mappings.FindAll((InputManagerData.Mapping x) => x.action == InputAction.NextSpectatorTarget).ForEach(delegate(InputManagerData.Mapping x)
		{
			mappings.Remove(x);
		});
		mappings.FindAll((InputManagerData.Mapping x) => x.action == InputAction.PreviousSpectatorTarget).ForEach(delegate(InputManagerData.Mapping x)
		{
			mappings.Remove(x);
		});
		if (gamepad)
		{
			mappings.RemoveAll((InputManagerData.Mapping x) => x.gamepadKeys == "");
			mappings.RemoveAll((InputManagerData.Mapping x) => x.gamepadStaticKey);
		}
		else
		{
			mappings.FindAll((InputManagerData.Mapping x) => x.action == InputAction.EmotePanel).ForEach(delegate(InputManagerData.Mapping x)
			{
				mappings.Remove(x);
			});
		}
		mappings.RemoveAll((InputManagerData.Mapping x) => x.notUsed);
		for (int num = 0; num < mappings.Count; num++)
		{
			GameObject gameObject;
			if (isFirst)
			{
				gameObject = UnityEngine.Object.Instantiate(keyTemplate, UE_Content);
				keyBindUis.Add(gameObject);
			}
			else
			{
				gameObject = keyBindUis[(int)Math.Truncate((double)num * 0.5)];
			}
			UIPrefab_KeyBindBtns keyBindBtns = gameObject.GetComponent<UIPrefab_KeyBindBtns>();
			keyBindBtns.gamepad = gamepad;
			keyBindBtns.targetScrollRect = scrollRect;
			keyBindBtns.mappingL = mappings[num];
			keyBindBtns.UE_keyUITemplateLeft.GetComponentInChildren<TextMeshProUGUI>().text = Hub.GetL10NText(keyBindBtns.mappingL.description);
			Sprite sprite = null;
			sprite = ((!gamepad) ? Hub.s.gameSettingManager.keyImageData.GetKeyImage(keyBindBtns.mappingL.keys) : Hub.s.gameSettingManager.keyImageData.GetKeyImage(keyBindBtns.mappingL.gamepadKeys));
			if (sprite != null)
			{
				Sprite sprite2 = sprite;
				float width = sprite2.rect.width;
				float height = sprite2.rect.height;
				Vector2 sizeDelta = new Vector2(width * 0.56f, height * 0.56f);
				keyBindBtns.UE_ButtonTemplateLeft.image.sprite = sprite;
				keyBindBtns.UE_ButtonTemplateLeft.image.rectTransform.sizeDelta = sizeDelta;
			}
			else
			{
				keyBindBtns.UE_ButtonTemplateLeft.image.sprite = null;
			}
			keyBindBtns.OnLeft = delegate
			{
				StartCoroutine(ChangeKeyProcess(keyBindBtns.mappingL.action, gamepad));
				keyBindBtns.OnClicked(left: true);
			};
			if (num + 1 >= mappings.Count)
			{
				keyBindBtns.UE_keyUITemplateRight.gameObject.SetActive(value: false);
				keyBindBtns.ResetClicked();
				keyBindBtns.Show();
				continue;
			}
			num++;
			keyBindBtns.mappingR = mappings[num];
			keyBindBtns.UE_keyUITemplateRight.GetComponentInChildren<TextMeshProUGUI>().text = Hub.GetL10NText(keyBindBtns.mappingR.description);
			Sprite sprite3 = null;
			sprite3 = ((!gamepad) ? Hub.s.gameSettingManager.keyImageData.GetKeyImage(keyBindBtns.mappingR.keys) : Hub.s.gameSettingManager.keyImageData.GetKeyImage(keyBindBtns.mappingR.gamepadKeys));
			if (sprite3 != null)
			{
				Sprite sprite4 = sprite3;
				float width2 = sprite4.rect.width;
				float height2 = sprite4.rect.height;
				Vector2 sizeDelta2 = new Vector2(width2 * 0.56f, height2 * 0.56f);
				keyBindBtns.UE_ButtonTemplateRight.image.sprite = sprite3;
				keyBindBtns.UE_ButtonTemplateRight.image.rectTransform.sizeDelta = sizeDelta2;
			}
			else
			{
				keyBindBtns.UE_ButtonTemplateRight.image.sprite = null;
			}
			keyBindBtns.OnRight = delegate
			{
				StartCoroutine(ChangeKeyProcess(keyBindBtns.mappingR.action, gamepad));
				keyBindBtns.OnClicked(left: false);
			};
			keyBindBtns.ResetClicked();
			keyBindBtns.Show();
		}
		UE_Content.GetComponent<RectTransform>().sizeDelta = new Vector2(UE_Content.GetComponent<RectTransform>().sizeDelta.x, keyTemplate.GetComponent<RectTransform>().sizeDelta.y * (float)keyBindUis.Count);
		isFirst = false;
	}

	private IEnumerator ChangeKeyProcess(InputAction action, bool gamepad = false)
	{
		if (Hub.s != null)
		{
			Hub.s.uiman.isChangingKeyBind = true;
		}
		UE_Panel.gameObject.SetActive(value: true);
		yield return Hub.s.inputman.changeKeyBindCoroutine = Hub.s.inputman.StartCoroutine(Hub.s.inputman.ChangeKeyBindProcess(action, gamepad));
		Hub.s.inputman.changeKeyBindCoroutine = null;
		if (Hub.s.pdata.main != null)
		{
			Hub.s.uiman.isChangingKeyBind = false;
		}
		SetBtn_v2();
	}
}
