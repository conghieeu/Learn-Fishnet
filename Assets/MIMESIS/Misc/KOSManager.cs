using System.Reflection;
using Gpp;
using Gpp.Auth;
using Gpp.Constants;
using Gpp.Core;
using Gpp.Models;
using UnityEngine;

public class KOSManager : MonoBehaviour
{
	public string configStage = string.Empty;

	public string lastCreatorCode = string.Empty;

	private void Awake()
	{
		Logger.RLog("[AwakeLogs] KOSManager.Awake ->");
		Initialize();
		Logger.RLog("[AwakeLogs] KOSManager.Awake <-");
	}

	public void Initialize()
	{
		GppSDK.Initialize(delegate(Result result)
		{
			if (!result.IsError)
			{
				OnInitializeSuccess();
			}
			else
			{
				OnInitializeFailure(result.Error);
			}
		});
	}

	public void OnInitializeSuccess()
	{
		CheckActiveStage();
		Login();
	}

	public void OnInitializeFailure(Error error)
	{
		Logger.RLog($"KOS Init Failed Error Code : {error.Code}");
		Logger.RLog("KOS Init Failed Error Message : " + error.Message);
	}

	public void Login()
	{
		GppSDK.Login(delegate(Result<GppUser> result)
		{
			if (!result.IsError)
			{
				OnLoginSuccess(result.Value);
				GetLastCreatorCode();
			}
			else
			{
				OnLoginFailure(result.Error);
			}
		}, PlatformType.Steam, isHeadless: true);
	}

	private void OnLoginSuccess(GppUser userInfo)
	{
	}

	private void OnLoginFailure(Error error)
	{
		Logger.RLog($"KOS Login Failed Error Code : {error.Code}");
		Logger.RLog("KOS Login Failed Error Message : " + error.Message);
	}

	public void GetLastCreatorCode()
	{
		GppSDK.GetLastCreatorCode(delegate(Result<CreatorCodeResult> result)
		{
			if (!result.IsError)
			{
				lastCreatorCode = result.Value.Code;
			}
			else
			{
				Logger.RLog($"Get Last Creator Code Failed : {result.Error.Code}");
				Logger.RLog("Get Last Creator Code Failed : " + result.Error.Message);
			}
		});
	}

	public void SetCreatorCode(string code)
	{
		Logger.RLog("Set Creator Code : " + code);
		GppSDK.SetCreatorCode(code, delegate(Result<CreatorCodeResult> result)
		{
			if (!result.IsError)
			{
				lastCreatorCode = result.Value.Code;
				if (Hub.s.uiman.creatorCodeUI != null)
				{
					Hub.s.uiman.creatorCodeUI.SuccessInputCreatorCode();
				}
			}
			else
			{
				Logger.RLog($"Set Creator Code Failed : {result.Error.Code}");
				Logger.RLog("Set Creator Code Failed : " + result.Error.Message);
				if (Hub.s.uiman.creatorCodeUI != null)
				{
					Hub.s.uiman.creatorCodeUI.FailInputCreatorCode(result.Error.Code.ToString());
				}
			}
		});
	}

	public void CheckActiveStage()
	{
		Object obj = Resources.Load("GppSDK/GppConfig");
		if (obj == null)
		{
			Logger.RError("[GPP] Config not found at Resources/GppSDK/GppConfig");
			return;
		}
		FieldInfo field = obj.GetType().GetField("activeStage");
		Logger.RLog($"GPP Config ActiveStage: {field.GetValue(obj)}");
		configStage = field.GetValue(obj).ToString();
	}
}
