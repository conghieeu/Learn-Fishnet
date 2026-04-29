using System.Buffers;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using ReluProtocol.Enum;

namespace ReluProtocol.C2S
{
	[MemoryPackable(GenerateType.Object)]
	public class EnterDungeonRes : IResMsg, IMemoryPackable<EnterDungeonRes>, IMemoryPackFormatterRegister
	{
		[Preserve]
		private sealed class EnterDungeonResFormatter : MemoryPackFormatter<EnterDungeonRes>
		{
			[Preserve]
			public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref EnterDungeonRes value)
			{
				EnterDungeonRes.Serialize(ref writer, ref value);
			}

			[Preserve]
			public override void Deserialize(ref MemoryPackReader reader, ref EnterDungeonRes value)
			{
				EnterDungeonRes.Deserialize(ref reader, ref value);
			}
		}

		public PlayerInfo playerInfo { get; set; } = new PlayerInfo();

		public int shopGroupID { get; set; }

		public int currentWeatherMasterID { get; set; }

		public int forecastWeatherMasterID { get; set; }

		[MemoryPackConstructor]
		public EnterDungeonRes(int hashCode)
			: base(MsgType.C2S_EnterDungeonRes, hashCode)
		{
			base.reliable = true;
		}

		public EnterDungeonRes()
			: this(0)
		{
		}

		static EnterDungeonRes()
		{
			RegisterFormatter();
		}

		[Preserve]
		public new static void RegisterFormatter()
		{
			if (!MemoryPackFormatterProvider.IsRegistered<EnterDungeonRes>())
			{
				MemoryPackFormatterProvider.Register(new EnterDungeonResFormatter());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<EnterDungeonRes[]>())
			{
				MemoryPackFormatterProvider.Register(new ArrayFormatter<EnterDungeonRes>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
			}
			if (!MemoryPackFormatterProvider.IsRegistered<MsgErrorCode>())
			{
				MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgErrorCode>());
			}
		}

		[Preserve]
		public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref EnterDungeonRes? value) where TBufferWriter : class, IBufferWriter<byte>
		{
			if (value == null)
			{
				writer.WriteNullObjectHeader();
				return;
			}
			writer.WriteUnmanagedWithObjectHeader<MsgType, int, MsgErrorCode>(7, value.msgType, value.hashCode, value.errorCode);
			writer.WritePackable<PlayerInfo>(value.playerInfo);
			writer.WriteUnmanaged<int, int, int>(value.shopGroupID, value.currentWeatherMasterID, value.forecastWeatherMasterID);
		}

		[Preserve]
		public static void Deserialize(ref MemoryPackReader reader, ref EnterDungeonRes? value)
		{
			if (!reader.TryReadObjectHeader(out var memberCount))
			{
				value = null;
				return;
			}
			MsgType value2;
			int value3;
			MsgErrorCode value4;
			PlayerInfo value5;
			int value6;
			int value7;
			int value8;
			if (memberCount == 7)
			{
				if (value == null)
				{
					reader.ReadUnmanaged<MsgType, int, MsgErrorCode>(out value2, out value3, out value4);
					value5 = reader.ReadPackable<PlayerInfo>();
					reader.ReadUnmanaged<int, int, int>(out value6, out value7, out value8);
				}
				else
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					value4 = value.errorCode;
					value5 = value.playerInfo;
					value6 = value.shopGroupID;
					value7 = value.currentWeatherMasterID;
					value8 = value.forecastWeatherMasterID;
					reader.ReadUnmanaged<MsgType>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadUnmanaged<MsgErrorCode>(out value4);
					reader.ReadPackable(ref value5);
					reader.ReadUnmanaged<int>(out value6);
					reader.ReadUnmanaged<int>(out value7);
					reader.ReadUnmanaged<int>(out value8);
				}
			}
			else
			{
				if (memberCount > 7)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(EnterDungeonRes), 7, memberCount);
					return;
				}
				if (value == null)
				{
					value2 = MsgType.Invalid;
					value3 = 0;
					value4 = MsgErrorCode.Success;
					value5 = null;
					value6 = 0;
					value7 = 0;
					value8 = 0;
				}
				else
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					value4 = value.errorCode;
					value5 = value.playerInfo;
					value6 = value.shopGroupID;
					value7 = value.currentWeatherMasterID;
					value8 = value.forecastWeatherMasterID;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<MsgType>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<int>(out value3);
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<MsgErrorCode>(out value4);
							if (memberCount != 3)
							{
								reader.ReadPackable(ref value5);
								if (memberCount != 4)
								{
									reader.ReadUnmanaged<int>(out value6);
									if (memberCount != 5)
									{
										reader.ReadUnmanaged<int>(out value7);
										if (memberCount != 6)
										{
											reader.ReadUnmanaged<int>(out value8);
											_ = 7;
										}
									}
								}
							}
						}
					}
				}
				_ = value;
			}
			value = new EnterDungeonRes(value3)
			{
				msgType = value2,
				errorCode = value4,
				playerInfo = value5,
				shopGroupID = value6,
				currentWeatherMasterID = value7,
				forecastWeatherMasterID = value8
			};
		}
	}
}
