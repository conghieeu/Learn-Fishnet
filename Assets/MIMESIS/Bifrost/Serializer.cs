using System;
using System.IO;
using System.Net;
using System.Text;
using MemoryPack;
using Newtonsoft.Json;
using ReluProtocol;

namespace Bifrost
{
	public static class Serializer
	{
		public static void Clone(ISchema src, ISchema dst)
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter bw = new BinaryWriter(memoryStream);
			src.Save(bw);
			memoryStream.Position = 0L;
			BinaryReader bw2 = new BinaryReader(memoryStream);
			dst.Load(bw2);
		}

		public static bool LinkPackagingForBytesStream(BinaryWriter binWriter, ISchema msg)
		{
			binWriter.Write(msg.GetLength() + 4);
			msg.Save(binWriter);
			return true;
		}

		public static string ToJSon(ISchema msg)
		{
			return JsonConvert.SerializeObject(msg);
		}

		public static T FromJSon<T>(string msg) where T : ISchema, new()
		{
			new T();
			return JsonConvert.DeserializeObject<T>(msg) ?? new T();
		}

		public static void Load(BinaryReader binReader, ref bool boolValue)
		{
			boolValue = binReader.ReadBoolean();
		}

		public static void Load(BinaryReader binReader, ref byte nValue)
		{
			nValue = binReader.ReadByte();
		}

		public static void Load(BinaryReader binReader, ref sbyte nValue)
		{
			nValue = binReader.ReadSByte();
		}

		public static void Load(BinaryReader binReader, ref ushort nValue)
		{
			nValue = binReader.ReadUInt16();
		}

		public static void Load(BinaryReader binReader, ref short nValue)
		{
			nValue = binReader.ReadInt16();
		}

		public static void Load(BinaryReader reader, ref uint uintValue)
		{
			uintValue = reader.ReadUInt32();
		}

		public static void Load(BinaryReader reader, ref int intValue)
		{
			intValue = reader.ReadInt32();
		}

		public static void Load(BinaryReader binReader, ref ulong nValue)
		{
			nValue = binReader.ReadUInt64();
		}

		public static void Load(BinaryReader binReader, ref long nValue)
		{
			nValue = binReader.ReadInt64();
		}

		public static void Load(BinaryReader binReader, ref double nValue)
		{
			nValue = binReader.ReadDouble();
		}

		public static void Load(BinaryReader binReader, ref float nValue)
		{
			nValue = binReader.ReadSingle();
		}

		public static void Load(BinaryReader binReader, ref char nValue)
		{
			nValue = binReader.ReadChar();
		}

		public static void Load(BinaryReader binReader, ref string strValue)
		{
			int num = IPAddress.NetworkToHostOrder(binReader.ReadInt32());
			if (0 < num)
			{
				byte[] bytes = binReader.ReadBytes(num);
				strValue = Encoding.UTF8.GetString(bytes);
			}
			else
			{
				strValue = string.Empty;
			}
		}

		public static void Load(BinaryReader binReader, ref MemoryStream stream)
		{
			int num = IPAddress.NetworkToHostOrder(binReader.ReadInt32());
			if (0 < num)
			{
				byte[] array = binReader.ReadBytes(num);
				stream.Write(array, 0, array.Length);
				stream.Position = 0L;
			}
		}

		public static void Save(BinaryWriter binWriter, bool boolValue)
		{
			binWriter.Write(boolValue);
		}

		public static void Save(BinaryWriter binWriter, byte nValue)
		{
			binWriter.Write(nValue);
		}

		public static void Save(BinaryWriter binWriter, sbyte nValue)
		{
			binWriter.Write(nValue);
		}

		public static void Save(BinaryWriter binWriter, ushort nValue)
		{
			binWriter.Write(nValue);
		}

		public static void Save(BinaryWriter binWriter, short nValue)
		{
			binWriter.Write(nValue);
		}

		public static void Save(BinaryWriter binWriter, uint nValue)
		{
			binWriter.Write(nValue);
		}

		public static void Save(BinaryWriter binWriter, int nValue)
		{
			binWriter.Write(nValue);
		}

		public static void Save(BinaryWriter binWriter, ulong nValue)
		{
			binWriter.Write(nValue);
		}

		public static void Save(BinaryWriter binWriter, long nValue)
		{
			binWriter.Write(nValue);
		}

		public static void Save(BinaryWriter binWriter, double nValue)
		{
			binWriter.Write(nValue);
		}

		public static void Save(BinaryWriter binWriter, float nValue)
		{
			binWriter.Write(nValue);
		}

		public static void Save(BinaryWriter binWriter, char nValue)
		{
			binWriter.Write(nValue);
		}

		public static void Save(BinaryWriter binWriter, byte[] pkt)
		{
			binWriter.Write(pkt);
		}

		public static void Save(BinaryWriter binWriter, string strValue)
		{
			if (strValue == null || strValue.Length == 0)
			{
				binWriter.Write(0);
				return;
			}
			binWriter.Write(IPAddress.HostToNetworkOrder(Encoding.UTF8.GetBytes(strValue).Length));
			binWriter.Write(Encoding.UTF8.GetBytes(strValue));
		}

