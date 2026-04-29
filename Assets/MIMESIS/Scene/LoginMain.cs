using System.Collections;
using ReluProtocol;
using ReluProtocol.Enum;
using Steamworks;
using UnityEngine;
using UnityEngine.CrashReportHandler;

public class LoginMain : MonoBehaviour
{
	[SerializeField]
	private GameObject ui_networkErrorPrefab;

	[SerializeField]
	private GameObject ui_loginPrefab;

	[SerializeField]
	private GameObject ui_BrightnessCalibrationPrefab;

	private UIPrefab_login ui_login;

	[SerializeField]
	private GameObject ui_timeWarningPrefab;

	[SerializeField]
	private GameObject ui_OpeningAgeWarning;

	private UIPrefab_NetworkError networkErrorUI;

	private bool ageCheck;

	private APIRequestHandler apiHandler => Hub.s.apihandler;

	private UIManager uiman => Hub.s.uiman;

	private Hub.PersistentData pdata => Hub.s.pdata;

	private void RLog(string msg, bool sendToServer = false)
	{
	}

	private void Start()
	{
		if (!(Hub.s == null))
		{
			Hub.s.uiman.tos = PlayerPrefs.GetInt("tos", 0) == 1;
			Hub.s.uiman.pp = PlayerPrefs.GetInt("pp", 0) == 1;
			StartCoroutine(Run());
		}
	}

	private IEnumerator KRWarning()
	{
		yield return new WaitUntil(() => SteamManager.Initialized);
		if (!(SteamUtils.GetIPCountry() != "KR"))
		{
			UIPrefab_OpeningAgeWarning uIPrefab_OpeningAgeWarning = uiman.InstatiateUIPrefab<UIPrefab_OpeningAgeWarning>(ui_OpeningAgeWarning, eUIHeight.OverTheTop);
			uIPrefab_OpeningAgeWarning.Show();
			yield return uIPrefab_OpeningAgeWarning.Process();
		}
	}

	private IEnumerator Run()
	{
		yield return Hub.oneSecond;
		Hub.s.netman2.Initialize();
		yield return Run_SignInSteam();
		yield return KRWarning();
		yield return Run_Login();
		Hub.s.apihandler.EnqueueAPI<APIEnterLobbyLogRes>(new APIEnterLobbyLogReq
		{
			guid = Hub.s.pdata.GUID,
			sessionID = Hub.s.pdata.ClientSessionID
		}, delegate(IResMsg res)
		{
			if (res.errorCode != MsgErrorCode.Success)
			{
				Logger.RError($"APIEnterLobbyLogReq failed : {res.errorCode}");
			}
		});
		uiman.InstatiateUIPrefab<UIPrefab_GamepadCursor>(uiman.prefab_GamepadCursor, eUIHeight.OverTheTop).Show();
		Hub.LoadScene("MainMenuScene");
	}

	private IEnumerator ShowTimeWarning()
	{
		uiman.InstatiateUIPrefab<UIPrefab_TimeWarning>(ui_timeWarningPrefab, eUIHeight.Top).Show();
		yield break;
	}

	private IEnumerator Run_ShowHello()
	{
		if (Hub.s == null)
		{
			Logger.RError("Hub.s is null");
		}
		if (pdata == null)
		{
			Logger.RError("pdata is null");
		}
		if (pdata.commandLineArgs == null)
		{
			Logger.RError("pdata.commandLineArgs is null");
		}
		if (pdata.commandLineArgs.Length == 1)
		{
			yield return null;
		}
	}

	private IEnumerator CheckAgeProcess()
	{
		Hub.s.uiman.OpenCheckAge();
		yield return new WaitUntil(() => Hub.s.uiman.ageCheck != 0);
	}

	private IEnumerator Run_Login()
	{
		pdata.MyNickName = SteamFriends.GetPersonaName();
		yield return null;
		Logger.RLog("My Nickname is set to : " + pdata.MyNickName);
		CrashReportHandler.SetUserMetadata("my_nickname", pdata.MyNickName);
	}

	private IEnumerator Run_BrightnessCalibration()
	{
		if (PlayerPrefs.GetInt("BrightnessCalibrationDone") != 1)
		{
			UIPrefab_BrightnessCalibration brightnessCalibration = uiman.InstatiateUIPrefab<UIPrefab_BrightnessCalibration>(ui_BrightnessCalibrationPrefab);
			brightnessCalibration.Show();
			bool BrightnessCalibrationDone = false;
			brightnessCalibration.OnButtonOK = delegate
			{
				PlayerPrefs.SetInt("BrightnessCalibrationDone", 1);
				BrightnessCalibrationDone = true;
			};
			yield return new WaitUntil(() => BrightnessCalibrationDone);
			yield return brightnessCalibration.Cor_Hide();
		}
	}

	private IEnumerator Run_SignInSteam()
	{
		Hub.s.SteamConnector.SignIn();
		yield return new WaitUntil(() => Hub.s.SteamConnector.SignedIn);
		if (Hub.s.SteamConnector.LastError != MsgErrorCode.Success)
		{
			Logger.RError($"Steam SignIn Error, errorcode : {Hub.s.SteamConnector.LastError}");
		}
	}

	private void RegisterGlobalCheatCommand()
	{
	}

	private IEnumerator Run_TOS()
	{
		if (!Hub.s.uiman.tos)
		{
			Hub.s.uiman.ShowTermsOfService();
		}
		yield return new WaitUntil(() => Hub.s.uiman.tos);
	}

	private IEnumerator Run_PP()
	{
		if (!Hub.s.uiman.pp)
		{
			Hub.s.uiman.ShowPrivacyPolicy();
		}
		yield return new WaitUntil(() => Hub.s.uiman.pp);
	}
}
