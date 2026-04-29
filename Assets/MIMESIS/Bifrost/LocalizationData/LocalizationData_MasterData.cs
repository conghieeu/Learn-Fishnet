using System;
using System.IO;
using System.Net;

namespace Bifrost.LocalizationData
{
	public class LocalizationData_MasterData : ISchema
	{
		public int id;

		public string key = string.Empty;

		public string en = string.Empty;

		public string ko = string.Empty;

		public string es = string.Empty;

		public string ja = string.Empty;

		public string fr = string.Empty;

		public string de = string.Empty;

		public string zh_cn = string.Empty;

		public string zh_tw = string.Empty;

		public string pt_br = string.Empty;

		public string it = string.Empty;

		public string ru = string.Empty;

		public string uk = string.Empty;

		public string vi = string.Empty;

		public string th = string.Empty;

		public string tr = string.Empty;

		public string pl = string.Empty;

		public LocalizationData_MasterData(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public LocalizationData_MasterData()
			: base(3342066989u, "LocalizationData_MasterData")
		{
		}

		public void CopyTo(LocalizationData_MasterData dest)
		{
			dest.id = id;
			dest.key = key;
			dest.en = en;
			dest.ko = ko;
			dest.es = es;
			dest.ja = ja;
			dest.fr = fr;
			dest.de = de;
			dest.zh_cn = zh_cn;
			dest.zh_tw = zh_tw;
			dest.pt_br = pt_br;
			dest.it = it;
			dest.ru = ru;
			dest.uk = uk;
			dest.vi = vi;
			dest.th = th;
			dest.tr = tr;
			dest.pl = pl;
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			return 0 + Serializer.GetLength(id) + Serializer.GetLength(key) + Serializer.GetLength(en) + Serializer.GetLength(ko) + Serializer.GetLength(es) + Serializer.GetLength(ja) + Serializer.GetLength(fr) + Serializer.GetLength(de) + Serializer.GetLength(zh_cn) + Serializer.GetLength(zh_tw) + Serializer.GetLength(pt_br) + Serializer.GetLength(it) + Serializer.GetLength(ru) + Serializer.GetLength(uk) + Serializer.GetLength(vi) + Serializer.GetLength(th) + Serializer.GetLength(tr) + Serializer.GetLength(pl);
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
			Serializer.Load(br, ref id);
			Serializer.Load(br, ref key);
			Serializer.Load(br, ref en);
			Serializer.Load(br, ref ko);
			Serializer.Load(br, ref es);
			Serializer.Load(br, ref ja);
			Serializer.Load(br, ref fr);
			Serializer.Load(br, ref de);
			Serializer.Load(br, ref zh_cn);
			Serializer.Load(br, ref zh_tw);
			Serializer.Load(br, ref pt_br);
			Serializer.Load(br, ref it);
			Serializer.Load(br, ref ru);
			Serializer.Load(br, ref uk);
			Serializer.Load(br, ref vi);
			Serializer.Load(br, ref th);
			Serializer.Load(br, ref tr);
			Serializer.Load(br, ref pl);
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, id);
			Serializer.Save(bw, key);
			Serializer.Save(bw, en);
			Serializer.Save(bw, ko);
			Serializer.Save(bw, es);
			Serializer.Save(bw, ja);
			Serializer.Save(bw, fr);
			Serializer.Save(bw, de);
			Serializer.Save(bw, zh_cn);
			Serializer.Save(bw, zh_tw);
			Serializer.Save(bw, pt_br);
			Serializer.Save(bw, it);
			Serializer.Save(bw, ru);
			Serializer.Save(bw, uk);
			Serializer.Save(bw, vi);
			Serializer.Save(bw, th);
			Serializer.Save(bw, tr);
			Serializer.Save(bw, pl);
		}

		public bool Equal(LocalizationData_MasterData comp)
		{
			if (id != comp.id)
			{
				return false;
			}
			if (key != comp.key)
			{
				return false;
			}
			if (en != comp.en)
			{
				return false;
			}
			if (ko != comp.ko)
			{
				return false;
			}
			if (es != comp.es)
			{
				return false;
			}
			if (ja != comp.ja)
			{
				return false;
			}
			if (fr != comp.fr)
			{
				return false;
			}
			if (de != comp.de)
			{
				return false;
			}
			if (zh_cn != comp.zh_cn)
			{
				return false;
			}
			if (zh_tw != comp.zh_tw)
			{
				return false;
			}
			if (pt_br != comp.pt_br)
			{
				return false;
			}
			if (it != comp.it)
			{
				return false;
			}
			if (ru != comp.ru)
			{
				return false;
			}
			if (uk != comp.uk)
			{
				return false;
			}
			if (vi != comp.vi)
			{
				return false;
			}
			if (th != comp.th)
			{
				return false;
			}
			if (tr != comp.tr)
			{
				return false;
			}
			if (pl != comp.pl)
			{
				return false;
			}
			return true;
		}

		public LocalizationData_MasterData Clone()
		{
			LocalizationData_MasterData localizationData_MasterData = new LocalizationData_MasterData();
			CopyTo(localizationData_MasterData);
			return localizationData_MasterData;
		}

		public override void Clean()
		{
			id = 0;
			key = string.Empty;
			en = string.Empty;
			ko = string.Empty;
			es = string.Empty;
			ja = string.Empty;
			fr = string.Empty;
			de = string.Empty;
			zh_cn = string.Empty;
			zh_tw = string.Empty;
			pt_br = string.Empty;
			it = string.Empty;
			ru = string.Empty;
			uk = string.Empty;
			vi = string.Empty;
			th = string.Empty;
			tr = string.Empty;
			pl = string.Empty;
		}
	}
}
