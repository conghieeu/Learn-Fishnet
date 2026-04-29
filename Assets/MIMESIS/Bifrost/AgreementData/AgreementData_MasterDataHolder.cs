using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.AgreementData
{
	public class AgreementData_MasterDataHolder : ISchema
	{
		public int versionInfo;

		public List<AgreementData_MasterData> dataHolder = new List<AgreementData_MasterData>();

		public AgreementData_MasterDataHolder(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public AgreementData_MasterDataHolder()
			: base(467697005u, "AgreementData_MasterDataHolder")
		{
		}

		public void CopyTo(AgreementData_MasterDataHolder dest)
		{
			dest.versionInfo = versionInfo;
			dest.dataHolder.Clear();
			foreach (AgreementData_MasterData item in dataHolder)
			{
				AgreementData_MasterData agreementData_MasterData = new AgreementData_MasterData();
				item.CopyTo(agreementData_MasterData);
				dest.dataHolder.Add(agreementData_MasterData);
			}
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			int num = 0;
			num += Serializer.GetLength(versionInfo);
			num += 4;
			foreach (AgreementData_MasterData item in dataHolder)
			{
				num += item.GetLengthInternal();
			}
			return num;
		}

		public override bool Load(BinaryReader br)
		{
			uint uintValue = 0u;
			Serializer.Load(br, ref uintValue);
			if (MsgID != (uint)IPAddress.NetworkToHostOrder((int)uintValue))
			{
				return false;
			}
			try
			{
				LoadInternal(br);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public void LoadInternal(BinaryReader br)
		{
			Serializer.Load(br, ref versionInfo);
			int intValue = 0;
			Serializer.Load(br, ref intValue);
			while (intValue-- > 0)
			{
				AgreementData_MasterData agreementData_MasterData = new AgreementData_MasterData();
				agreementData_MasterData.LoadInternal(br);
				dataHolder.Add(agreementData_MasterData);
			}
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, versionInfo);
			Serializer.Save(bw, dataHolder.Count);
			foreach (AgreementData_MasterData item in dataHolder)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(AgreementData_MasterDataHolder comp)
		{
			if (versionInfo != comp.versionInfo)
			{
				return false;
			}
			if (comp.dataHolder.Count != dataHolder.Count)
			{
				return false;
			}
			foreach (AgreementData_MasterData item in dataHolder)
			{
				bool flag = false;
				foreach (AgreementData_MasterData item2 in comp.dataHolder)
				{
					if (item.Equal(item2))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		public AgreementData_MasterDataHolder Clone()
		{
			AgreementData_MasterDataHolder agreementData_MasterDataHolder = new AgreementData_MasterDataHolder();
			CopyTo(agreementData_MasterDataHolder);
			return agreementData_MasterDataHolder;
		}

		public override void Clean()
		{
			versionInfo = 0;
			dataHolder.Clear();
		}
	}
}