		public static void Save(BinaryWriter binWriter, MemoryStream stream)
		{
			binWriter.Write(IPAddress.HostToNetworkOrder((int)stream.Length));
			binWriter.Write(stream.ToArray(), 0, (int)stream.Length);
		}

		public static int GetLength(bool boolValue)
		{
			return 1;
		}

		public static int GetLength(byte nValue)
		{
			return 1;
		}

		public static int GetLength(sbyte nValue)
		{
			return 1;
		}

		public static int GetLength(ushort nValue)
		{
			return 2;
		}

		public static int GetLength(short nValue)
		{
			return 2;
		}

		public static int GetLength(uint nValue)
		{
			return 4;
		}

		public static int GetLength(int nValue)
		{
			return 4;
		}

		public static int GetLength(ulong nValue)
		{
			return 8;
		}

		public static int GetLength(long nValue)
		{
			return 8;
		}

		public static int GetLength(double nValue)
		{
			return 8;
		}

		public static int GetLength(float nValue)
		{
			return 4;
		}

		public static int GetLength(char nValue)
		{
			return 2;
		}

		public static int GetLength(string strValue)
		{
			if (strValue == null || strValue.Length == 0)
			{
				return 4;
			}
			return 4 + Encoding.UTF8.GetBytes(strValue).Length;
		}

		public static int GetLength(MemoryStream stream)
		{
			return 4 + (int)stream.Length;
		}

