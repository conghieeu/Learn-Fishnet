using System;
using System.IO;
using System.Net;

namespace Bifrost.TramupgradeData
{
	public class TramupgradeData_MasterData : ISchema
	{
		public int id;

		public string name = string.Empty;

		public string upgrade_name = string.Empty;

		public string tooltip = string.Empty;

		public string icon = string.Empty;

		public int rate;

		public int anim_pointoftime;

		public string upgrade_tram_parts_name = string.Empty;

		public TramupgradeData_MasterData(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public TramupgradeData_MasterData()
			: base(1809265180u, "TramupgradeData_MasterData")
		{
		}

		public void CopyTo(TramupgradeData_MasterData dest)
		{
			dest.id = id;
			dest.name = name;
			dest.upgrade_name = upgrade_name;
			dest.tooltip = tooltip;
			dest.icon = icon;
			dest.rate = rate;
			dest.anim_pointoftime = anim_pointoftime;
			dest.upgrade_tram_parts_name = upgrade_tram_parts_name;
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			return 0 + Serializer.GetLength(id) + Serializer.GetLength(name) + Serializer.GetLength(upgrade_name) + Serializer.GetLength(tooltip) + Serializer.GetLength(icon) + Serializer.GetLength(rate) + Serializer.GetLength(anim_pointoftime) + Serializer.GetLength(upgrade_tram_parts_name);
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
			Serializer.Load(br, ref name);
			Serializer.Load(br, ref upgrade_name);
			Serializer.Load(br, ref tooltip);
			Serializer.Load(br, ref icon);
			Serializer.Load(br, ref rate);
			Serializer.Load(br, ref anim_pointoftime);
			Serializer.Load(br, ref upgrade_tram_parts_name);
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, id);
			Serializer.Save(bw, name);
			Serializer.Save(bw, upgrade_name);
			Serializer.Save(bw, tooltip);
			Serializer.Save(bw, icon);
			Serializer.Save(bw, rate);
			Serializer.Save(bw, anim_pointoftime);
			Serializer.Save(bw, upgrade_tram_parts_name);
		}

		public bool Equal(TramupgradeData_MasterData comp)
		{
			if (id != comp.id)
			{
				return false;
			}
			if (name != comp.name)
			{
				return false;
			}
			if (upgrade_name != comp.upgrade_name)
			{
				return false;
			}
			if (tooltip != comp.tooltip)
			{
				return false;
			}
			if (icon != comp.icon)
			{
				return false;
			}
			if (rate != comp.rate)
			{
				return false;
			}
			if (anim_pointoftime != comp.anim_pointoftime)
			{
				return false;
			}
			if (upgrade_tram_parts_name != comp.upgrade_tram_parts_name)
			{
				return false;
			}
			return true;
		}

		public TramupgradeData_MasterData Clone()
		{
			TramupgradeData_MasterData tramupgradeData_MasterData = new TramupgradeData_MasterData();
			CopyTo(tramupgradeData_MasterData);
			return tramupgradeData_MasterData;
		}

		public override void Clean()
		{
			id = 0;
			name = string.Empty;
			upgrade_name = string.Empty;
			tooltip = string.Empty;
			icon = string.Empty;
			rate = 0;
			anim_pointoftime = 0;
			upgrade_tram_parts_name = string.Empty;
		}
	}
}
