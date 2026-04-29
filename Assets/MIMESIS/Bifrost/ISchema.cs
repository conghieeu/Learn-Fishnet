using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace Bifrost
{
	public abstract class ISchema
	{
		[JsonIgnore]
		public readonly uint MsgID;

		[JsonIgnore]
		public readonly string MsgName;

		[JsonIgnore]
		public byte[] pktBinary;

		public ISchema(uint msgID, string msgName)
		{
			MsgID = msgID;
			MsgName = msgName;
		}

		public abstract int GetLength();

		public abstract void Save(BinaryWriter bw);

		public abstract bool Load(BinaryReader bw);

		public virtual bool LoadFromJson<T>(string jsonData) where T : ISchema, new()
		{
			Type type = GetType();
			FieldInfo field = type.GetField("dataHolder");
			if (field != null)
			{
				Type fieldType = field.FieldType;
				try
				{
					object obj = JsonConvert.DeserializeObject(jsonData, fieldType);
					if (obj == null)
					{
						return false;
					}
					field.SetValue(this, obj);
					FieldInfo field2 = type.GetField("versionInfo");
					if (field2 != null)
					{
						field2.SetValue(this, 1);
					}
					return true;
				}
				catch (Exception ex)
				{
					Logger.RError("JSON LoadFromJson failed: " + ex.Message);
					return false;
				}
			}
			try
			{
				T val = JsonConvert.DeserializeObject<T>(jsonData);
				if (val == null)
				{
					return false;
				}
				FieldInfo[] fields = typeof(T).GetFields();
				foreach (FieldInfo fieldInfo in fields)
				{
					fieldInfo.SetValue(this, fieldInfo.GetValue(val));
				}
				return true;
			}
			catch (Exception ex2)
			{
				Logger.RError("JSON LoadFromJson failed: " + ex2.Message);
				return false;
			}
		}

		public abstract void Clean();

		public void CopyTo(MemoryStream stream)
		{
			BinaryWriter bw = new BinaryWriter(stream);
			Save(bw);
		}

		public void MakeBinary()
		{
			MemoryStream memoryStream = new MemoryStream();
			CopyTo(memoryStream);
			pktBinary = memoryStream.GetBuffer();
		}
	}
}
