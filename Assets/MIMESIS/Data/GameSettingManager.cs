using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dissonance;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GameSettingManager : MonoBehaviour
{
	private CommActivationMode _voivoiceMode = CommActivationMode.VoiceActivation;

	public Volume volume;

	public KeyImageData keyImageData;

	private string resetKeyCode = "resetKeyCode";

	private float _masterVolume = 0.5f;

	private float _micVolume = 0.5f;

	private float _mouseSensitivity = 0.5f;

	private int _invertYAxis = 1;

	private int _targetFrameRate = 60;

	private float _gamma = 0.5f;

	private float _brightness = 0.5f;

	private bool _vsync;

	private FullScreenMode _lastDisplayMode;

	private FullScreenMode _displayMode;

	private List<Volume> postProcessVolumes = new List<Volume>();

	private List<LiftGammaGain> colorGradings = new List<LiftGammaGain>();

	private Coroutine _resolutionCoroutine;

	[HideInInspector]
	public int prevScreenWidth;

	[HideInInspector]
	public int prevScreenHeight;

	private float targetAspect = 1.3333334f;

	public CommActivationMode voiceMode
	{
		get
		{
			return _voivoiceMode;
		}
		set
		{
			_voivoiceMode = value;
			PlayerPrefs.SetInt("voiceMode", (int)_voivoiceMode);
		}
	}

	public float masterVolume
	{
		get
		{
			return _masterVolume;
		}
		set
		{
			_masterVolume = value;
			Hub.s.legacyAudio.SetMasterVolume(_masterVolume);
			Hub.s.audioman.SetMasterVolume(_masterVolume);
			PlayerPrefs.SetFloat("masterVolume", _masterVolume);
		}
	}

	public float micVolume
	{
		get
		{
			return _micVolume;
		}
		set
		{
			_micVolume = value;
			Hub.s.legacyAudio.SetVoiceVolume(_micVolume);
			PlayerPrefs.SetFloat("micVolume", _micVolume);
		}
	}

	public float mouseSensitivity
	{
		get
		{
			return _mouseSensitivity;
		}
		set
		{
			_mouseSensitivity = value;
			if (_mouseSensitivity < 0.1f)
			{
				Hub.s.inputman.mouseSensetivity = 0.1f;
			}
			else
			{
				Hub.s.inputman.mouseSensetivity = _mouseSensitivity;
			}
			PlayerPrefs.SetFloat("mouse_Sensitivity", _mouseSensitivity);
		}
	}

	public int invertYAxis
	{
		get
		{
			return _invertYAxis;
		}
		set
		{
			_invertYAxis = value;
			Hub.s.inputman.invertYAxis = _invertYAxis;
			PlayerPrefs.SetInt("invertYAxis", _invertYAxis);
		}
	}

	public int targetFrameRate
	{
		get
		{
			return _targetFrameRate;
		}
		set
		{
			_targetFrameRate = value;
			PlayerPrefs.SetInt("TargetFrameRate", _targetFrameRate);
			Application.targetFrameRate = _targetFrameRate;
		}
	}

	public float gamma
	{
		get
		{
			return _gamma;
		}
		set
		{
			_gamma = value;
			PlayerPrefs.SetFloat("gamma", _gamma);
		}
	}

	public float brightness
	{
		get
		{
			return _brightness;
		}
		set
		{
			_brightness = value;
			PlayerPrefs.SetFloat("brightness", _brightness);
		}
	}

	public bool vsync
	{
		get
		{
			return _vsync;
		}
		set
		{
			_vsync = value;
			QualitySettings.vSyncCount = (_vsync ? 1 : 0);
			PlayerPrefs.SetInt("vsync", _vsync ? 1 : 0);
		}
	}

	public FullScreenMode lastDisplayMode
	{
		get
		{
			return _lastDisplayMode;
		}
		set
		{
			_lastDisplayMode = value;
			PlayerPrefs.SetInt("lastDisplayMode", (int)_lastDisplayMode);
		}
	}

	public FullScreenMode displayMode
	{
		get
		{
			return _displayMode;
		}
		set
		{
			_displayMode = value;
			if (_displayMode == FullScreenMode.FullScreenWindow || _displayMode == FullScreenMode.ExclusiveFullScreen)
			{
				lastDisplayMode = _displayMode;
				PlayerPrefs.SetInt("displayMode", (int)_displayMode);
			}
		}
	}

	private void Start()
	{
		Logger.RLog("[AwakeLogs] GameSettingManager.Start ->");
		resetKeyCode = "resetKeyCode4";
		if (PlayerPrefs.GetInt(resetKeyCode, 0) == 0)
		{
			ResetDefaultSetting();
			PlayerPrefs.SetInt(resetKeyCode, 1);
		}
		else
		{
			LoadSettings();
		}
		Application.targetFrameRate = targetFrameRate;
		prevScreenWidth = Screen.width;
		prevScreenHeight = Screen.height;
		Logger.RLog("[AwakeLogs] GameSettingManager.Start <-");
	}

	public void SetGamma(float value)
	{
		gamma = value;
		colorGradings.ForEach(delegate(LiftGammaGain cg)
		{
			cg.gamma.value = new Vector4(cg.gamma.value.x, cg.gamma.value.y, cg.gamma.value.z, value);
		});
	}

	public void SetGain(float value)
	{
		brightness = value;
		colorGradings.ForEach(delegate(LiftGammaGain cg)
		{
			cg.gain.value = new Vector4(cg.gain.value.x, cg.gain.value.y, cg.gain.value.z, value);
		});
	}

	public void LoadLiftGammaGainFromCurrentScene()
	{
		postProcessVolumes = GetAllPostProcessVolumes();
		foreach (Volume postProcessVolume in postProcessVolumes)
		{
			VolumeProfile profile = postProcessVolume.profile;
			if (!(profile == null))
			{
				if (!profile.TryGet<LiftGammaGain>(out var component))
				{
					component = profile.Add<LiftGammaGain>();
				}
				colorGradings.Add(component);
				component.active = true;
				component.lift.overrideState = false;
				component.gamma.overrideState = true;
				component.gain.overrideState = true;
				component.gamma.value = new Vector4(component.lift.value.x, component.lift.value.y, component.lift.value.z, gamma);
				component.gain.value = new Vector4(component.gamma.value.x, component.gamma.value.y, component.gamma.value.z, brightness);
			}
		}
	}

	public List<Volume> GetAllPostProcessVolumes()
	{
		new List<Volume>();
		return Object.FindObjectsOfType<Volume>(includeInactive: true).ToList();
	}

	public void ResetDefaultSetting()
	{
		PlayerPrefs.DeleteKey("masterVolume");
		PlayerPrefs.DeleteKey("micVolume");
		PlayerPrefs.DeleteKey("mouse_Sensitivity");
		PlayerPrefs.DeleteKey("invertYAxis");
		PlayerPrefs.DeleteKey("gamma");
		PlayerPrefs.DeleteKey("brightness");
		LoadSettings();
	}

	public void LoadSettings()
	{
		masterVolume = PlayerPrefs.GetFloat("masterVolume", Hub.s.gameConfig.gameSetting.defaultMasterVolume);
		micVolume = PlayerPrefs.GetFloat("micVolume", Hub.s.gameConfig.gameSetting.defaultMicVolume);
		mouseSensitivity = PlayerPrefs.GetFloat("mouse_Sensitivity", Hub.s.gameConfig.gameSetting.defaultMouseSensitivity);
		invertYAxis = PlayerPrefs.GetInt("invertYAxis", 1);
		targetFrameRate = PlayerPrefs.GetInt("TargetFrameRate", Hub.s.gameConfig.gameSetting.defaultFrameRate);
		voiceMode = (CommActivationMode)PlayerPrefs.GetInt("voiceMode", 1);
		if (PlayerPrefs.GetInt("FirstLogin1", 0) == 0)
		{
			PlayerPrefs.SetInt("FirstLogin1", 1);
			PlayerPrefs.Save();
			Resolution currentResolution = Screen.currentResolution;
			Hub.s.gameSettingManager.displayMode = FullScreenMode.FullScreenWindow;
			Screen.SetResolution(currentResolution.width, currentResolution.height, FullScreenMode.FullScreenWindow);
		}
		else
		{
			displayMode = (FullScreenMode)PlayerPrefs.GetInt("displayMode", (int)Hub.s.gameConfig.gameSetting.defaultFullScreenMode);
			lastDisplayMode = (FullScreenMode)PlayerPrefs.GetInt("lastDisplayMode", (int)Hub.s.gameConfig.gameSetting.defaultFullScreenMode);
		}
		gamma = PlayerPrefs.GetFloat("gamma", Hub.s.gameConfig.gameSetting.defaultGamma);
		brightness = PlayerPrefs.GetFloat("brightness", Hub.s.gameConfig.gameSetting.defaultBrightness);
		vsync = PlayerPrefs.GetInt("vsync", 1) == 1;
	}

	private IEnumerator WaitForResolutionAndMode(int targetW, int targetH, FullScreenMode targetMode)
	{
		while (Screen.width != targetW || Screen.height != targetH || Screen.fullScreenMode != targetMode)
		{
			yield return null;
		}
		if (Hub.s.uiman.settingsUI != null)
		{
			Hub.s.uiman.settingsUI.UpdateGameSettings_ScreenModeDropDown();
		}
		_resolutionCoroutine = null;
	}
}
