using System;
using Mimic.InputSystem;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPrefab_KeyBindBtns : UIPrefabScript
{
	public delegate void OnMouseOverTargetChanged();

	public const string UEID_keyUITemplateLeft = "keyUITemplateLeft";

	public const string UEID_Left = "Left";

	public const string UEID_LeftSelected = "LeftSelected";

	public const string UEID_LeftKeyNameClicked = "LeftKeyNameClicked";

	public const string UEID_LeftKeyName = "LeftKeyName";

	public const string UEID_ButtonTemplateLeft = "ButtonTemplateLeft";

	public const string UEID_keyUITemplateRight = "keyUITemplateRight";

	public const string UEID_Right = "Right";

	public const string UEID_RightSelected = "RightSelected";

	public const string UEID_RightKeyNameClicked = "RightKeyNameClicked";

	public const string UEID_RightKeyName = "RightKeyName";

	public const string UEID_ButtonTemplateRight = "ButtonTemplateRight";

	private Transform _UE_keyUITemplateLeft;

	private Button _UE_Left;

	private Action<string> _OnLeft;

	private Image _UE_LeftSelected;

	private TMP_Text _UE_LeftKeyNameClicked;

	private TMP_Text _UE_LeftKeyName;

	private Button _UE_ButtonTemplateLeft;

	private Action<string> _OnButtonTemplateLeft;

	private Transform _UE_keyUITemplateRight;

	private Button _UE_Right;

	private Action<string> _OnRight;

	private Image _UE_RightSelected;

	private TMP_Text _UE_RightKeyNameClicked;

	private TMP_Text _UE_RightKeyName;

	private Button _UE_ButtonTemplateRight;

	private Action<string> _OnButtonTemplateRight;

	public ScrollRect targetScrollRect;

	public bool gamepad;

	public InputManagerData.Mapping mappingL;

	public InputManagerData.Mapping mappingR;

	public Transform UE_keyUITemplateLeft => _UE_keyUITemplateLeft ?? (_UE_keyUITemplateLeft = PickTransform("keyUITemplateLeft"));

	public Button UE_Left => _UE_Left ?? (_UE_Left = PickButton("Left"));

	public Action<string> OnLeft
	{
		get
		{
			return _OnLeft;
		}
		set
		{
			_OnLeft = value;
			SetOnButtonClick("Left", value);
		}
	}

	public Image UE_LeftSelected => _UE_LeftSelected ?? (_UE_LeftSelected = PickImage("LeftSelected"));

	public TMP_Text UE_LeftKeyNameClicked => _UE_LeftKeyNameClicked ?? (_UE_LeftKeyNameClicked = PickText("LeftKeyNameClicked"));

	public TMP_Text UE_LeftKeyName => _UE_LeftKeyName ?? (_UE_LeftKeyName = PickText("LeftKeyName"));

	public Button UE_ButtonTemplateLeft => _UE_ButtonTemplateLeft ?? (_UE_ButtonTemplateLeft = PickButton("ButtonTemplateLeft"));

	public Action<string> OnButtonTemplateLeft
	{
		get
		{
			return _OnButtonTemplateLeft;
		}
		set
		{
			_OnButtonTemplateLeft = value;
			SetOnButtonClick("ButtonTemplateLeft", value);
		}
	}

	public Transform UE_keyUITemplateRight => _UE_keyUITemplateRight ?? (_UE_keyUITemplateRight = PickTransform("keyUITemplateRight"));

	public Button UE_Right => _UE_Right ?? (_UE_Right = PickButton("Right"));

	public Action<string> OnRight
	{
		get
		{
			return _OnRight;
		}
		set
		{
			_OnRight = value;
			SetOnButtonClick("Right", value);
		}
	}

	public Image UE_RightSelected => _UE_RightSelected ?? (_UE_RightSelected = PickImage("RightSelected"));

	public TMP_Text UE_RightKeyNameClicked => _UE_RightKeyNameClicked ?? (_UE_RightKeyNameClicked = PickText("RightKeyNameClicked"));

	public TMP_Text UE_RightKeyName => _UE_RightKeyName ?? (_UE_RightKeyName = PickText("RightKeyName"));

	public Button UE_ButtonTemplateRight => _UE_ButtonTemplateRight ?? (_UE_ButtonTemplateRight = PickButton("ButtonTemplateRight"));

	public Action<string> OnButtonTemplateRight
	{
		get
		{
			return _OnButtonTemplateRight;
		}
		set
		{
			_OnButtonTemplateRight = value;
			SetOnButtonClick("ButtonTemplateRight", value);
		}
	}

	public event OnMouseOverTargetChanged onMouseOverTargetChanged;

	private void Start()
	{
		AddMouseOverEnterEvent(UE_Left.gameObject, UE_LeftKeyName);
		AddMouseOverExitEvent(UE_Left.gameObject, UE_LeftKeyName);
		AddMouseOverEnterEvent(UE_Right.gameObject, UE_RightKeyName);
		AddMouseOverExitEvent(UE_Right.gameObject, UE_RightKeyName);
		UE_Right.GetComponent<ScrollForwarder>().targetScrollRect = targetScrollRect;
		UE_Left.GetComponent<ScrollForwarder>().targetScrollRect = targetScrollRect;
	}

	public void OnClicked(bool left)
	{
		if (left)
		{
			UE_LeftSelected.gameObject.SetActive(value: true);
			UE_LeftSelected.color = new Color(1f, 1f, 1f, 1f);
			UE_ButtonTemplateLeft.image.color = new Color(1f, 1f, 1f, 0f);
			UE_LeftKeyNameClicked.text = UE_LeftKeyName.text;
			if (Hub.s == null)
			{
				UE_LeftKeyNameClicked.color = Hub.s.uiman.mouseOverTextColor;
			}
			else
			{
				UE_LeftKeyNameClicked.color = new Color(0f, 0f, 0f, 1f);
			}
		}
		else
		{
			UE_RightSelected.gameObject.SetActive(value: true);
			UE_RightSelected.color = new Color(1f, 1f, 1f, 1f);
			UE_ButtonTemplateRight.image.color = new Color(1f, 1f, 1f, 0f);
			UE_RightKeyNameClicked.text = UE_RightKeyName.text;
			if (Hub.s == null)
			{
				UE_RightKeyNameClicked.color = Hub.s.uiman.mouseOverTextColor;
			}
			else
			{
				UE_RightKeyNameClicked.color = new Color(0f, 0f, 0f, 1f);
			}
		}
	}

	public void ResetClicked()
	{
		EventSystem.current.SetSelectedGameObject(null);
		UE_LeftSelected.gameObject.SetActive(value: false);
		UE_LeftSelected.color = new Color(1f, 1f, 1f, 0f);
		UE_ButtonTemplateLeft.image.color = new Color(1f, 1f, 1f, 1f);
		UE_RightSelected.gameObject.SetActive(value: false);
		UE_RightSelected.color = new Color(1f, 1f, 1f, 0f);
		UE_ButtonTemplateRight.image.color = new Color(1f, 1f, 1f, 1f);
	}
}