		public static byte[] Serialize<T>(T msg) where T : IMsg
		{
			try
			{
				return MemoryPackSerializer.Serialize(in msg);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		public static T Deserialize<T>(MemoryStream stream) where T : IMsg, new()
		{
			try
			{
				T val = MemoryPackSerializer.Deserialize<T>(stream.GetBuffer());
				if (val == null)
				{
					Logger.RError("Failed to Deserialize Msg");
					return new T();
				}
				return val;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		public static T? Deserialize<T>(in ArraySegment<byte> data) where T : IMsg, new()
		{
			try
			{
				T val = MemoryPackSerializer.Deserialize<T>(data);
				if (val == null)
				{
					Logger.RError("Failed to Deserialize Msg");
					return null;
				}
				return val;
			}
			catch (Exception e)
			{
				Logger.RError(e);
				return null;
			}
		}

		public static T Deserialize<T>(string jsonBody) where T : IMsg, new()
		{
			try
			{
				T val = JsonConvert.DeserializeObject<T>(jsonBody);
				if (val == null)
				{
					Logger.RError($"Json Deserialize Failed. message body: {jsonBody}, message Type : {new T().msgType}");
					return new T();
				}
				return val;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		public static void Load(byte[] buffer, ref bool boolValue)
		{
			boolValue = BitConverter.ToBoolean(buffer, 0);
		}

		public static void Load(byte[] buffer, ref byte nValue)
		{
			nValue = buffer[0];
		}

		public static void Load(byte[] buffer, ref sbyte nValue)
		{
			nValue = (sbyte)buffer[0];
		}

		public static void Load(byte[] buffer, ref ushort nValue)
		{
			nValue = BitConverter.ToUInt16(buffer, 0);
		}

		public static void Load(byte[] buffer, ref short nValue)
		{
			nValue = BitConverter.ToInt16(buffer, 0);
		}

		public static void Load(byte[] buffer, ref uint uintValue)
		{
			uintValue = BitConverter.ToUInt32(buffer, 0);
		}

		public static void Load(byte[] buffer, ref int intValue)
		{
			intValue = BitConverter.ToInt32(buffer, 0);
		}

		public static void Load(byte[] buffer, ref ulong nValue)
		{
			nValue = BitConverter.ToUInt64(buffer, 0);
		}

		public static void Load(byte[] buffer, ref long nValue)
		{
			nValue = BitConverter.ToInt64(buffer, 0);
		}

		public static void Load(byte[] buffer, ref double nValue)
		{
			nValue = BitConverter.ToDouble(buffer, 0);
		}

		public static void Load(byte[] buffer, ref float nValue)
		{
			nValue = BitConverter.ToSingle(buffer, 0);
		}

		public static void Load(byte[] buffer, ref char nValue)
		{
			nValue = BitConverter.ToChar(buffer, 0);
		}

		public static void Load(byte[] buffer, ref string nValue)
		{
			nValue = Encoding.UTF8.GetString(buffer);
		}

		public static void Load(byte[] buffer, ref byte[] nValue)
		{
			nValue = buffer;
		}

		public static void Load(byte[] buffer, ref ArraySegment<byte> nValue)
		{
			nValue = new ArraySegment<byte>(buffer);
		}

		public static void Load(byte[] buffer, ref ArraySegment<byte> nValue, int offset, int count)
		{
			nValue = new ArraySegment<byte>(buffer, offset, count);
		}

		public static void Load(ArraySegment<byte> segment, int offset, ref bool boolValue)
		{
			if (segment.Array == null || segment.Offset < 0 || segment.Offset >= segment.Array.Length)
			{
				throw new ArgumentException("Invalid ArraySegment");
			}
			boolValue = BitConverter.ToBoolean(segment.Array, offset);
		}

		public static void Load(ArraySegment<byte> segment, int offset, ref byte nValue)
		{
			if (segment.Array == null || segment.Offset < 0 || segment.Offset >= segment.Array.Length)
			{
				throw new ArgumentException("Invalid ArraySegment");
			}
			nValue = segment.Array[0];
		}

		public static void Load(ArraySegment<byte> segment, int offset, ref sbyte nValue)
		{
			if (segment.Array == null || segment.Offset < 0 || segment.Offset >= segment.Array.Length)
			{
				throw new ArgumentException("Invalid ArraySegment");
			}
			nValue = (sbyte)segment.Array[0];
		}

		public static void Load(ArraySegment<byte> segment, int offset, ref ushort nValue)
		{
			if (segment.Array == null || segment.Offset < 0 || segment.Offset >= segment.Array.Length)
			{
				throw new ArgumentException("Invalid ArraySegment");
			}
			nValue = BitConverter.ToUInt16(segment.Array, offset);
		}

		public static void Load(ArraySegment<byte> segment, int offset, ref short nValue)
		{
			if (segment.Array == null || segment.Offset < 0 || segment.Offset >= segment.Array.Length)
			{
				throw new ArgumentException("Invalid ArraySegment");
			}
			nValue = BitConverter.ToInt16(segment.Array, offset);
		}

		public static void Load(ArraySegment<byte> segment, int offset, ref uint uintValue)
		{
			if (segment.Array == null || segment.Offset < 0 || segment.Offset >= segment.Array.Length)
			{
				throw new ArgumentException("Invalid ArraySegment");
			}
			uintValue = BitConverter.ToUInt32(segment.Array, offset);
		}

		public static void Load(ArraySegment<byte> segment, int offset, ref int intValue)
		{
			if (segment.Array == null || segment.Offset < 0 || segment.Offset >= segment.Array.Length)
			{
				throw new ArgumentException("Invalid ArraySegment");
			}
			intValue = BitConverter.ToInt32(segment.Array, offset);
		}

		public static void Load(ArraySegment<byte> segment, int offset, ref ulong nValue)
		{
			if (segment.Array == null || segment.Offset < 0 || segment.Offset >= segment.Array.Length)
			{
				throw new ArgumentException("Invalid ArraySegment");
			}
			nValue = BitConverter.ToUInt64(segment.Array, offset);
		}

		public static void Load(ArraySegment<byte> segment, int offset, ref long nValue)
		{
			if (segment.Array == null || segment.Offset < 0 || segment.Offset >= segment.Array.Length)
			{
				throw new ArgumentException("Invalid ArraySegment");
			}
			nValue = BitConverter.ToInt64(segment.Array, offset);
		}

		public static void Load(ArraySegment<byte> segment, int offset, ref double nValue)
		{
			if (segment.Array == null || segment.Offset < 0 || segment.Offset >= segment.Array.Length)
			{
				throw new ArgumentException("Invalid ArraySegment");
			}
			nValue = BitConverter.ToDouble(segment.Array, offset);
		}

		public static void Load(ArraySegment<byte> segment, int offset, ref float nValue)
		{
			if (segment.Array == null || segment.Offset < 0 || segment.Offset >= segment.Array.Length)
			{
				throw new ArgumentException("Invalid ArraySegment");
			}
			nValue = BitConverter.ToSingle(segment.Array, offset);
		}

		public static void Load(ArraySegment<byte> segment, int offset, ref char nValue)
		{
			if (segment.Array == null || segment.Offset < 0 || segment.Offset >= segment.Array.Length)
			{
				throw new ArgumentException("Invalid ArraySegment");
			}
			nValue = BitConverter.ToChar(segment.Array, offset);
		}

		public static void Load(ArraySegment<byte> segment, int offset, ref string nValue)
		{
			if (segment.Array == null || segment.Offset < 0 || segment.Offset >= segment.Array.Length)
			{
				throw new ArgumentException("Invalid ArraySegment");
			}
			nValue = Encoding.UTF8.GetString(segment.Array, segment.Offset, segment.Count);
		}

		public static void Load(ArraySegment<byte> segment, int offset, ref byte[] nValue)
		{
			nValue = new byte[segment.Count];
			if (segment.Array == null || segment.Offset < 0 || segment.Offset >= segment.Array.Length)
			{
				throw new ArgumentException("Invalid ArraySegment");
			}
			Array.Copy(segment.Array, segment.Offset, nValue, 0, segment.Count);
		}

		public static void Load(ArraySegment<byte> segment, int offset, ref ArraySegment<byte> nValue)
		{
			nValue = segment;
		}
	}
}
