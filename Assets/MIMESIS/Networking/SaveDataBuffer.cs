using System;
using System.IO;

public class SaveDataBuffer
{
	public const double SaveTermSeconds = 10.0;

	public readonly string SaveFilePath = "";

	public DateTime NextSaveTime = DateTime.Now;

	public bool IsNeedSave;

	public MemoryStream SaveMemoryStream = new MemoryStream();

	public Action SaveAsyncSuccessAction;

	public Action<SaveAsyncResult> SaveAsyncFailAction;

	public SaveDataBuffer(string filePath, byte[] saveData)
	{
		SaveFilePath = filePath;
		NextSaveTime = DateTime.Now;
		SaveAsyncSuccessAction = null;
		SaveAsyncFailAction = null;
		SetSaveData(saveData);
	}

	public bool IsCanSave(DateTime dtNow, bool isIgnoreTime)
	{
		if (!IsNeedSave)
		{
			return false;
		}
		if (!isIgnoreTime && dtNow < NextSaveTime)
		{
			return false;
		}
		return true;
	}

	public void SetSaveData(byte[] saveData)
	{
		IsNeedSave = true;
		SaveMemoryStream.SetLength(0L);
		SaveMemoryStream.Write(saveData, 0, saveData.Length);
	}

	public void ReadyNextSave(DateTime dtNow)
	{
		IsNeedSave = false;
		NextSaveTime = dtNow.AddSeconds(10.0);
		SaveMemoryStream.SetLength(0L);
		SaveAsyncSuccessAction = null;
		SaveAsyncFailAction = null;
	}
}
