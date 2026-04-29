using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dissonance;
using Dissonance.Audio.Capture;
using Dissonance.Integrations.FishNet;
using Mimic.InputSystem;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIPrefab_GameSettings : UIPrefabScript
{
	public delegate void OnMouseOverTargetChanged();

	public const string UEID_BackButton = "BackButton";

	public const string UEID_CLOSE_mouseover = "CLOSE_mouseover";

	public const string UEID_ResetButton = "ResetButton";

	public const string UEID_RESET_mouseover = "RESET_mouseover";

	public const string UEID_KeyBinds_mouseover = "KeyBinds_mouseover";

	public const string UEID_KEYBINDS = "KEYBINDS";

	public const string UEID_ChangeKeyBindsButton = "ChangeKeyBindsButton";

	public const string UEID_KeybindsRightArrow = "KeybindsRightArrow";

	public const string UEID_KeyBindsGamepad_mouseover = "KeyBindsGamepad_mouseover";

	public const string UEID_KEYBINDSGAMEPAD = "KEYBINDSGAMEPAD";

	public const string UEID_ChangeKeyBindsGamepadButton = "ChangeKeyBindsGamepadButton";

	public const string UEID_KeybindsGamepadRightArrow = "KeybindsGamepadRightArrow";

	public const string UEID_INBERT_Y_AXIS_mouseover = "INBERT_Y_AXIS_mouseover";

	public const string UEID_YPrev = "YPrev";

	public const string UEID_YPrevArrow = "YPrevArrow";

	public const string UEID_YSelected = "YSelected";

	public const string UEID_YNext = "YNext";

	public const string UEID_YNextArrow = "YNextArrow";

	public const string UEID_INVERT_YAXIS = "INVERT_YAXIS";

	public const string UEID_LOOK_SENSITIVITY_mouseover = "LOOK_SENSITIVITY_mouseover";

	public const string UEID_LookSensitivity_Slider = "LookSensitivity_Slider";

	public const string UEID_LOOK_SENSITIVITY = "LOOK_SENSITIVITY";

	public const string UEID_DEVICE_INDEX_mouseover = "DEVICE_INDEX_mouseover";

	public const string UEID_DEVICE_INDEX = "DEVICE_INDEX";

	public const string UEID_Dropdown_selectMic = "Dropdown_selectMic";

	public const string UEID_Dropdown_selectMic_Label = "Dropdown_selectMic_Label";

	public const string UEID_Dropdown_selectMicArrow = "Dropdown_selectMicArrow";

	public const string UEID_item_mouseOver = "item_mouseOver";

	public const string UEID_Voice_MODE_mouseover = "Voice_MODE_mouseover";

	public const string UEID_voiceSelected = "voiceSelected";

	public const string UEID_voicePrev = "voicePrev";

	public const string UEID_voicePrevArrow = "voicePrevArrow";

	public const string UEID_voiceNext = "voiceNext";

	public const string UEID_voiceNextArrow = "voiceNextArrow";

	public const string UEID_VOICE_MODE = "VOICE_MODE";

	public const string UEID_volumeGauge = "volumeGauge";

	public const string UEID_MASTER_VOLUME_mouseover = "MASTER_VOLUME_mouseover";

	public const string UEID_MasterVolume_Slider = "MasterVolume_Slider";

	public const string UEID_MASTER_VOLUME = "MASTER_VOLUME";

	public const string UEID_MIC_VOLUME_mouseover = "MIC_VOLUME_mouseover";

	public const string UEID_MicVolume_Slider = "MicVolume_Slider";

	public const string UEID_Mic_VOLUME = "Mic_VOLUME";

	public const string UEID_TERMS_OF_SERVICE_Button = "TERMS_OF_SERVICE_Button";

	public const string UEID_TERMS_OF_SERVICE_mouseover = "TERMS_OF_SERVICE_mouseover";

	public const string UEID_PRIVACY_POLICY_Button = "PRIVACY_POLICY_Button";

	public const string UEID_PRIVACY_POLICY_mouseover = "PRIVACY_POLICY_mouseover";

	public const string UEID_AGE_RATINGS_Button = "AGE_RATINGS_Button";

	public const string UEID_AGE_RATINGS_mouseover = "AGE_RATINGS_mouseover";

	public const string UEID_DISPLAY_MODE_mouseover = "DISPLAY_MODE_mouseover";

	public const string UEID_Dropdown_selectDisplayMode = "Dropdown_selectDisplayMode";

	public const string UEID_Dropdown_selectDisplay_Label = "Dropdown_selectDisplay_Label";

	public const string UEID_Dropdown_selectDosplayArrow = "Dropdown_selectDosplayArrow";

	public const string UEID_item_DisplaymouseOver = "item_DisplaymouseOver";

	public const string UEID_DISPLAY_MODE = "DISPLAY_MODE";

	public const string UEID_GAMMA_mouseover = "GAMMA_mouseover";

	public const string UEID_Gamma_Slider = "Gamma_Slider";

	public const string UEID_GAMMA = "GAMMA";

	public const string UEID_BRIGHTNESS_mouseover = "BRIGHTNESS_mouseover";

	public const string UEID_Brightness_Slider = "Brightness_Slider";

	public const string UEID_BRIGHTNESS = "BRIGHTNESS";

	public const string UEID_LANGUAGE_mouseover = "LANGUAGE_mouseover";

	public const string UEID_Dropdown_selectLanguage = "Dropdown_selectLanguage";

	public const string UEID_Dropdown_selectLanguage_Label = "Dropdown_selectLanguage_Label";

	public const string UEID_Dropdown_selectLanguageArrow = "Dropdown_selectLanguageArrow";

	public const string UEID_item_DropdownmouseOver = "item_DropdownmouseOver";

	public const string UEID_LANGUAGE = "LANGUAGE";

	public const string UEID_FRAMERATE_mouseover = "FRAMERATE_mouseover";

	public const string UEID_Framerate_select = "Framerate_select";

	public const string UEID_Framerate_select_Label = "Framerate_select_Label";

	public const string UEID_Framerate_selectArrow = "Framerate_selectArrow";

	public const string UEID_item_YmouseOver = "item_YmouseOver";

	public const string UEID_FRAMERATE = "FRAMERATE";

	public const string UEID_RESOLUTION_mouseover = "RESOLUTION_mouseover";

	public const string UEID_Resolution_select = "Resolution_select";

	public const string UEID_Resolution_select_Label = "Resolution_select_Label";

	public const string UEID_Resolution_selectArrow = "Resolution_selectArrow";

	public const string UEID_item_RmouseOver = "item_RmouseOver";

	public const string UEID_RESOLUTION = "RESOLUTION";

	public const string UEID_Vsync_mouseover = "Vsync_mouseover";

	public const string UEID_VsyncLeft = "VsyncLeft";

	public const string UEID_VsyncLeftArrow = "VsyncLeftArrow";

	public const string UEID_VsyncSelected = "VsyncSelected";

	public const string UEID_VsyncRight = "VsyncRight";

	public const string UEID_VsyncRightArrow = "VsyncRightArrow";

	public const string UEID_Vsync = "Vsync";

	public const string UEID_CreatorCode = "CreatorCode";

	public const string UEID_CreatorCode_mouseover = "CreatorCode_mouseover";

	public const string UEID_CreateCodeText = "CreateCodeText";

	public const string UEID_lastInputedCreatorCode = "lastInputedCreatorCode";

	private Button _UE_BackButton;

	private Action<string> _OnBackButton;

	private Image _UE_CLOSE_mouseover;

	private Button _UE_ResetButton;

	private Action<string> _OnResetButton;

	private Image _UE_RESET_mouseover;

	private Image _UE_KeyBinds_mouseover;

	private TMP_Text _UE_KEYBINDS;

	private Button _UE_ChangeKeyBindsButton;

	private Action<string> _OnChangeKeyBindsButton;

	private Image _UE_KeybindsRightArrow;

	private Image _UE_KeyBindsGamepad_mouseover;

	private TMP_Text _UE_KEYBINDSGAMEPAD;

	private Button _UE_ChangeKeyBindsGamepadButton;

	private Action<string> _OnChangeKeyBindsGamepadButton;

	private Image _UE_KeybindsGamepadRightArrow;

	private Image _UE_INBERT_Y_AXIS_mouseover;

	private Button _UE_YPrev;

	private Action<string> _OnYPrev;

	private Image _UE_YPrevArrow;

	private TMP_Text _UE_YSelected;

	private Button _UE_YNext;

	private Action<string> _OnYNext;

	private Image _UE_YNextArrow;

	private TMP_Text _UE_INVERT_YAXIS;

	private Image _UE_LOOK_SENSITIVITY_mouseover;

	private Transform _UE_LookSensitivity_Slider;

	private TMP_Text _UE_LOOK_SENSITIVITY;

	private Image _UE_DEVICE_INDEX_mouseover;

	private TMP_Text _UE_DEVICE_INDEX;

	private Image _UE_Dropdown_selectMic;

	private TMP_Text _UE_Dropdown_selectMic_Label;

	private Image _UE_Dropdown_selectMicArrow;

	private Image _UE_item_mouseOver;

	private Image _UE_Voice_MODE_mouseover;

	private TMP_Text _UE_voiceSelected;

	private Button _UE_voicePrev;

	private Action<string> _OnvoicePrev;

	private Image _UE_voicePrevArrow;

	private Button _UE_voiceNext;

	private Action<string> _OnvoiceNext;

	private Image _UE_voiceNextArrow;

	private TMP_Text _UE_VOICE_MODE;

	private Image _UE_volumeGauge;

	private Image _UE_MASTER_VOLUME_mouseover;

	private Transform _UE_MasterVolume_Slider;

	private TMP_Text _UE_MASTER_VOLUME;

	private Image _UE_MIC_VOLUME_mouseover;

	private Transform _UE_MicVolume_Slider;

	private TMP_Text _UE_Mic_VOLUME;

	private Button _UE_TERMS_OF_SERVICE_Button;

	private Action<string> _OnTERMS_OF_SERVICE_Button;

	private Image _UE_TERMS_OF_SERVICE_mouseover;

	private Button _UE_PRIVACY_POLICY_Button;

	private Action<string> _OnPRIVACY_POLICY_Button;

	private Image _UE_PRIVACY_POLICY_mouseover;

	private Button _UE_AGE_RATINGS_Button;

	private Action<string> _OnAGE_RATINGS_Button;

	private Image _UE_AGE_RATINGS_mouseover;

	private Image _UE_DISPLAY_MODE_mouseover;

	private Image _UE_Dropdown_selectDisplayMode;

	private TMP_Text _UE_Dropdown_selectDisplay_Label;

	private Image _UE_Dropdown_selectDosplayArrow;

	private Image _UE_item_DisplaymouseOver;

	private TMP_Text _UE_DISPLAY_MODE;

	private Image _UE_GAMMA_mouseover;

	private Transform _UE_Gamma_Slider;

	private TMP_Text _UE_GAMMA;

	private Image _UE_BRIGHTNESS_mouseover;

	private Transform _UE_Brightness_Slider;

	private TMP_Text _UE_BRIGHTNESS;

	private Image _UE_LANGUAGE_mouseover;

	private Image _UE_Dropdown_selectLanguage;

	private TMP_Text _UE_Dropdown_selectLanguage_Label;

	private Image _UE_Dropdown_selectLanguageArrow;

	private Image _UE_item_DropdownmouseOver;

	private TMP_Text _UE_LANGUAGE;

	private Image _UE_FRAMERATE_mouseover;

	private Image _UE_Framerate_select;

	private TMP_Text _UE_Framerate_select_Label;

	private Image _UE_Framerate_selectArrow;

	private Image _UE_item_YmouseOver;

	private TMP_Text _UE_FRAMERATE;

	private Image _UE_RESOLUTION_mouseover;

	private Image _UE_Resolution_select;

	private TMP_Text _UE_Resolution_select_Label;

	private Image _UE_Resolution_selectArrow;

	private Image _UE_item_RmouseOver;

	private TMP_Text _UE_RESOLUTION;

	private Image _UE_Vsync_mouseover;

	private Button _UE_VsyncLeft;

	private Action<string> _OnVsyncLeft;

	private Image _UE_VsyncLeftArrow;

	private TMP_Text _UE_VsyncSelected;

	private Button _UE_VsyncRight;

	private Action<string> _OnVsyncRight;

	private Image _UE_VsyncRightArrow;

	private TMP_Text _UE_Vsync;

	private Button _UE_CreatorCode;

	private Action<string> _OnCreatorCode;

	private Image _UE_CreatorCode_mouseover;

	private TMP_Text _UE_CreateCodeText;

	private TMP_Text _UE_lastInputedCreatorCode;

	private CommActivationMode voiceMode = CommActivationMode.VoiceActivation;

	private AudioClip micClip;

	private string micName;

	private int sampleWindow = 128;

	private bool invertYAxis;

	private bool vsync;

	public Sprite sliderNormalSprite;

	public Sprite sliderNormalLongSprite;

	public Sprite sliderSelectedSprite;

	public Sprite sliderSelectedLongSprite;

	public Sprite sliderGaugeNormalSprite;

	public Sprite sliderGaugeNormalLongSprite;

	public Sprite sliderGuageSelectedSprite;

	public Sprite sliderGuageSelectedLongSprite;

	public bool OnPushToTalk;

	private string micNameSelected = "";

	private BasicMicrophoneCapture _microphoneCapture;

	public List<Image> mouseovers = new List<Image>();

	private FullScreenMode currentDisplayMode;

	private bool OnDisplayModeChanged;

	[Tooltip("스크롤 속도(px/sec)")]
	public float marqueeSpeed = 50f;

	public bool initResoutionDropdown;

	private int prevValue = -1;

	private AudioClip _clip;

	public Button UE_BackButton => _UE_BackButton ?? (_UE_BackButton = PickButton("BackButton"));

	public Action<string> OnBackButton
	{
		get
		{
			return _OnBackButton;
		}
		set
		{
			_OnBackButton = value;
			SetOnButtonClick("BackButton", value);
		}
	}

	public Image UE_CLOSE_mouseover => _UE_CLOSE_mouseover ?? (_UE_CLOSE_mouseover = PickImage("CLOSE_mouseover"));

	public Button UE_ResetButton => _UE_ResetButton ?? (_UE_ResetButton = PickButton("ResetButton"));

	public Action<string> OnResetButton
	{
		get
		{
			return _OnResetButton;
		}
		set
		{
			_OnResetButton = value;
			SetOnButtonClick("ResetButton", value);
		}
	}

	public Image UE_RESET_mouseover => _UE_RESET_mouseover ?? (_UE_RESET_mouseover = PickImage("RESET_mouseover"));

	public Image UE_KeyBinds_mouseover => _UE_KeyBinds_mouseover ?? (_UE_KeyBinds_mouseover = PickImage("KeyBinds_mouseover"));

	public TMP_Text UE_KEYBINDS => _UE_KEYBINDS ?? (_UE_KEYBINDS = PickText("KEYBINDS"));

	public Button UE_ChangeKeyBindsButton => _UE_ChangeKeyBindsButton ?? (_UE_ChangeKeyBindsButton = PickButton("ChangeKeyBindsButton"));

	public Action<string> OnChangeKeyBindsButton
	{
		get
		{
			return _OnChangeKeyBindsButton;
		}
		set
		{
			_OnChangeKeyBindsButton = value;
			SetOnButtonClick("ChangeKeyBindsButton", value);
		}
	}

	public Image UE_KeybindsRightArrow => _UE_KeybindsRightArrow ?? (_UE_KeybindsRightArrow = PickImage("KeybindsRightArrow"));

	public Image UE_KeyBindsGamepad_mouseover => _UE_KeyBindsGamepad_mouseover ?? (_UE_KeyBindsGamepad_mouseover = PickImage("KeyBindsGamepad_mouseover"));

	public TMP_Text UE_KEYBINDSGAMEPAD => _UE_KEYBINDSGAMEPAD ?? (_UE_KEYBINDSGAMEPAD = PickText("KEYBINDSGAMEPAD"));

	public Button UE_ChangeKeyBindsGamepadButton => _UE_ChangeKeyBindsGamepadButton ?? (_UE_ChangeKeyBindsGamepadButton = PickButton("ChangeKeyBindsGamepadButton"));

	public Action<string> OnChangeKeyBindsGamepadButton
	{
		get
		{
			return _OnChangeKeyBindsGamepadButton;
		}
		set
		{
			_OnChangeKeyBindsGamepadButton = value;
			SetOnButtonClick("ChangeKeyBindsGamepadButton", value);
		}
	}

	public Image UE_KeybindsGamepadRightArrow => _UE_KeybindsGamepadRightArrow ?? (_UE_KeybindsGamepadRightArrow = PickImage("KeybindsGamepadRightArrow"));

	public Image UE_INBERT_Y_AXIS_mouseover => _UE_INBERT_Y_AXIS_mouseover ?? (_UE_INBERT_Y_AXIS_mouseover = PickImage("INBERT_Y_AXIS_mouseover"));

	public Button UE_YPrev => _UE_YPrev ?? (_UE_YPrev = PickButton("YPrev"));

	public Action<string> OnYPrev
	{
		get
		{
			return _OnYPrev;
		}
		set
		{
			_OnYPrev = value;
			SetOnButtonClick("YPrev", value);
		}
	}

	public Image UE_YPrevArrow => _UE_YPrevArrow ?? (_UE_YPrevArrow = PickImage("YPrevArrow"));

	public TMP_Text UE_YSelected => _UE_YSelected ?? (_UE_YSelected = PickText("YSelected"));

	public Button UE_YNext => _UE_YNext ?? (_UE_YNext = PickButton("YNext"));

	public Action<string> OnYNext
	{
		get
		{
			return _OnYNext;
		}
		set
		{
			_OnYNext = value;
			SetOnButtonClick("YNext", value);
		}
	}

	public Image UE_YNextArrow => _UE_YNextArrow ?? (_UE_YNextArrow = PickImage("YNextArrow"));

	public TMP_Text UE_INVERT_YAXIS => _UE_INVERT_YAXIS ?? (_UE_INVERT_YAXIS = PickText("INVERT_YAXIS"));

	public Image UE_LOOK_SENSITIVITY_mouseover => _UE_LOOK_SENSITIVITY_mouseover ?? (_UE_LOOK_SENSITIVITY_mouseover = PickImage("LOOK_SENSITIVITY_mouseover"));

	public Transform UE_LookSensitivity_Slider => _UE_LookSensitivity_Slider ?? (_UE_LookSensitivity_Slider = PickTransform("LookSensitivity_Slider"));

	public TMP_Text UE_LOOK_SENSITIVITY => _UE_LOOK_SENSITIVITY ?? (_UE_LOOK_SENSITIVITY = PickText("LOOK_SENSITIVITY"));

	public Image UE_DEVICE_INDEX_mouseover => _UE_DEVICE_INDEX_mouseover ?? (_UE_DEVICE_INDEX_mouseover = PickImage("DEVICE_INDEX_mouseover"));

	public TMP_Text UE_DEVICE_INDEX => _UE_DEVICE_INDEX ?? (_UE_DEVICE_INDEX = PickText("DEVICE_INDEX"));

	public Image UE_Dropdown_selectMic => _UE_Dropdown_selectMic ?? (_UE_Dropdown_selectMic = PickImage("Dropdown_selectMic"));

	public TMP_Text UE_Dropdown_selectMic_Label => _UE_Dropdown_selectMic_Label ?? (_UE_Dropdown_selectMic_Label = PickText("Dropdown_selectMic_Label"));

	public Image UE_Dropdown_selectMicArrow => _UE_Dropdown_selectMicArrow ?? (_UE_Dropdown_selectMicArrow = PickImage("Dropdown_selectMicArrow"));

	public Image UE_item_mouseOver => _UE_item_mouseOver ?? (_UE_item_mouseOver = PickImage("item_mouseOver"));

	public Image UE_Voice_MODE_mouseover => _UE_Voice_MODE_mouseover ?? (_UE_Voice_MODE_mouseover = PickImage("Voice_MODE_mouseover"));

	public TMP_Text UE_voiceSelected => _UE_voiceSelected ?? (_UE_voiceSelected = PickText("voiceSelected"));

	public Button UE_voicePrev => _UE_voicePrev ?? (_UE_voicePrev = PickButton("voicePrev"));

	public Action<string> OnvoicePrev
	{
		get
		{
			return _OnvoicePrev;
		}
		set
		{
			_OnvoicePrev = value;
			SetOnButtonClick("voicePrev", value);
		}
	}

	public Image UE_voicePrevArrow => _UE_voicePrevArrow ?? (_UE_voicePrevArrow = PickImage("voicePrevArrow"));

	public Button UE_voiceNext => _UE_voiceNext ?? (_UE_voiceNext = PickButton("voiceNext"));

	public Action<string> OnvoiceNext
	{
		get
		{
			return _OnvoiceNext;
		}
		set
		{
			_OnvoiceNext = value;
			SetOnButtonClick("voiceNext", value);
		}
	}

	public Image UE_voiceNextArrow => _UE_voiceNextArrow ?? (_UE_voiceNextArrow = PickImage("voiceNextArrow"));

	public TMP_Text UE_VOICE_MODE => _UE_VOICE_MODE ?? (_UE_VOICE_MODE = PickText("VOICE_MODE"));

	public Image UE_volumeGauge => _UE_volumeGauge ?? (_UE_volumeGauge = PickImage("volumeGauge"));

	public Image UE_MASTER_VOLUME_mouseover => _UE_MASTER_VOLUME_mouseover ?? (_UE_MASTER_VOLUME_mouseover = PickImage("MASTER_VOLUME_mouseover"));

	public Transform UE_MasterVolume_Slider => _UE_MasterVolume_Slider ?? (_UE_MasterVolume_Slider = PickTransform("MasterVolume_Slider"));

	public TMP_Text UE_MASTER_VOLUME => _UE_MASTER_VOLUME ?? (_UE_MASTER_VOLUME = PickText("MASTER_VOLUME"));

	public Image UE_MIC_VOLUME_mouseover => _UE_MIC_VOLUME_mouseover ?? (_UE_MIC_VOLUME_mouseover = PickImage("MIC_VOLUME_mouseover"));

	public Transform UE_MicVolume_Slider => _UE_MicVolume_Slider ?? (_UE_MicVolume_Slider = PickTransform("MicVolume_Slider"));

	public TMP_Text UE_Mic_VOLUME => _UE_Mic_VOLUME ?? (_UE_Mic_VOLUME = PickText("Mic_VOLUME"));

	public Button UE_TERMS_OF_SERVICE_Button => _UE_TERMS_OF_SERVICE_Button ?? (_UE_TERMS_OF_SERVICE_Button = PickButton("TERMS_OF_SERVICE_Button"));

	public Action<string> OnTERMS_OF_SERVICE_Button
	{
		get
		{
			return _OnTERMS_OF_SERVICE_Button;
		}
		set
		{
			_OnTERMS_OF_SERVICE_Button = value;
			SetOnButtonClick("TERMS_OF_SERVICE_Button", value);
		}
	}

	public Image UE_TERMS_OF_SERVICE_mouseover => _UE_TERMS_OF_SERVICE_mouseover ?? (_UE_TERMS_OF_SERVICE_mouseover = PickImage("TERMS_OF_SERVICE_mouseover"));

	public Button UE_PRIVACY_POLICY_Button => _UE_PRIVACY_POLICY_Button ?? (_UE_PRIVACY_POLICY_Button = PickButton("PRIVACY_POLICY_Button"));

	public Action<string> OnPRIVACY_POLICY_Button
	{
		get
		{
			return _OnPRIVACY_POLICY_Button;
		}
		set
		{
			_OnPRIVACY_POLICY_Button = value;
			SetOnButtonClick("PRIVACY_POLICY_Button", value);
		}
	}

	public Image UE_PRIVACY_POLICY_mouseover => _UE_PRIVACY_POLICY_mouseover ?? (_UE_PRIVACY_POLICY_mouseover = PickImage("PRIVACY_POLICY_mouseover"));

	public Button UE_AGE_RATINGS_Button => _UE_AGE_RATINGS_Button ?? (_UE_AGE_RATINGS_Button = PickButton("AGE_RATINGS_Button"));

	public Action<string> OnAGE_RATINGS_Button
	{
		get
		{
			return _OnAGE_RATINGS_Button;
		}
		set
		{
			_OnAGE_RATINGS_Button = value;
			SetOnButtonClick("AGE_RATINGS_Button", value);
		}
	}

	public Image UE_AGE_RATINGS_mouseover => _UE_AGE_RATINGS_mouseover ?? (_UE_AGE_RATINGS_mouseover = PickImage("AGE_RATINGS_mouseover"));

	public Image UE_DISPLAY_MODE_mouseover => _UE_DISPLAY_MODE_mouseover ?? (_UE_DISPLAY_MODE_mouseover = PickImage("DISPLAY_MODE_mouseover"));

	public Image UE_Dropdown_selectDisplayMode => _UE_Dropdown_selectDisplayMode ?? (_UE_Dropdown_selectDisplayMode = PickImage("Dropdown_selectDisplayMode"));

	public TMP_Text UE_Dropdown_selectDisplay_Label => _UE_Dropdown_selectDisplay_Label ?? (_UE_Dropdown_selectDisplay_Label = PickText("Dropdown_selectDisplay_Label"));

	public Image UE_Dropdown_selectDosplayArrow => _UE_Dropdown_selectDosplayArrow ?? (_UE_Dropdown_selectDosplayArrow = PickImage("Dropdown_selectDosplayArrow"));

	public Image UE_item_DisplaymouseOver => _UE_item_DisplaymouseOver ?? (_UE_item_DisplaymouseOver = PickImage("item_DisplaymouseOver"));

	public TMP_Text UE_DISPLAY_MODE => _UE_DISPLAY_MODE ?? (_UE_DISPLAY_MODE = PickText("DISPLAY_MODE"));

	public Image UE_GAMMA_mouseover => _UE_GAMMA_mouseover ?? (_UE_GAMMA_mouseover = PickImage("GAMMA_mouseover"));

	public Transform UE_Gamma_Slider => _UE_Gamma_Slider ?? (_UE_Gamma_Slider = PickTransform("Gamma_Slider"));

	public TMP_Text UE_GAMMA => _UE_GAMMA ?? (_UE_GAMMA = PickText("GAMMA"));

	public Image UE_BRIGHTNESS_mouseover => _UE_BRIGHTNESS_mouseover ?? (_UE_BRIGHTNESS_mouseover = PickImage("BRIGHTNESS_mouseover"));

	public Transform UE_Brightness_Slider => _UE_Brightness_Slider ?? (_UE_Brightness_Slider = PickTransform("Brightness_Slider"));

	public TMP_Text UE_BRIGHTNESS => _UE_BRIGHTNESS ?? (_UE_BRIGHTNESS = PickText("BRIGHTNESS"));

	public Image UE_LANGUAGE_mouseover => _UE_LANGUAGE_mouseover ?? (_UE_LANGUAGE_mouseover = PickImage("LANGUAGE_mouseover"));

	public Image UE_Dropdown_selectLanguage => _UE_Dropdown_selectLanguage ?? (_UE_Dropdown_selectLanguage = PickImage("Dropdown_selectLanguage"));

	public TMP_Text UE_Dropdown_selectLanguage_Label => _UE_Dropdown_selectLanguage_Label ?? (_UE_Dropdown_selectLanguage_Label = PickText("Dropdown_selectLanguage_Label"));

	public Image UE_Dropdown_selectLanguageArrow => _UE_Dropdown_selectLanguageArrow ?? (_UE_Dropdown_selectLanguageArrow = PickImage("Dropdown_selectLanguageArrow"));

	public Image UE_item_DropdownmouseOver => _UE_item_DropdownmouseOver ?? (_UE_item_DropdownmouseOver = PickImage("item_DropdownmouseOver"));

	public TMP_Text UE_LANGUAGE => _UE_LANGUAGE ?? (_UE_LANGUAGE = PickText("LANGUAGE"));

	public Image UE_FRAMERATE_mouseover => _UE_FRAMERATE_mouseover ?? (_UE_FRAMERATE_mouseover = PickImage("FRAMERATE_mouseover"));

	public Image UE_Framerate_select => _UE_Framerate_select ?? (_UE_Framerate_select = PickImage("Framerate_select"));

	public TMP_Text UE_Framerate_select_Label => _UE_Framerate_select_Label ?? (_UE_Framerate_select_Label = PickText("Framerate_select_Label"));

	public Image UE_Framerate_selectArrow => _UE_Framerate_selectArrow ?? (_UE_Framerate_selectArrow = PickImage("Framerate_selectArrow"));

	public Image UE_item_YmouseOver => _UE_item_YmouseOver ?? (_UE_item_YmouseOver = PickImage("item_YmouseOver"));

	public TMP_Text UE_FRAMERATE => _UE_FRAMERATE ?? (_UE_FRAMERATE = PickText("FRAMERATE"));

	public Image UE_RESOLUTION_mouseover => _UE_RESOLUTION_mouseover ?? (_UE_RESOLUTION_mouseover = PickImage("RESOLUTION_mouseover"));

	public Image UE_Resolution_select => _UE_Resolution_select ?? (_UE_Resolution_select = PickImage("Resolution_select"));

	public TMP_Text UE_Resolution_select_Label => _UE_Resolution_select_Label ?? (_UE_Resolution_select_Label = PickText("Resolution_select_Label"));

	public Image UE_Resolution_selectArrow => _UE_Resolution_selectArrow ?? (_UE_Resolution_selectArrow = PickImage("Resolution_selectArrow"));

	public Image UE_item_RmouseOver => _UE_item_RmouseOver ?? (_UE_item_RmouseOver = PickImage("item_RmouseOver"));

	public TMP_Text UE_RESOLUTION => _UE_RESOLUTION ?? (_UE_RESOLUTION = PickText("RESOLUTION"));

	public Image UE_Vsync_mouseover => _UE_Vsync_mouseover ?? (_UE_Vsync_mouseover = PickImage("Vsync_mouseover"));

	public Button UE_VsyncLeft => _UE_VsyncLeft ?? (_UE_VsyncLeft = PickButton("VsyncLeft"));

	public Action<string> OnVsyncLeft
	{
		get
		{
			return _OnVsyncLeft;
		}
		set
		{
			_OnVsyncLeft = value;
			SetOnButtonClick("VsyncLeft", value);
		}
	}

	public Image UE_VsyncLeftArrow => _UE_VsyncLeftArrow ?? (_UE_VsyncLeftArrow = PickImage("VsyncLeftArrow"));

	public TMP_Text UE_VsyncSelected => _UE_VsyncSelected ?? (_UE_VsyncSelected = PickText("VsyncSelected"));

	public Button UE_VsyncRight => _UE_VsyncRight ?? (_UE_VsyncRight = PickButton("VsyncRight"));

	public Action<string> OnVsyncRight
	{
		get
		{
			return _OnVsyncRight;
		}
		set
		{
			_OnVsyncRight = value;
			SetOnButtonClick("VsyncRight", value);
		}
	}

	public Image UE_VsyncRightArrow => _UE_VsyncRightArrow ?? (_UE_VsyncRightArrow = PickImage("VsyncRightArrow"));

	public TMP_Text UE_Vsync => _UE_Vsync ?? (_UE_Vsync = PickText("Vsync"));

	public Button UE_CreatorCode => _UE_CreatorCode ?? (_UE_CreatorCode = PickButton("CreatorCode"));

	public Action<string> OnCreatorCode
	{
		get
		{
			return _OnCreatorCode;
		}
		set
		{
			_OnCreatorCode = value;
			SetOnButtonClick("CreatorCode", value);
		}
	}

	public Image UE_CreatorCode_mouseover => _UE_CreatorCode_mouseover ?? (_UE_CreatorCode_mouseover = PickImage("CreatorCode_mouseover"));

	public TMP_Text UE_CreateCodeText => _UE_CreateCodeText ?? (_UE_CreateCodeText = PickText("CreateCodeText"));

	public TMP_Text UE_lastInputedCreatorCode => _UE_lastInputedCreatorCode ?? (_UE_lastInputedCreatorCode = PickText("lastInputedCreatorCode"));

	public event OnMouseOverTargetChanged onMouseOverTargetChanged;

	private void Start()
	{
		_microphoneCapture = Hub.s.voiceman.GetComponent<BasicMicrophoneCapture>();
		UE_ResetButton.onClick.AddListener(delegate
		{
			if (!(this == null) && base.gameObject.activeInHierarchy)
			{
				ResetSetting();
			}
		});
		UE_BackButton.onClick.AddListener(delegate
		{
			if (!(this == null) && base.gameObject.activeInHierarchy)
			{
				Hub.s.uiman.CloseGameSettings();
			}
		});
		UE_CreatorCode.onClick.AddListener(delegate
		{
			if (!(this == null) && base.gameObject.activeInHierarchy)
			{
				Hub.s.uiman.OpenCreatorCodeUI(UE_lastInputedCreatorCode);
			}
		});
		UE_lastInputedCreatorCode.text = Hub.s.kosManager.lastCreatorCode;
		AddMouseOverEnterEvent(UE_BackButton.gameObject, new List<Image> { UE_CLOSE_mouseover }, new List<TMP_Text> { UE_BackButton.GetComponentInChildren<TMP_Text>() });
		AddMouseOverEnterEvent(UE_ResetButton.gameObject, new List<Image> { UE_RESET_mouseover }, new List<TMP_Text> { UE_ResetButton.GetComponentInChildren<TMP_Text>() });
		AddMouseOverExitEvent(UE_BackButton.gameObject, new List<Image> { UE_CLOSE_mouseover }, new List<TMP_Text> { UE_BackButton.GetComponentInChildren<TMP_Text>() });
		AddMouseOverExitEvent(UE_ResetButton.gameObject, new List<Image> { UE_RESET_mouseover }, new List<TMP_Text> { UE_ResetButton.GetComponentInChildren<TMP_Text>() });
		AddMouseOverEnterEvent(UE_CreatorCode.gameObject, new List<Image> { UE_CreatorCode_mouseover }, new List<TMP_Text> { UE_CreateCodeText });
		AddMouseOverExitEvent(UE_CreatorCode.gameObject, new List<Image> { UE_CreatorCode_mouseover }, new List<TMP_Text> { UE_CreateCodeText });
		UE_MasterVolume_Slider.GetComponent<Slider>().onValueChanged.AddListener(delegate(float value)
		{
			if (!(this == null) && base.gameObject.activeInHierarchy)
			{
				OnValueChangedMasterVolume(value);
			}
		});
		UE_MicVolume_Slider.GetComponent<Slider>().onValueChanged.AddListener(delegate(float value)
		{
			if (!(this == null) && base.gameObject.activeInHierarchy)
			{
				OnValueChangedMicVolume(value);
			}
		});
		AddMouseOverEnterEvent(UE_DEVICE_INDEX_mouseover.gameObject, new List<Image> { UE_DEVICE_INDEX_mouseover }, new List<TMP_Text> { UE_DEVICE_INDEX, UE_Dropdown_selectMic_Label, UE_Dropdown_selectMic_Label }, delegate
		{
			UE_Dropdown_selectMicArrow.color = Hub.s.uiman.mouseOverTextColor;
		});
		AddMouseOverEnterEvent(UE_Voice_MODE_mouseover.gameObject, new List<Image> { UE_Voice_MODE_mouseover }, new List<TMP_Text> { UE_VOICE_MODE, UE_voiceSelected }, delegate
		{
			UE_voicePrevArrow.color = Hub.s.uiman.mouseOverTextColor;
			UE_voiceNextArrow.color = Hub.s.uiman.mouseOverTextColor;
			SliderMouseOverImage(UE_volumeGauge.transform, over: true, isLong: true);
		});
		AddMouseOverEnterEvent(UE_MASTER_VOLUME_mouseover.gameObject, new List<Image> { UE_MASTER_VOLUME_mouseover }, new List<TMP_Text> { UE_MASTER_VOLUME }, delegate
		{
			SliderMouseOverImage(UE_MasterVolume_Slider, over: true);
		});
		AddMouseOverEnterEvent(UE_MIC_VOLUME_mouseover.gameObject, new List<Image> { UE_MIC_VOLUME_mouseover }, new List<TMP_Text> { UE_Mic_VOLUME }, delegate
		{
			SliderMouseOverImage(UE_MicVolume_Slider, over: true);
		});
		AddMouseOverEnterEvent(UE_Dropdown_selectMic.gameObject, new List<Image> { UE_DEVICE_INDEX_mouseover }, new List<TMP_Text> { UE_DEVICE_INDEX, UE_Dropdown_selectMic_Label, UE_Dropdown_selectMic_Label }, delegate
		{
			UE_Dropdown_selectMicArrow.color = Hub.s.uiman.mouseOverTextColor;
		});
		AddMouseOverEnterEvent(UE_voicePrev.gameObject, new List<Image> { UE_Voice_MODE_mouseover }, new List<TMP_Text> { UE_VOICE_MODE, UE_voiceSelected }, delegate
		{
			UE_voicePrevArrow.color = Hub.s.uiman.mouseOverTextColor;
			UE_voiceNextArrow.color = Hub.s.uiman.mouseOverTextColor;
		});
		AddMouseOverEnterEvent(UE_voiceNext.gameObject, new List<Image> { UE_Voice_MODE_mouseover }, new List<TMP_Text> { UE_VOICE_MODE, UE_voiceSelected }, delegate
		{
			UE_voicePrevArrow.color = Hub.s.uiman.mouseOverTextColor;
			UE_voiceNextArrow.color = Hub.s.uiman.mouseOverTextColor;
		});
		AddMouseOverEnterEvent(UE_MasterVolume_Slider.gameObject, new List<Image> { UE_MASTER_VOLUME_mouseover }, new List<TMP_Text> { UE_MASTER_VOLUME }, delegate
		{
			SliderMouseOverImage(UE_MasterVolume_Slider, over: true);
		});
		AddMouseOverEnterEvent(UE_MicVolume_Slider.gameObject, new List<Image> { UE_MIC_VOLUME_mouseover }, new List<TMP_Text> { UE_Mic_VOLUME }, delegate
		{
			SliderMouseOverImage(UE_MicVolume_Slider, over: true);
		});
		AddMouseOverExitEvent(UE_DEVICE_INDEX_mouseover.gameObject, new List<Image> { UE_DEVICE_INDEX_mouseover }, new List<TMP_Text> { UE_DEVICE_INDEX, UE_Dropdown_selectMic_Label, UE_Dropdown_selectMic_Label }, delegate
		{
			UE_Dropdown_selectMicArrow.color = Color.white;
		});
		AddMouseOverExitEvent(UE_Voice_MODE_mouseover.gameObject, new List<Image> { UE_Voice_MODE_mouseover }, new List<TMP_Text> { UE_VOICE_MODE, UE_voiceSelected }, delegate
		{
			UE_voicePrevArrow.color = Color.white;
			UE_voiceNextArrow.color = Color.white;
			SliderMouseOverImage(UE_volumeGauge.transform, over: false, isLong: true);
		});
		AddMouseOverExitEvent(UE_MASTER_VOLUME_mouseover.gameObject, new List<Image> { UE_MASTER_VOLUME_mouseover }, new List<TMP_Text> { UE_MASTER_VOLUME }, delegate
		{
			SliderMouseOverImage(UE_MasterVolume_Slider, over: false);
		});
		AddMouseOverExitEvent(UE_MIC_VOLUME_mouseover.gameObject, new List<Image> { UE_MIC_VOLUME_mouseover }, new List<TMP_Text> { UE_Mic_VOLUME }, delegate
		{
			SliderMouseOverImage(UE_MicVolume_Slider, over: false);
		});
		AddMouseOverExitEvent(UE_Dropdown_selectMic.gameObject, new List<Image> { UE_DEVICE_INDEX_mouseover }, new List<TMP_Text> { UE_DEVICE_INDEX, UE_Dropdown_selectMic_Label, UE_Dropdown_selectMic_Label }, delegate
		{
			UE_Dropdown_selectMicArrow.color = Color.white;
		});
		AddMouseOverExitEvent(UE_voicePrev.gameObject, new List<Image> { UE_Voice_MODE_mouseover }, new List<TMP_Text> { UE_VOICE_MODE, UE_voiceSelected }, delegate
		{
			UE_voicePrevArrow.color = Color.white;
			UE_voiceNextArrow.color = Color.white;
		});
		AddMouseOverExitEvent(UE_voiceNext.gameObject, new List<Image> { UE_Voice_MODE_mouseover }, new List<TMP_Text> { UE_VOICE_MODE, UE_voiceSelected }, delegate
		{
			UE_voicePrevArrow.color = Color.white;
			UE_voiceNextArrow.color = Color.white;
		});
		UE_voicePrev.onClick.AddListener(delegate
		{
			if (!(this == null) && base.gameObject.activeInHierarchy)
			{
				PrevVoiceMode();
			}
		});
		UE_voiceNext.onClick.AddListener(delegate
		{
			if (!(this == null) && base.gameObject.activeInHierarchy)
			{
				NextVoiceMode();
			}
		});
		UE_YNext.onClick.AddListener(delegate
		{
			if (!(this == null) && base.gameObject.activeInHierarchy)
			{
				InvertYAxis();
			}
		});
		UE_YPrev.onClick.AddListener(delegate
		{
			if (!(this == null) && base.gameObject.activeInHierarchy)
			{
				InvertYAxis();
			}
		});
		UE_LookSensitivity_Slider.GetComponent<Slider>().onValueChanged.AddListener(delegate(float value)
		{
			if (!(this == null) && base.gameObject.activeInHierarchy)
			{
				OnValueChangedMouseSensitivity(value);
			}
		});
		SetMaxFramerate();
		UE_ChangeKeyBindsButton.onClick.AddListener(delegate
		{
			if (!(this == null) && base.gameObject.activeInHierarchy)
			{
				Hub.s.uiman.OpenChangeKeyBinding();
			}
		});
		UE_ChangeKeyBindsGamepadButton.onClick.AddListener(delegate
		{
			if (!(this == null) && base.gameObject.activeInHierarchy)
			{
				Hub.s.uiman.OpenChangeKeyBinding(gamepad: true);
			}
		});
		AddMouseOverEnterEvent(UE_LOOK_SENSITIVITY_mouseover.gameObject, new List<Image> { UE_LOOK_SENSITIVITY_mouseover }, new List<TMP_Text> { UE_LOOK_SENSITIVITY }, delegate
		{
			SliderMouseOverImage(UE_LookSensitivity_Slider, over: true);
		});
		AddMouseOverEnterEvent(UE_LookSensitivity_Slider.gameObject, new List<Image> { UE_LOOK_SENSITIVITY_mouseover }, new List<TMP_Text> { UE_LOOK_SENSITIVITY }, delegate
		{
			SliderMouseOverImage(UE_LookSensitivity_Slider, over: true);
		});
		AddMouseOverEnterEvent(UE_INBERT_Y_AXIS_mouseover.gameObject, new List<Image> { UE_INBERT_Y_AXIS_mouseover }, new List<TMP_Text> { UE_INVERT_YAXIS }, delegate
		{
			UE_YPrevArrow.color = Hub.s.uiman.mouseOverTextColor;
			UE_YNextArrow.color = Hub.s.uiman.mouseOverTextColor;
			UE_YSelected.color = Hub.s.uiman.mouseOverTextColor;
		});
		AddMouseOverEnterEvent(UE_FRAMERATE_mouseover.gameObject, new List<Image> { UE_FRAMERATE_mouseover }, new List<TMP_Text> { UE_FRAMERATE }, delegate
		{
			UE_Framerate_selectArrow.color = Hub.s.uiman.mouseOverTextColor;
			UE_Framerate_select_Label.color = Hub.s.uiman.mouseOverTextColor;
		});
		AddMouseOverEnterEvent(UE_RESOLUTION_mouseover.gameObject, new List<Image> { UE_RESOLUTION_mouseover }, new List<TMP_Text> { UE_RESOLUTION }, delegate
		{
			UE_Resolution_selectArrow.color = Hub.s.uiman.mouseOverTextColor;
			UE_Resolution_select_Label.color = Hub.s.uiman.mouseOverTextColor;
		});
		AddMouseOverEnterEvent(UE_YPrev.gameObject, new List<Image> { UE_INBERT_Y_AXIS_mouseover }, new List<TMP_Text> { UE_INVERT_YAXIS }, delegate
		{
			UE_YPrevArrow.color = Hub.s.uiman.mouseOverTextColor;
			UE_YNextArrow.color = Hub.s.uiman.mouseOverTextColor;
		});
		AddMouseOverEnterEvent(UE_YNext.gameObject, new List<Image> { UE_INBERT_Y_AXIS_mouseover }, new List<TMP_Text> { UE_INVERT_YAXIS }, delegate
		{
			UE_YPrevArrow.color = Hub.s.uiman.mouseOverTextColor;
			UE_YNextArrow.color = Hub.s.uiman.mouseOverTextColor;
		});
		AddMouseOverEnterEvent(UE_Framerate_select.gameObject, new List<Image> { UE_FRAMERATE_mouseover }, new List<TMP_Text> { UE_FRAMERATE }, delegate
		{
			UE_Framerate_selectArrow.color = Hub.s.uiman.mouseOverTextColor;
			UE_Framerate_select_Label.color = Hub.s.uiman.mouseOverTextColor;
		});
		AddMouseOverEnterEvent(UE_Resolution_select.gameObject, new List<Image> { UE_RESOLUTION_mouseover }, new List<TMP_Text> { UE_RESOLUTION }, delegate
		{
			UE_Resolution_selectArrow.color = Hub.s.uiman.mouseOverTextColor;
			UE_Resolution_select_Label.color = Hub.s.uiman.mouseOverTextColor;
		});
		AddMouseOverEnterEvent(UE_KeyBinds_mouseover.gameObject, new List<Image> { UE_KeyBinds_mouseover }, new List<TMP_Text> { UE_KEYBINDS }, delegate
		{
			UE_KEYBINDS.color = Hub.s.uiman.mouseOverTextColor;
			UE_ChangeKeyBindsButton.image.color = Hub.s.uiman.mouseOverTextColor;
			UE_ChangeKeyBindsButton.GetComponentInChildren<TextMeshProUGUI>().color = Hub.s.uiman.mouseOverTextColor;
		});
		AddMouseOverEnterEvent(UE_KeyBindsGamepad_mouseover.gameObject, new List<Image> { UE_KeyBindsGamepad_mouseover }, new List<TMP_Text> { UE_KEYBINDSGAMEPAD }, delegate
		{
			UE_KEYBINDSGAMEPAD.color = Hub.s.uiman.mouseOverTextColor;
			UE_ChangeKeyBindsGamepadButton.image.color = Hub.s.uiman.mouseOverTextColor;
			UE_ChangeKeyBindsGamepadButton.GetComponentInChildren<TextMeshProUGUI>().color = Hub.s.uiman.mouseOverTextColor;
			UE_KeybindsGamepadRightArrow.color = Hub.s.uiman.mouseOverTextColor;
		});
		AddMouseOverEnterEvent(UE_ChangeKeyBindsButton.gameObject, new List<Image> { UE_KeyBinds_mouseover }, new List<TMP_Text> { UE_KEYBINDS }, delegate
		{
			UE_KEYBINDS.color = Hub.s.uiman.mouseOverTextColor;
			UE_ChangeKeyBindsButton.image.color = Hub.s.uiman.mouseOverTextColor;
			UE_ChangeKeyBindsButton.GetComponentInChildren<TextMeshProUGUI>().color = Hub.s.uiman.mouseOverTextColor;
			UE_KeybindsRightArrow.color = Hub.s.uiman.mouseOverTextColor;
		});
		AddMouseOverEnterEvent(UE_ChangeKeyBindsGamepadButton.gameObject, new List<Image> { UE_KeyBindsGamepad_mouseover }, new List<TMP_Text> { UE_KEYBINDSGAMEPAD }, delegate
		{
			UE_KEYBINDSGAMEPAD.color = Hub.s.uiman.mouseOverTextColor;
			UE_ChangeKeyBindsGamepadButton.image.color = Hub.s.uiman.mouseOverTextColor;
			UE_ChangeKeyBindsGamepadButton.GetComponentInChildren<TextMeshProUGUI>().color = Hub.s.uiman.mouseOverTextColor;
			UE_KeybindsGamepadRightArrow.color = Hub.s.uiman.mouseOverTextColor;
		});
		AddMouseOverExitEvent(UE_LOOK_SENSITIVITY_mouseover.gameObject, new List<Image> { UE_LOOK_SENSITIVITY_mouseover }, new List<TMP_Text> { UE_LOOK_SENSITIVITY }, delegate
		{
			SliderMouseOverImage(UE_LookSensitivity_Slider, over: false);
		});
		AddMouseOverExitEvent(UE_INBERT_Y_AXIS_mouseover.gameObject, new List<Image> { UE_INBERT_Y_AXIS_mouseover }, new List<TMP_Text> { UE_INVERT_YAXIS }, delegate
		{
			UE_YPrevArrow.color = Color.white;
			UE_YNextArrow.color = Color.white;
			UE_YSelected.color = Color.white;
		});
		AddMouseOverExitEvent(UE_FRAMERATE_mouseover.gameObject, new List<Image> { UE_FRAMERATE_mouseover }, new List<TMP_Text> { UE_FRAMERATE }, delegate
		{
			UE_Framerate_selectArrow.color = Color.white;
			UE_Framerate_select_Label.color = Color.white;
		});
		AddMouseOverExitEvent(UE_RESOLUTION_mouseover.gameObject, new List<Image> { UE_RESOLUTION_mouseover }, new List<TMP_Text> { UE_RESOLUTION }, delegate
		{
			UE_Resolution_selectArrow.color = Color.white;
			UE_Resolution_select_Label.color = Color.white;
		});
		AddMouseOverExitEvent(UE_YPrev.gameObject, new List<Image> { UE_INBERT_Y_AXIS_mouseover }, new List<TMP_Text> { UE_INVERT_YAXIS }, delegate
		{
			UE_YPrevArrow.color = Color.white;
			UE_YNextArrow.color = Color.white;
		});
		AddMouseOverExitEvent(UE_YNext.gameObject, new List<Image> { UE_INBERT_Y_AXIS_mouseover }, new List<TMP_Text> { UE_INVERT_YAXIS }, delegate
		{
			UE_YPrevArrow.color = Color.white;
			UE_YNextArrow.color = Color.white;
		});
		AddMouseOverExitEvent(UE_Framerate_select.gameObject, new List<Image> { UE_FRAMERATE_mouseover }, new List<TMP_Text> { UE_FRAMERATE }, delegate
		{
			UE_Framerate_selectArrow.color = Color.white;
			UE_Framerate_select_Label.color = Color.white;
		});
		AddMouseOverExitEvent(UE_RESOLUTION_mouseover.gameObject, new List<Image> { UE_RESOLUTION_mouseover }, new List<TMP_Text> { UE_RESOLUTION }, delegate
		{
			UE_Resolution_selectArrow.color = Color.white;
			UE_Resolution_select_Label.color = Color.white;
		});
		AddMouseOverExitEvent(UE_KeyBinds_mouseover.gameObject, new List<Image> { UE_KeyBinds_mouseover }, new List<TMP_Text> { UE_KEYBINDS }, delegate
		{
			UE_KEYBINDS.color = Color.white;
			UE_ChangeKeyBindsButton.image.color = Color.white;
			UE_ChangeKeyBindsButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
			UE_KeybindsRightArrow.color = Color.white;
		});
		AddMouseOverExitEvent(UE_ChangeKeyBindsButton.gameObject, new List<Image> { UE_KeyBinds_mouseover }, new List<TMP_Text> { UE_KEYBINDS }, delegate
		{
			UE_KEYBINDS.color = Color.white;
			UE_ChangeKeyBindsButton.image.color = Color.white;
			UE_ChangeKeyBindsButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
			UE_KeybindsRightArrow.color = Color.white;
		});
		AddMouseOverExitEvent(UE_ChangeKeyBindsGamepadButton.gameObject, new List<Image> { UE_KeyBindsGamepad_mouseover }, new List<TMP_Text> { UE_KEYBINDSGAMEPAD }, delegate
		{
			UE_KEYBINDSGAMEPAD.color = Color.white;
			UE_ChangeKeyBindsButton.image.color = Color.white;
			UE_ChangeKeyBindsGamepadButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
			UE_KeybindsGamepadRightArrow.color = Color.white;
		});
		AddMouseOverExitEvent(UE_KeyBindsGamepad_mouseover.gameObject, new List<Image> { UE_KeyBindsGamepad_mouseover }, new List<TMP_Text> { UE_KEYBINDSGAMEPAD }, delegate
		{
			UE_KEYBINDSGAMEPAD.color = Color.white;
			UE_ChangeKeyBindsGamepadButton.image.color = Color.white;
			UE_ChangeKeyBindsGamepadButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
			UE_KeybindsGamepadRightArrow.color = Color.white;
		});
		UE_Brightness_Slider.GetComponent<Slider>().onValueChanged.AddListener(delegate(float value)
		{
			if (!(this == null) && base.gameObject.activeInHierarchy)
			{
				OnValueChangedBrightness(value);
			}
		});
		UE_Gamma_Slider.GetComponent<Slider>().onValueChanged.AddListener(delegate(float value)
		{
			if (!(this == null) && base.gameObject.activeInHierarchy)
			{
				OnValueChangedGamma(value);
			}
		});
		SetLanguage();
		SetDisplayMode();
		UpdateGameSettings_ScreenModeDropDown(invoke: true);
		AddMouseOverEnterEvent(UE_DISPLAY_MODE_mouseover.gameObject, new List<Image> { UE_DISPLAY_MODE_mouseover }, new List<TMP_Text> { UE_DISPLAY_MODE }, delegate
		{
			UE_Dropdown_selectDosplayArrow.color = Hub.s.uiman.mouseOverTextColor;
			UE_Dropdown_selectDisplay_Label.color = Hub.s.uiman.mouseOverTextColor;
		});
		AddMouseOverEnterEvent(UE_GAMMA_mouseover.gameObject, new List<Image> { UE_GAMMA_mouseover }, new List<TMP_Text> { UE_GAMMA }, delegate
		{
			SliderMouseOverImage(UE_Gamma_Slider, over: true);
		});
		AddMouseOverEnterEvent(UE_BRIGHTNESS_mouseover.gameObject, new List<Image> { UE_BRIGHTNESS_mouseover }, new List<TMP_Text> { UE_BRIGHTNESS }, delegate
		{
			SliderMouseOverImage(UE_Brightness_Slider, over: true);
		});
		AddMouseOverEnterEvent(UE_LANGUAGE_mouseover.gameObject, new List<Image> { UE_LANGUAGE_mouseover }, new List<TMP_Text> { UE_LANGUAGE }, delegate
		{
			UE_Dropdown_selectLanguageArrow.color = Hub.s.uiman.mouseOverTextColor;
			UE_Dropdown_selectLanguage_Label.color = Hub.s.uiman.mouseOverTextColor;
		});
		AddMouseOverEnterEvent(UE_Dropdown_selectDisplayMode.gameObject, new List<Image> { UE_DISPLAY_MODE_mouseover }, new List<TMP_Text> { UE_DISPLAY_MODE }, delegate
		{
			UE_Dropdown_selectDosplayArrow.color = Hub.s.uiman.mouseOverTextColor;
			UE_Dropdown_selectDisplay_Label.color = Hub.s.uiman.mouseOverTextColor;
		});
		AddMouseOverEnterEvent(UE_Gamma_Slider.gameObject, new List<Image> { UE_GAMMA_mouseover }, new List<TMP_Text> { UE_GAMMA }, delegate
		{
			SliderMouseOverImage(UE_Gamma_Slider, over: true);
		});
		AddMouseOverEnterEvent(UE_Brightness_Slider.gameObject, new List<Image> { UE_BRIGHTNESS_mouseover }, new List<TMP_Text> { UE_BRIGHTNESS }, delegate
		{
			SliderMouseOverImage(UE_Brightness_Slider, over: true);
		});
		AddMouseOverEnterEvent(UE_Dropdown_selectLanguage.gameObject, new List<Image> { UE_LANGUAGE_mouseover }, new List<TMP_Text> { UE_LANGUAGE }, delegate
		{
			UE_Dropdown_selectLanguageArrow.color = Hub.s.uiman.mouseOverTextColor;
			UE_Dropdown_selectLanguage_Label.color = Hub.s.uiman.mouseOverTextColor;
		});
		AddMouseOverEnterEvent(UE_Vsync_mouseover.gameObject, new List<Image> { UE_Vsync_mouseover }, new List<TMP_Text> { UE_Vsync }, delegate
		{
			UE_Vsync.color = Hub.s.uiman.mouseOverTextColor;
			UE_VsyncLeftArrow.color = Hub.s.uiman.mouseOverTextColor;
			UE_VsyncRightArrow.color = Hub.s.uiman.mouseOverTextColor;
			UE_VsyncSelected.color = Hub.s.uiman.mouseOverTextColor;
		});
		AddMouseOverEnterEvent(UE_VsyncLeft.gameObject, new List<Image> { UE_Vsync_mouseover }, new List<TMP_Text> { UE_Vsync }, delegate
		{
			UE_VsyncLeftArrow.color = Hub.s.uiman.mouseOverTextColor;
			UE_VsyncRightArrow.color = Hub.s.uiman.mouseOverTextColor;
			UE_VsyncSelected.color = Hub.s.uiman.mouseOverTextColor;
		});
		AddMouseOverEnterEvent(UE_VsyncRight.gameObject, new List<Image> { UE_Vsync_mouseover }, new List<TMP_Text> { UE_Vsync }, delegate
		{
			UE_VsyncRightArrow.color = Hub.s.uiman.mouseOverTextColor;
			UE_VsyncLeftArrow.color = Hub.s.uiman.mouseOverTextColor;
			UE_VsyncSelected.color = Hub.s.uiman.mouseOverTextColor;
		});
		AddMouseOverExitEvent(UE_DISPLAY_MODE_mouseover.gameObject, new List<Image> { UE_DISPLAY_MODE_mouseover }, new List<TMP_Text> { UE_DISPLAY_MODE }, delegate
		{
			UE_Dropdown_selectDosplayArrow.color = Color.white;
			UE_Dropdown_selectDisplay_Label.color = Color.white;
		});
		AddMouseOverExitEvent(UE_GAMMA_mouseover.gameObject, new List<Image> { UE_GAMMA_mouseover }, new List<TMP_Text> { UE_GAMMA }, delegate
		{
			SliderMouseOverImage(UE_Gamma_Slider, over: false);
		});
		AddMouseOverExitEvent(UE_BRIGHTNESS_mouseover.gameObject, new List<Image> { UE_BRIGHTNESS_mouseover }, new List<TMP_Text> { UE_BRIGHTNESS }, delegate
		{
			SliderMouseOverImage(UE_Brightness_Slider, over: false);
		});
		AddMouseOverExitEvent(UE_LANGUAGE_mouseover.gameObject, new List<Image> { UE_LANGUAGE_mouseover }, new List<TMP_Text> { UE_LANGUAGE }, delegate
		{
			UE_Dropdown_selectLanguageArrow.color = Color.white;
			UE_Dropdown_selectLanguage_Label.color = Color.white;
		});
		AddMouseOverExitEvent(UE_Dropdown_selectDisplayMode.gameObject, new List<Image> { UE_DISPLAY_MODE_mouseover }, new List<TMP_Text> { UE_DISPLAY_MODE }, delegate
		{
			UE_Dropdown_selectDosplayArrow.color = Color.white;
			UE_Dropdown_selectDisplay_Label.color = Color.white;
		});
		AddMouseOverExitEvent(UE_Dropdown_selectLanguage.gameObject, new List<Image> { UE_LANGUAGE_mouseover }, new List<TMP_Text> { UE_LANGUAGE }, delegate
		{
			UE_Dropdown_selectLanguageArrow.color = Color.white;
			UE_Dropdown_selectLanguage_Label.color = Color.white;
		});
		AddMouseOverExitEvent(UE_Vsync_mouseover.gameObject, new List<Image> { UE_Vsync_mouseover }, new List<TMP_Text> { UE_Vsync }, delegate
		{
			UE_Vsync.color = Color.white;
			UE_VsyncLeftArrow.color = Color.white;
			UE_VsyncRightArrow.color = Color.white;
			UE_VsyncSelected.color = Color.white;
		});
		AddMouseOverExitEvent(UE_VsyncLeft.gameObject, new List<Image> { UE_Vsync_mouseover }, new List<TMP_Text> { UE_Vsync }, delegate
		{
			UE_VsyncLeftArrow.color = Color.white;
			UE_VsyncRightArrow.color = Color.white;
			UE_VsyncSelected.color = Color.white;
		});
		AddMouseOverExitEvent(UE_VsyncRight.gameObject, new List<Image> { UE_Vsync_mouseover }, new List<TMP_Text> { UE_Vsync }, delegate
		{
			UE_VsyncRightArrow.color = Color.white;
			UE_VsyncLeftArrow.color = Color.white;
			UE_VsyncSelected.color = Color.white;
		});
		UE_VsyncLeft.onClick.AddListener(delegate
		{
			if (!(this == null) && base.gameObject.activeInHierarchy)
			{
				ChangeVsync();
			}
		});
		UE_VsyncRight.onClick.AddListener(delegate
		{
			if (!(this == null) && base.gameObject.activeInHierarchy)
			{
				ChangeVsync();
			}
		});
		InitializeResolutionDropdown();
	}

	private void OnEnable()
	{
		Set();
		Cursor.lockState = CursorLockMode.None;
		Hub.s.lcman.onLanguageChanged += SetDisplayMode;
		Hub.s.lcman.onLanguageChanged += SetVoiceMode;
		Hub.s.lcman.onLanguageChanged += SetInvertYAxis;
		Hub.s.lcman.onLanguageChanged += SetVsync;
		Hub.s.lcman.onLanguageChanged += SetMaxFramerate;
		AudioSettings.OnAudioConfigurationChanged += OnAudioDeviceChanged;
		if (Hub.s.kosManager.configStage == "cert")
		{
			UE_CreatorCode.transform.parent.gameObject.SetActive(value: false);
		}
		else
		{
			UE_CreatorCode.transform.parent.gameObject.SetActive(value: true);
		}
	}

	private void OnDisable()
	{
		if (SceneManager.GetActiveScene().name.Equals("MainMenuScene"))
		{
			Microphone.End(null);
			micClip = null;
			_clip = null;
		}
		if (Hub.s != null)
		{
			Hub.s.lcman.onLanguageChanged -= SetDisplayMode;
			Hub.s.lcman.onLanguageChanged -= SetVoiceMode;
			Hub.s.lcman.onLanguageChanged -= SetInvertYAxis;
			Hub.s.lcman.onLanguageChanged -= SetVsync;
			Hub.s.lcman.onLanguageChanged -= SetMaxFramerate;
		}
		AudioSettings.OnAudioConfigurationChanged -= OnAudioDeviceChanged;
	}

	private void Set()
	{
		SetMic_v2();
		SetVoiceMode();
		vsync = Hub.s.gameSettingManager.vsync;
		SetVsync();
		UE_Gamma_Slider.GetComponent<Slider>().value = Hub.s.gameSettingManager.gamma;
		UE_Brightness_Slider.GetComponent<Slider>().value = Hub.s.gameSettingManager.brightness;
		if (Hub.s.inputman.invertYAxis == 1)
		{
			invertYAxis = false;
			UE_YSelected.text = Hub.GetL10NText("UI_PREFAB_GAME_SETTINGS_Y_AXIS_OFF");
		}
		else
		{
			invertYAxis = true;
			UE_YSelected.text = Hub.GetL10NText("UI_PREFAB_GAME_SETTINGS_Y_AXIS_ON");
		}
		UE_LookSensitivity_Slider.GetComponent<Slider>().minValue = Hub.s.gameConfig.gameSetting.minMouseSensitivity;
		UE_LookSensitivity_Slider.GetComponent<Slider>().maxValue = Hub.s.gameConfig.gameSetting.maxMouseSensitivity;
		UE_LookSensitivity_Slider.GetComponent<Slider>().value = Hub.s.gameSettingManager.mouseSensitivity;
		UE_Brightness_Slider.GetComponent<Slider>().minValue = Hub.s.gameConfig.gameSetting.minBrightness;
		UE_Brightness_Slider.GetComponent<Slider>().maxValue = Hub.s.gameConfig.gameSetting.maxBrightness;
		UE_Gamma_Slider.GetComponent<Slider>().minValue = Hub.s.gameConfig.gameSetting.minGamma;
		UE_Gamma_Slider.GetComponent<Slider>().maxValue = Hub.s.gameConfig.gameSetting.maxGamma;
		UE_MasterVolume_Slider.GetComponent<Slider>().minValue = Hub.s.gameConfig.gameSetting.minMasterVolume;
		UE_MasterVolume_Slider.GetComponent<Slider>().maxValue = Hub.s.gameConfig.gameSetting.maxMasterVolume;
		UE_MicVolume_Slider.GetComponent<Slider>().minValue = Hub.s.gameConfig.gameSetting.minMicVolume;
		UE_MicVolume_Slider.GetComponent<Slider>().maxValue = Hub.s.gameConfig.gameSetting.maxMicVolume;
		UE_LookSensitivity_Slider.GetComponent<Slider>().minValue = Hub.s.gameConfig.gameSetting.minMouseSensitivity;
		UE_LookSensitivity_Slider.GetComponent<Slider>().maxValue = Hub.s.gameConfig.gameSetting.maxMouseSensitivity;
		UE_LookSensitivity_Slider.GetComponent<Slider>().value = Hub.s.gameSettingManager.mouseSensitivity;
		UE_Brightness_Slider.GetComponent<Slider>().value = Hub.s.gameSettingManager.brightness;
		UE_Gamma_Slider.GetComponent<Slider>().value = Hub.s.gameSettingManager.gamma;
		UE_MasterVolume_Slider.GetComponent<Slider>().value = Hub.s.gameSettingManager.masterVolume;
		UE_MicVolume_Slider.GetComponent<Slider>().value = Hub.s.gameSettingManager.micVolume;
	}

	public void AdjustWidthText(TMP_Text text)
	{
		text.ForceMeshUpdate();
		float preferredWidth = text.preferredWidth;
		RectTransform component = text.GetComponent<RectTransform>();
		component.sizeDelta = new Vector2(preferredWidth, component.sizeDelta.y);
	}

	public void SetMic()
	{
		DissonanceComms dissonanceComms = DissonanceFishNetComms.Instance.Comms;
		TMP_Dropdown selectMicDropdown = UE_Dropdown_selectMic.GetComponent<TMP_Dropdown>();
		List<string> list = new List<string>();
		dissonanceComms.GetMicrophoneDevices(list);
		if (SceneManager.GetActiveScene().name.Equals("MainMenuScene"))
		{
			if (list.Contains(micName))
			{
				micClip = Microphone.Start(micName, loop: true, 1, AudioSettings.outputSampleRate);
			}
			else
			{
				micClip = Microphone.Start(list[0], loop: true, 1, AudioSettings.outputSampleRate);
				micNameSelected = list[0];
				micName = list[0];
			}
		}
		dissonanceComms.MicrophoneName = micName;
		if (list.Count == 0)
		{
			list.Add("No microphone");
			return;
		}
		selectMicDropdown.onValueChanged.AddListener(delegate(int value)
		{
			if (value < 0 || value >= selectMicDropdown.options.Count)
			{
				Logger.RError("GameSettings::SetMic Invalid Dropdown Value: " + value);
			}
			else
			{
				string text = selectMicDropdown.options[value].text;
				dissonanceComms.MicrophoneName = text;
				micName = text;
				if (SceneManager.GetActiveScene().name.Equals("MainMenuScene"))
				{
					micClip = Microphone.Start(micName, loop: true, 1, AudioSettings.outputSampleRate);
				}
			}
		});
		if (selectMicDropdown.options.Count != list.Count || selectMicDropdown.options.Count == 0)
		{
			selectMicDropdown.ClearOptions();
			selectMicDropdown.AddOptions(list);
		}
		TMP_Text captionText = selectMicDropdown.captionText;
		float num = 0f;
		foreach (string item in list)
		{
			Vector2 vector = captionText.GetPreferredValues(item) + new Vector2(17f, 0f);
			if (vector.x > num)
			{
				num = vector.x;
			}
		}
		float num2 = 40f;
		float a = num + num2;
		RectTransform component = UE_DEVICE_INDEX_mouseover.GetComponent<RectTransform>();
		RectTransform component2 = UE_DEVICE_INDEX.GetComponent<RectTransform>();
		float b = Mathf.Abs(component.sizeDelta.x - component2.sizeDelta.x);
		float x = Mathf.Min(a, b);
		RectTransform component3 = selectMicDropdown.GetComponent<RectTransform>();
		component3.sizeDelta = new Vector2(x, component3.sizeDelta.y);
		RectTransform template = selectMicDropdown.template;
		int count = selectMicDropdown.options.Count;
		int num3 = ((count >= 9) ? 8 : count);
		float y = 33f * (float)num3;
		template.sizeDelta = new Vector2(template.sizeDelta.x, y);
	}

	private void OnAudioDeviceChanged(bool deviceWasChanged)
	{
		if (deviceWasChanged)
		{
			StartCoroutine(RefreshMicrophoneWithDelay());
		}
	}

	private IEnumerator RefreshMicrophoneWithDelay()
	{
		yield return new WaitForSeconds(0.5f);
	}

	public void SetMic_v2()
	{
		DissonanceComms comms = DissonanceFishNetComms.Instance.Comms;
		TMP_Dropdown component = UE_Dropdown_selectMic.GetComponent<TMP_Dropdown>();
		List<string> microphoneDevices = GetMicrophoneDevices(comms);
		InitializeMicrophoneInMainMenu(microphoneDevices);
		SetDissonanceMicrophoneName(comms, micName);
		SetupMicrophoneDropdown(component, microphoneDevices, comms);
		string[] devices = Microphone.devices;
		Logger.RLog("Microphone.devices");
		for (int i = 0; i < devices.Length; i++)
		{
			Logger.RLog(devices[i]);
		}
		Logger.RLog("dissonanceComms.GetMicrophoneDevices()");
		for (int j = 0; j < microphoneDevices.Count; j++)
		{
			Logger.RLog(microphoneDevices[j]);
		}
	}

	private List<string> GetMicrophoneDevices(DissonanceComms dissonanceComms)
	{
		List<string> list = new List<string>();
		dissonanceComms.GetMicrophoneDevices(list);
		if (list.Count == 0)
		{
			list.Add("No microphone");
		}
		return list;
	}

	private void InitializeMicrophoneInMainMenu(List<string> micNames)
	{
		if (SceneManager.GetActiveScene().name.Equals("MainMenuScene"))
		{
			if (micNames[0] == "No microphone")
			{
				micClip = null;
				micNameSelected = "No microphone";
				micName = "No microphone";
			}
			else if (micNames.Contains(micName))
			{
				micClip = Microphone.Start(micName, loop: true, 1, AudioSettings.outputSampleRate);
			}
			else
			{
				string deviceName = micNames[0];
				micClip = Microphone.Start(deviceName, loop: true, 1, AudioSettings.outputSampleRate);
				micNameSelected = deviceName;
				micName = deviceName;
			}
		}
	}

	private void SetDissonanceMicrophoneName(DissonanceComms dissonanceComms, string microphoneName)
	{
		dissonanceComms.MicrophoneName = microphoneName;
	}

	private void SetupMicrophoneDropdown(TMP_Dropdown selectMicDropdown, List<string> micNames, DissonanceComms dissonanceComms)
	{
		if (micNames.Count == 0 || micNames[0] == "No microphone")
		{
			Debug.Log("No microphone");
			return;
		}
		SetupDropdownOptions(selectMicDropdown, micNames);
		SetupDropdownEventListener(selectMicDropdown, dissonanceComms);
		SetupDropdownMarquee(selectMicDropdown);
	}

	private void SetupDropdownMarquee(TMP_Dropdown dropdown)
	{
		if (dropdown.captionText != null)
		{
			TextMeshProUGUI textMeshProUGUI = dropdown.captionText as TextMeshProUGUI;
			RectTransform rectTransform = textMeshProUGUI.rectTransform;
			if (rectTransform.parent.GetComponent<RectMask2D>() == null)
			{
				rectTransform.parent.gameObject.AddComponent<RectMask2D>();
			}
			if (textMeshProUGUI.GetComponent<MarqueeText>() == null)
			{
				textMeshProUGUI.gameObject.AddComponent<MarqueeText>().speed = marqueeSpeed;
			}
		}
		Transform transform = dropdown.template.Find("Viewport/Content");
		if (transform != null && transform.GetComponent<RectMask2D>() == null)
		{
			transform.gameObject.AddComponent<RectMask2D>();
		}
		TextMeshProUGUI textMeshProUGUI2 = dropdown.template.Find("Viewport/Content/Item/Item Label")?.GetComponent<TextMeshProUGUI>();
		if (textMeshProUGUI2 != null && textMeshProUGUI2.GetComponent<MarqueeText>() == null)
		{
			textMeshProUGUI2.gameObject.AddComponent<MarqueeText>().speed = marqueeSpeed;
		}
	}

	private void SetupDropdownOptions(TMP_Dropdown selectMicDropdown, List<string> micNames)
	{
		if (selectMicDropdown.options.Count != micNames.Count || selectMicDropdown.options.Count <= 1)
		{
			selectMicDropdown.ClearOptions();
			selectMicDropdown.AddOptions(micNames);
		}
	}

	private void SetupDropdownEventListener(TMP_Dropdown selectMicDropdown, DissonanceComms dissonanceComms)
	{
		selectMicDropdown.onValueChanged.AddListener(delegate(int value)
		{
			if (value < 0 || value >= selectMicDropdown.options.Count)
			{
				Logger.RLog("GameSettings::SetMic Invalid Dropdown Value: " + value);
			}
			else
			{
				string text = selectMicDropdown.options[value].text;
				dissonanceComms.MicrophoneName = text;
				micName = text;
				if (SceneManager.GetActiveScene().name.Equals("MainMenuScene"))
				{
					if (!Array.Exists(Microphone.devices, (string d) => d == micName))
					{
						Logger.RLog("GameSettings::SetMic 마이크를 찾을 수 없습니다: " + micName);
					}
					else
					{
						if (Microphone.IsRecording(micName))
						{
							Microphone.End(micName);
						}
						micClip = Microphone.Start(micName, loop: true, 1, AudioSettings.outputSampleRate);
					}
				}
			}
		});
	}

	private void AdjustDropdownSize(TMP_Dropdown selectMicDropdown, List<string> micNames)
	{
		TMP_Text captionText = selectMicDropdown.captionText;
		float num = CalculateMaxTextWidth(captionText, micNames);
		float num2 = 40f;
		float a = num + num2;
		float b = CalculateMaxAllowedWidth();
		float x = Mathf.Min(a, b);
		RectTransform component = selectMicDropdown.transform.GetChild(2).GetComponent<RectTransform>();
		component.sizeDelta = new Vector2(x, component.sizeDelta.y);
	}

	private float CalculateMaxTextWidth(TMP_Text captionText, List<string> micNames)
	{
		float num = 0f;
		foreach (string micName in micNames)
		{
			Vector2 vector = captionText.GetPreferredValues(micName) + new Vector2(17f, 0f);
			if (vector.x > num)
			{
				num = vector.x;
			}
		}
		return num;
	}

	private float CalculateMaxAllowedWidth()
	{
		RectTransform component = UE_DEVICE_INDEX_mouseover.GetComponent<RectTransform>();
		RectTransform component2 = UE_DEVICE_INDEX.GetComponent<RectTransform>();
		return Mathf.Abs(component.sizeDelta.x - component2.sizeDelta.x);
	}

	private void AdjustDropdownTemplateHeight(TMP_Dropdown selectMicDropdown)
	{
		RectTransform template = selectMicDropdown.template;
		int count = selectMicDropdown.options.Count;
		int num = ((count >= 9) ? 8 : count);
		float y = 33f * (float)num;
		template.sizeDelta = new Vector2(template.sizeDelta.x, y);
	}

	private string GetEllipsizedText(string originalText, float maxWidth, TMP_Text textComponent)
	{
		if (textComponent.GetPreferredValues(originalText).x <= maxWidth)
		{
			return originalText;
		}
		string text = "...";
		for (int num = originalText.Length; num > 0; num--)
		{
			string text2 = originalText.Substring(0, num) + text;
			if (textComponent.GetPreferredValues(text2).x <= maxWidth)
			{
				return text2;
			}
		}
		return text;
	}

	private void AddMouseOverEnterEvent(GameObject go, List<Image> targetImage, List<TMP_Text> textTarget, MouseOverDelegate @delegate = null)
	{
		EventTrigger eventTrigger = go.GetComponent<EventTrigger>();
		if (eventTrigger == null)
		{
			eventTrigger = go.AddComponent<EventTrigger>();
		}
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerEnter;
		entry.callback.AddListener(delegate
		{
			if (enableUISound && !string.IsNullOrEmpty(buttonHoverSfxId) && Application.isFocused)
			{
				Hub.s.audioman.PlaySfx(buttonHoverSfxId);
			}
			this.onMouseOverTargetChanged?.Invoke();
			targetImage?.ForEach(delegate(Image List)
			{
				List.color = new Color(1f, 1f, 1f, 1f);
			});
			textTarget?.ForEach(delegate(TMP_Text List)
			{
				List.color = Hub.s.uiman.mouseOverTextColor;
			});
			if (@delegate != null)
			{
				@delegate();
			}
		});
		eventTrigger.triggers.Add(entry);
	}

	private void AddMouseOverExitEvent(GameObject go, List<Image> targetImage, List<TMP_Text> textTarget, MouseOverDelegate @delegate = null)
	{
		EventTrigger eventTrigger = go.GetComponent<EventTrigger>();
		if (eventTrigger == null)
		{
			eventTrigger = go.AddComponent<EventTrigger>();
		}
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerExit;
		entry.callback.AddListener(delegate
		{
			targetImage?.ForEach(delegate(Image List)
			{
				List.color = new Color(1f, 1f, 1f, 0f);
			});
			textTarget?.ForEach(delegate(TMP_Text List)
			{
				List.color = Color.white;
			});
			if (@delegate != null)
			{
				@delegate();
			}
		});
		onMouseOverTargetChanged += delegate
		{
			if (go.GetComponent<Image>().raycastTarget)
			{
				targetImage?.ForEach(delegate(Image List)
				{
					List.color = new Color(1f, 1f, 1f, 0f);
				});
				textTarget?.ForEach(delegate(TMP_Text List)
				{
					List.color = Color.white;
				});
				if (@delegate != null)
				{
					@delegate();
				}
			}
		};
		eventTrigger.triggers.Add(entry);
	}

	private void SliderMouseOverImage(Transform slider, bool over, bool isLong = false)
	{
		if (over)
		{
			if (isLong)
			{
				slider.parent.GetComponent<Image>().sprite = sliderSelectedLongSprite;
				slider.GetComponent<Image>().sprite = sliderGuageSelectedLongSprite;
			}
			else
			{
				slider.transform.GetChild(0).GetComponent<Image>().sprite = sliderSelectedSprite;
				slider.GetComponent<Slider>().fillRect.GetComponent<Image>().sprite = sliderGuageSelectedSprite;
			}
		}
		else if (isLong)
		{
			slider.parent.GetComponent<Image>().sprite = sliderNormalLongSprite;
			slider.GetComponent<Image>().sprite = sliderGaugeNormalLongSprite;
		}
		else
		{
			slider.transform.GetChild(0).GetComponent<Image>().sprite = sliderNormalSprite;
			slider.GetComponent<Slider>().fillRect.GetComponent<Image>().sprite = sliderGaugeNormalSprite;
		}
	}

	private void SetVoiceMode()
	{
		Hub.s.voiceman.SetTalkMode(voiceMode);
		Hub.s.gameSettingManager.voiceMode = voiceMode;
		switch (voiceMode)
		{
		case CommActivationMode.Open:
			UE_voiceSelected.text = Hub.GetL10NText("UI_PREFAB_GAME_SETTINGS_AUDIO_OPEN");
			break;
		case CommActivationMode.VoiceActivation:
			UE_voiceSelected.text = Hub.GetL10NText("UI_PREFAB_GAME_SETTINGS_AUDIO_VOICE_ACTIVATION");
			break;
		case CommActivationMode.PushToTalk:
			UE_voiceSelected.text = Hub.GetL10NText("UI_PREFAB_GAME_SETTINGS_AUDIO_PUSH_TO_TALK");
			break;
		case CommActivationMode.None:
			UE_voiceSelected.text = Hub.GetL10NText("UI_PREFAB_GAME_SETTINGS_AUDIO_MUTE");
			break;
		}
	}

	private void PrevVoiceMode()
	{
		int num = (int)(voiceMode - 1);
		if (num < 0)
		{
			num = 2;
		}
		voiceMode = (CommActivationMode)num;
		SetVoiceMode();
	}

	private void NextVoiceMode()
	{
		int num = (int)(voiceMode + 1);
		if (num > 2)
		{
			num = 0;
		}
		voiceMode = (CommActivationMode)num;
		SetVoiceMode();
	}

	private void SetInvertYAxis()
	{
		if (invertYAxis)
		{
			UE_YSelected.text = Hub.GetL10NText("UI_PREFAB_GAME_SETTINGS_Y_AXIS_ON");
			Hub.s.gameSettingManager.invertYAxis = -1;
		}
		else
		{
			UE_YSelected.text = Hub.GetL10NText("UI_PREFAB_GAME_SETTINGS_Y_AXIS_OFF");
			Hub.s.gameSettingManager.invertYAxis = 1;
		}
	}

	private void InvertYAxis()
	{
		invertYAxis = !invertYAxis;
		SetInvertYAxis();
	}

	private void SetVsync()
	{
		if (vsync)
		{
			UE_VsyncSelected.text = Hub.GetL10NText("UI_PREFAB_GAME_SETTINGS_Y_AXIS_ON");
			QualitySettings.vSyncCount = (vsync ? 1 : 0);
			Hub.s.gameSettingManager.vsync = vsync;
			ActiveSetFramrateDropdown(active: false);
		}
		else
		{
			UE_VsyncSelected.text = Hub.GetL10NText("UI_PREFAB_GAME_SETTINGS_Y_AXIS_OFF");
			QualitySettings.vSyncCount = (vsync ? 1 : 0);
			Hub.s.gameSettingManager.vsync = vsync;
			Application.targetFrameRate = Hub.s.gameSettingManager.targetFrameRate;
			ActiveSetFramrateDropdown(active: true);
		}
	}

	private void ChangeVsync()
	{
		vsync = !vsync;
		SetVsync();
	}

	private void SetMaxFramerate()
	{
		TMP_Dropdown component = UE_Framerate_select.GetComponent<TMP_Dropdown>();
		List<string> fpsList = new List<string> { "30 FPS", "60 FPS", "120 FPS", "144 FPS", "240 FPS" };
		component.onValueChanged.AddListener(delegate(int num)
		{
			if (!vsync)
			{
				if (fpsList[num] == Hub.GetL10NText("UI_PREFAB_GAME_SETTINGS_FRAME_UNCAPPED"))
				{
					Application.targetFrameRate = -1;
				}
				else
				{
					int num2 = -1;
					switch (num)
					{
					case 0:
						num2 = 30;
						QualitySettings.vSyncCount = 0;
						Application.targetFrameRate = 30;
						break;
					case 1:
						QualitySettings.vSyncCount = 0;
						num2 = 60;
						Application.targetFrameRate = 60;
						break;
					case 2:
						QualitySettings.vSyncCount = 0;
						num2 = 120;
						Application.targetFrameRate = 120;
						break;
					case 3:
						QualitySettings.vSyncCount = 0;
						num2 = 144;
						Application.targetFrameRate = 144;
						break;
					case 4:
						QualitySettings.vSyncCount = 0;
						num2 = 240;
						Application.targetFrameRate = 240;
						break;
					case 5:
						QualitySettings.vSyncCount = 0;
						num2 = -1;
						Application.targetFrameRate = -1;
						break;
					default:
						QualitySettings.vSyncCount = 0;
						num2 = 60;
						Application.targetFrameRate = 60;
						break;
					}
					Hub.s.gameSettingManager.targetFrameRate = num2;
				}
			}
		});
		component.ClearOptions();
		component.AddOptions(fpsList);
		int value = GetClosestFPSIndex(target: Hub.s.gameSettingManager.targetFrameRate, fpsList: fpsList);
		component.value = value;
	}

	public static int GetClosestFPSIndex(List<string> fpsList, int target)
	{
		int num = -1;
		int num2 = int.MaxValue;
		for (int i = 0; i < fpsList.Count; i++)
		{
			string text = fpsList[i];
			if (!text.Contains("FPS"))
			{
				continue;
			}
			string[] array = text.Split(' ');
			if (array.Length != 0 && int.TryParse(array[0], out var result))
			{
				int num3 = Math.Abs(result - target);
				if (num3 < num2)
				{
					num2 = num3;
					num = i;
				}
			}
		}
		if (num == -1)
		{
			num = fpsList.IndexOf("Uncapped");
		}
		return num;
	}

	private void SetDisplayMode()
	{
		TMP_Dropdown component = UE_Dropdown_selectDisplayMode.GetComponent<TMP_Dropdown>();
		List<string> DisplayModeList = new List<string>
		{
			Hub.GetL10NText("UI_PREFAB_GAME_SETTINGS_DISPLAY_FULL_SCREEN"),
			Hub.GetL10NText("UI_PREFAB_GAME_SETTINGS_DISPLAY_WINDOWED_FULL_SCREEN"),
			Hub.GetL10NText("UI_PREFAB_GAME_SETTINGS_DISPLAY_WINDOW")
		};
		component.onValueChanged.AddListener(delegate(int value)
		{
			if (value == 3)
			{
				value = 2;
			}
			int width = Screen.width;
			int height = Screen.height;
			if (DisplayModeList[value] == Hub.GetL10NText("UI_PREFAB_GAME_SETTINGS_DISPLAY_FULL_SCREEN"))
			{
				if (width > 1920)
				{
					Screen.SetResolution(1920, 1080, FullScreenMode.ExclusiveFullScreen);
					TMP_Dropdown component2 = UE_Resolution_select.GetComponent<TMP_Dropdown>();
					List<string> list = component2.options.Select((TMP_Dropdown.OptionData option) => option.text).ToList();
					string targetResolution = "1920 x 1080";
					int num = list.FindIndex((string s) => s == targetResolution);
					if (num >= 0)
					{
						component2.value = num;
						component2.RefreshShownValue();
						prevValue = num;
					}
				}
				else
				{
					Screen.SetResolution(width, height, FullScreenMode.ExclusiveFullScreen);
				}
				currentDisplayMode = FullScreenMode.ExclusiveFullScreen;
				Hub.s.gameSettingManager.displayMode = currentDisplayMode;
				ActiveSetResolutionDropdown(active: true);
			}
			else if (DisplayModeList[value] == Hub.GetL10NText("UI_PREFAB_GAME_SETTINGS_DISPLAY_WINDOWED_FULL_SCREEN"))
			{
				UE_Resolution_select.GetComponent<TMP_Dropdown>().value = UE_Resolution_select.GetComponent<TMP_Dropdown>().options.Count - 1;
				UE_Resolution_select.GetComponent<TMP_Dropdown>().onValueChanged.Invoke(UE_Resolution_select.GetComponent<TMP_Dropdown>().options.Count - 1);
				Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.FullScreenWindow);
				currentDisplayMode = FullScreenMode.FullScreenWindow;
				Hub.s.gameSettingManager.displayMode = currentDisplayMode;
				ActiveSetResolutionDropdown(active: true);
			}
			else if (DisplayModeList[value] == Hub.GetL10NText("UI_PREFAB_GAME_SETTINGS_DISPLAY_MAXIMIZED_WINDOW"))
			{
				Screen.SetResolution(width, height, FullScreenMode.MaximizedWindow);
				currentDisplayMode = FullScreenMode.MaximizedWindow;
				ActiveSetResolutionDropdown(active: false);
			}
			else if (DisplayModeList[value] == Hub.GetL10NText("UI_PREFAB_GAME_SETTINGS_DISPLAY_WINDOW"))
			{
				Screen.SetResolution(width, height, FullScreenMode.Windowed);
				currentDisplayMode = FullScreenMode.Windowed;
				ActiveSetResolutionDropdown(active: false);
				Hub.s.gameSettingManager.prevScreenHeight = height;
				Hub.s.gameSettingManager.prevScreenWidth = width;
			}
			Application.targetFrameRate = Hub.s.gameSettingManager.targetFrameRate;
			OnResoultionChanged();
		});
		component.ClearOptions();
		component.AddOptions(DisplayModeList);
		switch (currentDisplayMode)
		{
		case FullScreenMode.ExclusiveFullScreen:
			component.value = 0;
			break;
		case FullScreenMode.FullScreenWindow:
			component.value = 1;
			break;
		case FullScreenMode.Windowed:
			component.value = 2;
			break;
		default:
			component.value = 0;
			break;
		}
	}

	private void SetLanguage()
	{
		TMP_Dropdown component = UE_Dropdown_selectLanguage.GetComponent<TMP_Dropdown>();
		List<string> languageList = new List<string>
		{
			"English", "한국어", "Español", "日本語", "Français", "Deutsch", "简体中文", "繁體中文", "Português (Brasil)", "Italiano",
			"русский язык", "Українська", "Tiếng Việt", "ไทย", "Türkçe", "Polski"
		};
		component.onValueChanged.AddListener(delegate(int num)
		{
			UE_Dropdown_selectLanguage_Label.text = languageList[num];
			L10NManager lcman = Hub.s.lcman;
			L10NManager.Language language = (L10NManager.Language)num;
			lcman.ChangeLanguage(language.ToString());
		});
		component.ClearOptions();
		component.AddOptions(languageList);
		L10NManager.Language value = (L10NManager.Language)Enum.Parse(typeof(L10NManager.Language), Hub.s.lcman.language);
		component.value = (int)value;
	}

	private void OnValueChangedMasterVolume(float value)
	{
		Hub.s.gameSettingManager.masterVolume = value;
	}

	private void OnValueChangedMicVolume(float value)
	{
		Hub.s.gameSettingManager.micVolume = value;
	}

	public void OnValueChangedMouseSensitivity(float value)
	{
		Hub.s.gameSettingManager.mouseSensitivity = value;
	}

	public void OnValueChangedGamma(float value)
	{
		Hub.s.gameSettingManager.SetGamma(value);
	}

	public void OnValueChangedBrightness(float value)
	{
		Hub.s.gameSettingManager.SetGain(value);
	}

	public void ResetSetting()
	{
		Hub.s.gameSettingManager.ResetDefaultSetting();
		Set();
	}

	public void UpdateGameSettings_ScreenModeDropDown(bool invoke = false)
	{
		UE_Dropdown_selectDisplayMode.GetComponent<TMP_Dropdown>().value = (int)Screen.fullScreenMode;
		if (invoke)
		{
			UE_Dropdown_selectDisplayMode.GetComponent<TMP_Dropdown>().onValueChanged.Invoke((int)Screen.fullScreenMode);
		}
	}

	private void ActiveSetResolutionDropdown(bool active)
	{
		if (active)
		{
			UE_RESOLUTION_mouseover.raycastTarget = true;
			UE_Resolution_select.GetComponent<TMP_Dropdown>().interactable = true;
			UE_Resolution_select.raycastTarget = true;
			UE_Resolution_select.GetComponent<Image>().raycastTarget = true;
			UE_Resolution_selectArrow.raycastTarget = true;
			UE_Resolution_select_Label.raycastTarget = true;
			UE_Resolution_selectArrow.color = Color.white;
			UE_Resolution_select_Label.color = Color.white;
			UE_RESOLUTION.color = Color.white;
			TMP_Dropdown component = UE_Resolution_select.GetComponent<TMP_Dropdown>();
			string text = $"{Screen.width} x {Screen.height}";
			int num = -1;
			for (int i = 0; i < component.options.Count; i++)
			{
				if (component.options[i].text == text)
				{
					num = i;
					break;
				}
			}
			if (num >= 0)
			{
				component.value = num;
				component.RefreshShownValue();
				prevValue = num;
			}
		}
		else
		{
			UE_RESOLUTION_mouseover.raycastTarget = false;
			UE_Resolution_select.GetComponent<TMP_Dropdown>().interactable = false;
			UE_Resolution_select.raycastTarget = false;
			UE_Resolution_select.GetComponent<Image>().raycastTarget = false;
			UE_Resolution_select_Label.raycastTarget = false;
			UE_Resolution_selectArrow.raycastTarget = false;
			UE_Resolution_selectArrow.color = Color.gray;
			UE_Resolution_select_Label.color = Color.gray;
			UE_RESOLUTION.color = Color.gray;
		}
	}

	private void ActiveSetFramrateDropdown(bool active)
	{
		if (active)
		{
			UE_FRAMERATE_mouseover.raycastTarget = true;
			UE_Framerate_select.GetComponent<TMP_Dropdown>().interactable = true;
			UE_Framerate_select.raycastTarget = true;
			UE_Framerate_select.GetComponent<Image>().raycastTarget = true;
			UE_Framerate_selectArrow.raycastTarget = true;
			UE_Framerate_select_Label.raycastTarget = true;
			UE_Framerate_selectArrow.color = Color.white;
			UE_Framerate_select_Label.color = Color.white;
			UE_FRAMERATE.color = Color.white;
		}
		else
		{
			UE_FRAMERATE_mouseover.raycastTarget = false;
			UE_Framerate_select.GetComponent<TMP_Dropdown>().interactable = false;
			UE_Framerate_select.raycastTarget = false;
			UE_Framerate_select.GetComponent<Image>().raycastTarget = false;
			UE_Framerate_selectArrow.raycastTarget = false;
			UE_Framerate_select_Label.raycastTarget = false;
			UE_Framerate_selectArrow.color = Color.gray;
			UE_Framerate_select_Label.color = Color.gray;
			UE_FRAMERATE.color = Color.gray;
		}
	}

	private void InitializeResolutionDropdown()
	{
		TMP_Dropdown component = UE_Resolution_select.GetComponent<TMP_Dropdown>();
		List<Resolution> uniqueResolutions = GetUniqueResolutions();
		List<string> options = CreateOptionStrings(uniqueResolutions);
		SetupResolutionDropdownOptions(component, options);
		RegisterDropdownCallback(component, options);
		SelectCurrentResolution(component, options);
	}

	public static List<Resolution> GetUniqueResolutions()
	{
		return (from r in Screen.resolutions
			group r by new { r.width, r.height }).Select(g =>
		{
			Resolution resolution = g.First();
			return new Resolution
			{
				width = resolution.width,
				height = resolution.height,
				refreshRate = 0
			};
		}).ToList();
	}

	private List<string> CreateOptionStrings(List<Resolution> resolutions)
	{
		List<string> list = new List<string>();
		foreach (Resolution resolution in resolutions)
		{
			list.Add($"{resolution.width} x {resolution.height}");
		}
		return list;
	}

	private void SetupResolutionDropdownOptions(TMP_Dropdown dropdown, List<string> options)
	{
		dropdown.ClearOptions();
		dropdown.AddOptions(options);
	}

	private void RegisterDropdownCallback(TMP_Dropdown dropdown, List<string> options)
	{
		dropdown.onValueChanged.RemoveAllListeners();
		dropdown.onValueChanged.AddListener(delegate(int index)
		{
			if (!initResoutionDropdown)
			{
				initResoutionDropdown = true;
			}
			else
			{
				string[] array = options[index].Split('x');
				int width = int.Parse(array[0].Trim());
				int height = int.Parse(array[1].Trim());
				Screen.SetResolution(width, height, currentDisplayMode);
				OnResoultionChanged();
			}
		});
	}

	private void SelectCurrentResolution(TMP_Dropdown dropdown, List<string> options)
	{
		string currentResString = $"{Screen.width} x {Screen.height}";
		int num = options.FindIndex((string s) => s == currentResString);
		if (num >= 0)
		{
			dropdown.value = num;
		}
		prevValue = dropdown.value;
	}

	public void ConfirmChangeResolution()
	{
		prevValue = UE_Resolution_select.GetComponent<TMP_Dropdown>().value;
	}

	public void CancelChangeResolution()
	{
		if (prevValue >= 0)
		{
			UE_Resolution_select.GetComponent<TMP_Dropdown>().value = prevValue;
			UE_Resolution_select.GetComponent<TMP_Dropdown>().RefreshShownValue();
		}
	}

	private void Update()
	{
		if (!(Hub.s == null) && !(Hub.s.inputman == null))
		{
			if (voiceMode == CommActivationMode.VoiceActivation || voiceMode == CommActivationMode.Open)
			{
				UE_volumeGauge.fillAmount = VolumeGauge() * 3f;
			}
			else if (voiceMode == CommActivationMode.PushToTalk && Hub.s.inputman.isPressed(InputAction.PushToTalk))
			{
				UE_volumeGauge.fillAmount = VolumeGauge() * 3f;
			}
			else if (voiceMode == CommActivationMode.None)
			{
				UE_volumeGauge.fillAmount = 0f;
			}
		}
	}

	public float VolumeGauge()
	{
		_clip = _microphoneCapture?.GetClip;
		if (SceneManager.GetActiveScene().name.Equals("MainMenuScene"))
		{
			_clip = micClip;
		}
		if (_clip == null)
		{
			return 0f;
		}
		int num = Microphone.GetPosition(micName) - sampleWindow;
		if (num < 0)
		{
			return 0f;
		}
		if (num + sampleWindow > _clip.samples)
		{
			return 0f;
		}
		float[] array = new float[sampleWindow];
		_clip.GetData(array, num);
		float num2 = 0f;
		for (int i = 0; i < array.Length; i++)
		{
			num2 += Mathf.Abs(array[i]);
		}
		return num2 / (float)sampleWindow;
	}

	private void OnResoultionChanged()
	{
		Hub.s.rpmman.ResetRenderScaleBelowFHD();
	}
}
