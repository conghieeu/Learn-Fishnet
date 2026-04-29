using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ReluProtocol;
using Steamworks;
using UnityEngine;

public class PlatformMgr : MonoSingleton<PlatformMgr>
{
	public const string SaveDirectoryName = "Save";

	private string _persistentDataPath;

	private string _uniqueUserPath = "";

	private bool _inited;

	private bool _IsSaving;

	private Dictionary<string, SaveDataBuffer> _saveDataBufferDic = new Dictionary<string, SaveDataBuffer>();

	private SaveAsyncResult _saveAsyncResult;

	private LoadAsyncResult _loadAsyncResult;

	private void Start()
	{
		Init();
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		Dispose();
	}

	public override void Init()
	{
		base.Init();
		if (!_inited)
		{
			_inited = true;
			_persistentDataPath = Application.persistentDataPath;
		}
	}

	public IEnumerator CoInit()
	{
		if (!_inited)
		{
			_inited = true;
			_persistentDataPath = Application.persistentDataPath;
		}
		yield break;
	}

	public void Dispose()
	{
		if (_inited)
		{
			_inited = false;
		}
	}

	private string GetSaveFileName(int slotID)
	{
		return MMSaveGameData.GetSaveFileName(slotID);
	}

	public T Load<T>(string fileName)
	{
		string saveFileFullPath = GetSaveFileFullPath(fileName);
		try
		{
			byte[] array = LoadFile(saveFileFullPath);
			if (array == null)
			{
				return default(T);
			}
			return SerializerUtil.Deserialize<T>(array);
		}
		catch (Exception ex)
		{
			Logger.RError("#Load : @@@ PlatformMgr.Load : " + saveFileFullPath + " : " + SerializerUtil.GetSerializerType() + "\nMaybe save file was corrupted!\n" + ex.ToString());
			return default(T);
		}
	}

	public bool CanLoadSaveGameData<T>(string inFilePath)
	{
		try
		{
			if (File.Exists(inFilePath))
			{
				return false;
			}
			if (Load<T>(inFilePath) == null)
			{
				Logger.RWarn("[CanLoadSaveGameData] SaveGameData Load test fail! inFilePath=" + inFilePath);
				return false;
			}
		}
		catch (Exception ex)
		{
			Logger.RError("#Load : @@@ PlatformMgr.Load : " + inFilePath + " : " + SerializerUtil.GetSerializerType() + "\nMaybe save file was corrupted!\n" + ex.ToString());
			return false;
		}
		return true;
	}

	private byte[] LoadFile(string filePath)
	{
		if (File.Exists(filePath))
		{
			using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
			{
				byte[] array = new byte[fileStream.Length];
				fileStream.Read(array, 0, array.Length);
				fileStream.Flush();
				fileStream.Close();
				return array;
			}
		}
		return null;
	}

	public void Save<T>(string fileName, T obj)
	{
		byte[] saveBytes = SerializerUtil.Serialize(fileName, obj);
		SaveFile(fileName, saveBytes);
	}

	public SaveAsyncResult SaveFile(string fileName, byte[] saveBytes)
	{
		string saveFileFullPath = GetSaveFileFullPath(fileName);
		return SaveDataBufferWriteToDisc(saveFileFullPath, saveBytes);
	}

	public void CheckDocumentsDirectory()
	{
		if (!Directory.Exists(GetSaveFileFolderPath()))
		{
			Directory.CreateDirectory(GetSaveFileFolderPath());
		}
	}

	public bool IsSaveFileExist(string fileName)
	{
		if (!new FileInfo(GetSaveFileFullPath(fileName)).Exists)
		{
			return false;
		}
		return true;
	}

	public void Delete(string fileName)
	{
		if (IsSaveFileExist(fileName))
		{
			DeleteFile(fileName);
		}
	}

	private void DeleteFile(string fileName)
	{
		string saveFileFullPath = GetSaveFileFullPath(fileName);
		CheckDocumentsDirectory();
		FileInfo fileInfo = new FileInfo(saveFileFullPath);
		if (fileInfo.Exists)
		{
			fileInfo.Delete();
		}
	}

	public string GetSaveFileFullPath(string filename)
	{
		return $"{GetSaveFileFolderPath()}/{filename}";
	}

	public string GetSaveFileFolderPath()
	{
		if (string.IsNullOrEmpty(_uniqueUserPath))
		{
			if (SteamManager.Initialized)
			{
				_uniqueUserPath = SteamUser.GetSteamID().ToString();
			}
			if (string.IsNullOrEmpty(_uniqueUserPath))
			{
				Logger.RError("[PlatformMgr] GetSteamID() fail.");
				Application.Quit();
			}
		}
		return _persistentDataPath + "/Save/" + _uniqueUserPath;
	}

	public void TrySaveDataBuffersToDisc_All_Force()
	{
		DateTime now = DateTime.Now;
		try
		{
			Dictionary<string, SaveDataBuffer>.Enumerator enumerator = _saveDataBufferDic.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SaveDataBuffer value = enumerator.Current.Value;
				if (value.IsCanSave(now, isIgnoreTime: true))
				{
					string filePath = value.SaveFilePath.ToString();
					byte[] saveBytes = value.SaveMemoryStream.ToArray();
					_ = value.SaveAsyncSuccessAction;
					_ = value.SaveAsyncFailAction;
					value.ReadyNextSave(now);
					SaveDataBufferWriteToDisc(filePath, saveBytes);
					now = DateTime.Now;
				}
			}
		}
		catch (Exception ex)
		{
			Logger.RError("[***][Save] TrySaveDataBuffersToDisc_All_Force. Error : " + ex.ToString());
		}
		finally
		{
		}
	}

	private SaveAsyncResult SaveDataBufferWriteToDisc(string filePath, byte[] saveBytes)
	{
		CheckDocumentsDirectory();
		string text = filePath + ".tmp";
		File.WriteAllBytes(text, saveBytes);
		if (File.Exists(filePath))
		{
			File.Replace(text, filePath, filePath + ".bak");
		}
		else
		{
			File.Move(text, filePath);
		}
		return SaveAsyncResult.Success;
	}

	public void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			TrySaveDataBuffersToDisc_All_Force();
		}
	}

	private void OnApplicationQuit()
	{
		TrySaveDataBuffersToDisc_All_Force();
	}
}
