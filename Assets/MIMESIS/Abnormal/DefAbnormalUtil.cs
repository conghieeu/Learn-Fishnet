using System;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Bifrost.ConstEnum;

public class DefAbnormalUtil
{
	public static DispelType ChangeImmuneType2DispelType(ImmuneType type)
	{
		return type switch
		{
			ImmuneType.AllCC => DispelType.AllCC, 
			ImmuneType.AllStats => DispelType.AllStats, 
			ImmuneType.TargetCC => DispelType.TargetCC, 
			ImmuneType.TargetImmutableStat => DispelType.TargetImmutableStat, 
			ImmuneType.TargetMutableStat => throw new Exception("TargetMutableStats is not supported"), 
			ImmuneType.TargetAbnormalID => DispelType.AbnormalID, 
			ImmuneType.Immortal => DispelType.ALL, 
			_ => DispelType.None, 
		};
	}

	public static ImmuneGrade ChangeImmuneType2Grade(ImmuneType type)
	{
		return type switch
		{
			ImmuneType.Immortal => ImmuneGrade.Immortal, 
			ImmuneType.AllCC => ImmuneGrade.AllCC, 
			ImmuneType.AllStats => ImmuneGrade.AllStats, 
			_ => ImmuneGrade.Invalid, 
		};
	}

	public static int GetPriority(CCType ccType)
	{
		return Hub.s.dataman.ExcelDataManager.GetDefAbnormal_MasterData(ccType)?.motion_priority ?? 9999999;
	}

	public static bool IsActionAbnormal(CCType ccType)
	{
		if (ccType != CCType.NormalPush && ccType != CCType.Knockback && ccType != CCType.Knockdown)
		{
			return ccType == CCType.Airborne;
		}
		return true;
	}

	public static StatType GetCCResistStatType(CCType ccType)
	{
		return StatType.Invalid;
	}

	public static bool GetTypeFieldImmune(string Input, out ImmutableArray<ImmuneData> output)
	{
		MatchCollection matchCollection = Regex.Matches(Regex.Replace(Input, "\\s+", ""), "(?<=\\()[^()]*(?=\\))");
		ImmutableArray<ImmuneData>.Builder builder = ImmutableArray.CreateBuilder<ImmuneData>();
		foreach (Match item in matchCollection)
		{
			string text = item.ToString();
			if (text == null)
			{
				Logger.RError("GetTypeFieldImmune immuneData is null");
				continue;
			}
			string[] array = text.Split(',');
			if (!Enum.TryParse<ImmuneType>(array[0], out var result))
			{
				Logger.RError("GetTypeFieldImmune immuneData type is invalid");
				continue;
			}
			switch (result)
			{
			case ImmuneType.TargetCC:
			{
				if (!Enum.TryParse<CCType>(array[1], out var result4))
				{
					Logger.RError("GetTypeFieldImmune ccType is invalid");
				}
				else
				{
					builder.Add(new ImmuneData(result4));
				}
				break;
			}
			case ImmuneType.TargetMutableStat:
			{
				if (!Enum.TryParse<MutableStatType>(array[1], out var result3))
				{
					Logger.RError("GetTypeFieldImmune mutableStatType is invalid");
				}
				else
				{
					builder.Add(new ImmuneData(result3));
				}
				break;
			}
			case ImmuneType.TargetImmutableStat:
			{
				if (!Enum.TryParse<StatType>(array[1], out var result2))
				{
					Logger.RError("GetTypeFieldImmune immutableStatType is invalid");
				}
				else
				{
					builder.Add(new ImmuneData(result2));
				}
				break;
			}
			default:
				builder.Add(new ImmuneData(result));
				break;
			}
		}
		output = builder.ToImmutable();
		return true;
	}
}
