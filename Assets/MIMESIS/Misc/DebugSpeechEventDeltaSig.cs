using System;
using System.Buffers;
using System.IO;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;
using Mimic.Voice.SpeechSystem;
using ReluProtocol;
using ReluProtocol.Enum;

[MemoryPackable(GenerateType.Object)]
public class DebugSpeechEventDeltaSig : IActorMsg, IMemoryPackable<DebugSpeechEventDeltaSig>, IMemoryPackFormatterRegister
{
	[Preserve]
	private sealed class DebugSpeechEventDeltaSigFormatter : MemoryPackFormatter<DebugSpeechEventDeltaSig>
	{
		[Preserve]
		public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref DebugSpeechEventDeltaSig value)
		{
			DebugSpeechEventDeltaSig.Serialize(ref writer, ref value);
		}

		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref DebugSpeechEventDeltaSig value)
		{
			DebugSpeechEventDeltaSig.Deserialize(ref reader, ref value);
		}
	}

	[MemoryPackInclude]
	public sbyte delta;

	[MemoryPackInclude]
	public byte[] serializedData;

	[MemoryPackIgnore]
	public SpeechType_AdjacentPlayerCount adjacentCount;

	[MemoryPackIgnore]
	public SpeechType_Area area;

	[MemoryPackIgnore]
	public SpeechType_GameTime gameTime;

	[MemoryPackIgnore]
	public SpeechType_FacingPlayerCount facingCount;

	[MemoryPackIgnore]
	public SpeechType_Teleporter teleporter;

	[MemoryPackIgnore]
	public SpeechType_IndoorEntered indoorEntered;

	[MemoryPackIgnore]
	public SpeechType_Charger charger;

	[MemoryPackIgnore]
	public SpeechType_CrowShop crowShop;

	[MemoryPackIgnore]
	public IncomingEvent[] incomingEvents;

	[MemoryPackIgnore]
	public int[] scrapObjects;

	[MemoryPackIgnore]
	public int[] monsters;

	public DebugSpeechEventDeltaSig()
		: base(MsgType.C2S_DebugSpeechEventDeltaSig)
	{
		incomingEvents = Array.Empty<IncomingEvent>();
		scrapObjects = Array.Empty<int>();
		monsters = Array.Empty<int>();
		serializedData = Array.Empty<byte>();
	}

	[MemoryPackOnSerializing]
	public void OnSerializing()
	{
		PackCustomData();
	}

	[MemoryPackOnDeserialized]
	public void OnDeserialized()
	{
		UnpackCustomData();
	}

	private void PackCustomData()
	{
		using MemoryStream memoryStream = new MemoryStream();
		using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		ushort num = 0;
		if (adjacentCount != SpeechType_AdjacentPlayerCount.Monologue)
		{
			num |= 1;
		}
		if (area != SpeechType_Area.None)
		{
			num |= 2;
		}
		if (gameTime != SpeechType_GameTime.First10)
		{
			num |= 4;
		}
		if (facingCount != SpeechType_FacingPlayerCount.None)
		{
			num |= 8;
		}
		if (teleporter != SpeechType_Teleporter.None)
		{
			num |= 0x10;
		}
		if (indoorEntered != SpeechType_IndoorEntered.None)
		{
			num |= 0x20;
		}
		if (charger != SpeechType_Charger.None)
		{
			num |= 0x40;
		}
		if (crowShop != SpeechType_CrowShop.None)
		{
			num |= 0x80;
		}
		IncomingEvent[] array = incomingEvents;
		if (array != null && array.Length != 0)
		{
			num |= 0x100;
		}
		int[] array2 = scrapObjects;
		if (array2 != null && array2.Length != 0)
		{
			num |= 0x200;
		}
		int[] array3 = monsters;
		if (array3 != null && array3.Length != 0)
		{
			num |= 0x400;
		}
		binaryWriter.Write(num);
		if ((num & 1) != 0)
		{
			binaryWriter.Write((byte)adjacentCount);
		}
		if ((num & 2) != 0)
		{
			binaryWriter.Write((byte)area);
		}
		if ((num & 4) != 0)
		{
			binaryWriter.Write((byte)gameTime);
		}
		if ((num & 8) != 0)
		{
			binaryWriter.Write((byte)facingCount);
		}
		if ((num & 0x10) != 0)
		{
			binaryWriter.Write((byte)teleporter);
		}
		if ((num & 0x20) != 0)
		{
			binaryWriter.Write((byte)indoorEntered);
		}
		if ((num & 0x40) != 0)
		{
			binaryWriter.Write((byte)charger);
		}
		if ((num & 0x80) != 0)
		{
			binaryWriter.Write((byte)crowShop);
		}
		if ((num & 0x100) != 0)
		{
			binaryWriter.Write(incomingEvents.Length);
			IncomingEvent[] array4 = incomingEvents;
			for (int i = 0; i < array4.Length; i++)
			{
				IncomingEvent incomingEvent = array4[i];
				binaryWriter.Write(incomingEvent.EventExpireTime);
				binaryWriter.Write((byte)incomingEvent.EventType);
				binaryWriter.Write(incomingEvent.EventID);
			}
		}
		if ((num & 0x200) != 0)
		{
			binaryWriter.Write(scrapObjects.Length);
			int[] array5 = scrapObjects;
			foreach (int value in array5)
			{
				binaryWriter.Write(value);
			}
		}
		if ((num & 0x400) != 0)
		{
			binaryWriter.Write(monsters.Length);
			int[] array5 = monsters;
			foreach (int value2 in array5)
			{
				binaryWriter.Write(value2);
			}
		}
		serializedData = memoryStream.ToArray();
	}

	private void UnpackCustomData()
	{
		if (serializedData == null || serializedData.Length == 0)
		{
			incomingEvents = Array.Empty<IncomingEvent>();
			scrapObjects = Array.Empty<int>();
			monsters = Array.Empty<int>();
			return;
		}
		using MemoryStream input = new MemoryStream(serializedData);
		using BinaryReader binaryReader = new BinaryReader(input);
		ushort num = binaryReader.ReadUInt16();
		if ((num & 1) != 0)
		{
			adjacentCount = (SpeechType_AdjacentPlayerCount)binaryReader.ReadByte();
		}
		else
		{
			adjacentCount = SpeechType_AdjacentPlayerCount.Monologue;
		}
		if ((num & 2) != 0)
		{
			area = (SpeechType_Area)binaryReader.ReadByte();
		}
		else
		{
			area = SpeechType_Area.None;
		}
		if ((num & 4) != 0)
		{
			gameTime = (SpeechType_GameTime)binaryReader.ReadByte();
		}
		else
		{
			gameTime = SpeechType_GameTime.First10;
		}
		if ((num & 8) != 0)
		{
			facingCount = (SpeechType_FacingPlayerCount)binaryReader.ReadByte();
		}
		else
		{
			facingCount = SpeechType_FacingPlayerCount.None;
		}
		if ((num & 0x10) != 0)
		{
			teleporter = (SpeechType_Teleporter)binaryReader.ReadByte();
		}
		else
		{
			teleporter = SpeechType_Teleporter.None;
		}
		if ((num & 0x20) != 0)
		{
			indoorEntered = (SpeechType_IndoorEntered)binaryReader.ReadByte();
		}
		else
		{
			indoorEntered = SpeechType_IndoorEntered.None;
		}
		if ((num & 0x40) != 0)
		{
			charger = (SpeechType_Charger)binaryReader.ReadByte();
		}
		else
		{
			charger = SpeechType_Charger.None;
		}
		if ((num & 0x80) != 0)
		{
			crowShop = (SpeechType_CrowShop)binaryReader.ReadByte();
		}
		else
		{
			crowShop = SpeechType_CrowShop.None;
		}
		if ((num & 0x100) != 0)
		{
			int num2 = binaryReader.ReadInt32();
			incomingEvents = new IncomingEvent[num2];
			for (int i = 0; i < num2; i++)
			{
				incomingEvents[i] = new IncomingEvent
				{
					EventExpireTime = binaryReader.ReadSingle(),
					EventType = (SpeechEvent_IncomingType)binaryReader.ReadByte(),
					EventID = binaryReader.ReadInt32()
				};
			}
		}
		else
		{
			incomingEvents = Array.Empty<IncomingEvent>();
		}
		if ((num & 0x200) != 0)
		{
			int num3 = binaryReader.ReadInt32();
			scrapObjects = new int[num3];
			for (int j = 0; j < num3; j++)
			{
				scrapObjects[j] = binaryReader.ReadInt32();
			}
		}
		else
		{
			scrapObjects = Array.Empty<int>();
		}
		if ((num & 0x400) != 0)
		{
			int num4 = binaryReader.ReadInt32();
			monsters = new int[num4];
			for (int k = 0; k < num4; k++)
			{
				monsters[k] = binaryReader.ReadInt32();
			}
		}
		else
		{
			monsters = Array.Empty<int>();
		}
	}

	static DebugSpeechEventDeltaSig()
	{
		RegisterFormatter();
	}

	[Preserve]
	public new static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<DebugSpeechEventDeltaSig>())
		{
			MemoryPackFormatterProvider.Register(new DebugSpeechEventDeltaSigFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<DebugSpeechEventDeltaSig[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<DebugSpeechEventDeltaSig>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<MsgType>())
		{
			MemoryPackFormatterProvider.Register(new UnmanagedFormatter<MsgType>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<byte[]>())
		{
			MemoryPackFormatterProvider.Register(new ArrayFormatter<byte>());
		}
	}

	[Preserve]
	public static void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref DebugSpeechEventDeltaSig? value) where TBufferWriter : class, IBufferWriter<byte>
	{
		value?.OnSerializing();
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteUnmanagedWithObjectHeader<MsgType, int, int, sbyte>(5, value.msgType, value.hashCode, value.actorID, in value.delta);
		writer.WriteUnmanagedArray(value.serializedData);
	}

	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, ref DebugSpeechEventDeltaSig? value)
	{
		MsgType value2;
		int value3;
		int value4;
		sbyte value5;
		byte[] value6;
		if (!reader.TryReadObjectHeader(out var memberCount))
		{
			value = null;
		}
		else
		{
			if (memberCount == 5)
			{
				if (value != null)
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					value4 = value.actorID;
					value5 = value.delta;
					value6 = value.serializedData;
					reader.ReadUnmanaged<MsgType>(out value2);
					reader.ReadUnmanaged<int>(out value3);
					reader.ReadUnmanaged<int>(out value4);
					reader.ReadUnmanaged<sbyte>(out value5);
					reader.ReadUnmanagedArray(ref value6);
					goto IL_0125;
				}
				reader.ReadUnmanaged<MsgType, int, int, sbyte>(out value2, out value3, out value4, out value5);
				value6 = reader.ReadUnmanagedArray<byte>();
			}
			else
			{
				if (memberCount > 5)
				{
					MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(DebugSpeechEventDeltaSig), 5, memberCount);
					goto IL_017d;
				}
				if (value == null)
				{
					value2 = MsgType.Invalid;
					value3 = 0;
					value4 = 0;
					value5 = 0;
					value6 = null;
				}
				else
				{
					value2 = value.msgType;
					value3 = value.hashCode;
					value4 = value.actorID;
					value5 = value.delta;
					value6 = value.serializedData;
				}
				if (memberCount != 0)
				{
					reader.ReadUnmanaged<MsgType>(out value2);
					if (memberCount != 1)
					{
						reader.ReadUnmanaged<int>(out value3);
						if (memberCount != 2)
						{
							reader.ReadUnmanaged<int>(out value4);
							if (memberCount != 3)
							{
								reader.ReadUnmanaged<sbyte>(out value5);
								if (memberCount != 4)
								{
									reader.ReadUnmanagedArray(ref value6);
									_ = 5;
								}
							}
						}
					}
				}
				if (value != null)
				{
					goto IL_0125;
				}
			}
			value = new DebugSpeechEventDeltaSig
			{
				msgType = value2,
				hashCode = value3,
				actorID = value4,
				delta = value5,
				serializedData = value6
			};
		}
		goto IL_017d;
		IL_017d:
		value?.OnDeserialized();
		return;
		IL_0125:
		value.msgType = value2;
		value.hashCode = value3;
		value.actorID = value4;
		value.delta = value5;
		value.serializedData = value6;
		goto IL_017d;
	}
}
