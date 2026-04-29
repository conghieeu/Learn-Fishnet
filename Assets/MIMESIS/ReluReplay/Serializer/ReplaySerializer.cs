using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using MemoryPack;
using MemoryPack.Compression;
using Mimic.Actors;
using ReluProtocol;
using ReluReplay.Data;

namespace ReluReplay.Serializer
{
	public static class ReplaySerializer
	{
		private static readonly object _msgFileLock = new object();

		private static readonly object _sndFileLock = new object();

		public static void SerializeHeader(IReplayHeader header, in string filePath)
		{
			using FileStream fileStream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write);
			byte[] array = MemoryPackSerializer.Serialize(in header);
			byte[] bytes = BitConverter.GetBytes(array.Length);
			fileStream.Write(bytes, 0, bytes.Length);
			fileStream.Write(array, 0, array.Length);
			fileStream.Flush();
		}

		public static void AddHeaderToExistingFile(ReplayHeader header, string filePath)
		{
			string text = filePath + ".temp";
			try
			{
				byte[] array;
				using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
				{
					array = new byte[fileStream.Length];
					fileStream.Read(array, 0, array.Length);
				}
				using (FileStream fileStream2 = new FileStream(text, FileMode.Create, FileAccess.Write))
				{
					Span<byte> span = stackalloc byte[4];
					span[0] = 82;
					span[1] = 69;
					span[2] = 76;
					span[3] = 85;
					fileStream2.Write(span);
					byte[] array2 = MemoryPackSerializer.Serialize(in header);
					Span<byte> span2 = stackalloc byte[4];
					BitConverter.TryWriteBytes(span2, array2.Length);
					fileStream2.Write(span2);
					fileStream2.Write(array2);
					fileStream2.Write(array);
					fileStream2.Flush();
				}
				File.Delete(filePath);
				File.Move(text, filePath);
			}
			catch (Exception)
			{
				if (File.Exists(text))
				{
					File.Delete(text);
				}
				throw;
			}
		}

		public static void SerializeHeader(ReplayHeader header, in string filePath)
		{
			using FileStream fileStream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write);
			byte[] bytes = Encoding.UTF8.GetBytes("RELU");
			fileStream.Write(bytes, 0, bytes.Length);
			byte[] array = MemoryPackSerializer.Serialize(in header);
			byte[] bytes2 = BitConverter.GetBytes(array.Length);
			fileStream.Write(bytes2, 0, bytes2.Length);
			fileStream.Write(array, 0, array.Length);
			fileStream.Flush();
		}

		public static void SerializeHeader(LegacyReplayHeader header, in string filePath)
		{
			using FileStream fileStream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write);
			byte[] array = MemoryPackSerializer.Serialize(in header);
			byte[] bytes = BitConverter.GetBytes(array.Length);
			fileStream.Write(bytes, 0, bytes.Length);
			fileStream.Write(array, 0, array.Length);
			fileStream.Flush();
		}

		public static IMsg DeepCopyViaSerializer(IMsg original)
		{
			byte[] array = MemoryPackSerializer.Serialize(original.GetType(), original);
			return (IMsg)MemoryPackSerializer.Deserialize(original.GetType(), array);
		}

		public static IMsg DeepCopy(IMsg original)
		{
			return DeepCopyViaSerializer(original);
		}

		public static void SerializeMsgToFile(in List<MsgWithTime> msgList, in string filePath)
		{
			List<WrappedMsg> value = new List<WrappedMsg>();
			foreach (MsgWithTime msg2 in msgList)
			{
				IMsg msg = msg2.msg;
				long time = msg2.time;
				string typeNameFromMsg = WrappedMsg.GetTypeNameFromMsg(msg);
				byte[] dataFromMsg = WrappedMsg.GetDataFromMsg(msg);
				value.Add(new WrappedMsg(typeNameFromMsg, time, dataFromMsg));
			}
			lock (_msgFileLock)
			{
				using FileStream fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write);
				using BrotliCompressor bufferWriter = new BrotliCompressor();
				MemoryPackSerializer.Serialize(in bufferWriter, in value);
				byte[] array = bufferWriter.ToArray();
				byte[] bytes = BitConverter.GetBytes(array.Length);
				fileStream.Write(bytes, 0, bytes.Length);
				fileStream.Write(array, 0, array.Length);
				fileStream.Flush();
			}
		}

		public static async UniTask<List<MsgWithTime>> DeserializeMsgFromFileAsync(FileStream fileStream)
		{
			List<MsgWithTime> msgList = new List<MsgWithTime>();
			while (fileStream.Position < fileStream.Length)
			{
				byte[] lengthBuffer = new byte[4];
				await fileStream.ReadAsync(lengthBuffer, 0, lengthBuffer.Length);
				int num = BitConverter.ToInt32(lengthBuffer, 0);
				byte[] dataBuffer = new byte[num];
				await fileStream.ReadAsync(dataBuffer, 0, dataBuffer.Length);
				using BrotliDecompressor brotliDecompressor = default(BrotliDecompressor);
				foreach (WrappedMsg item in MemoryPackSerializer.Deserialize<List<WrappedMsg>>(brotliDecompressor.Decompress(dataBuffer)))
				{
					msgList.Add(new MsgWithTime
					{
						msg = item.GetMessage(),
						time = item.Time
					});
				}
			}
			return msgList;
		}

		public static void SerializeSndToFile(in List<SndWithTime> sndList, in string filePath)
		{
			List<ReplayableSndEvent> value = new List<ReplayableSndEvent>();
			foreach (SndWithTime snd in sndList)
			{
				ProtoActor protoActor = Hub.s.pdata.main?.GetActorByActorID(snd.ActorID);
				if ((object)protoActor == null)
				{
					continue;
				}
				ReplayableSndEvent item;
				if (protoActor.IsPlayer())
				{
					item = new ReplayableSndEvent(SndEventType.PLAYER, snd.ActorID, protoActor.UID, snd.Time, ReplayableSndEvent.GetDataFromSndEvent(snd.SpeechEvent), ReplayableSndEvent.GetAudioDataFromSndEvent(snd.SpeechEvent));
				}
				else
				{
					if (!protoActor.IsMimic())
					{
						continue;
					}
					item = new ReplayableSndEvent(SndEventType.MIMIC, snd.ActorID, -1L, snd.Time, ReplayableSndEvent.GetDataFromSndEvent(snd.SpeechEvent), ReplayableSndEvent.GetAudioDataFromSndEvent(snd.SpeechEvent));
				}
				value.Add(item);
			}
			lock (_sndFileLock)
			{
				using FileStream fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write);
				using BrotliCompressor bufferWriter = new BrotliCompressor();
				MemoryPackSerializer.Serialize(in bufferWriter, in value);
				byte[] array = bufferWriter.ToArray();
				byte[] bytes = BitConverter.GetBytes(array.Length);
				fileStream.Write(bytes, 0, bytes.Length);
				fileStream.Write(array, 0, array.Length);
				fileStream.Flush();
			}
		}

		public static async UniTask<List<SndWithTime>> DeserializeSndFromFileAsync(string filePath)
		{
			using FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
			List<SndWithTime> sndList = new List<SndWithTime>();
			while (fileStream.Position < fileStream.Length)
			{
				byte[] lengthBuffer = new byte[4];
				await fileStream.ReadAsync(lengthBuffer, 0, lengthBuffer.Length);
				int num = BitConverter.ToInt32(lengthBuffer, 0);
				byte[] dataBuffer = new byte[num];
				await fileStream.ReadAsync(dataBuffer, 0, dataBuffer.Length);
				using BrotliDecompressor brotliDecompressor = default(BrotliDecompressor);
				foreach (ReplayableSndEvent item2 in MemoryPackSerializer.Deserialize<List<ReplayableSndEvent>>(brotliDecompressor.Decompress(dataBuffer)))
				{
					SndWithTime item = new SndWithTime
					{
						ActorID = item2.ActorID,
						PlayerUID = item2.PlayerUID,
						Time = item2.Time,
						SpeechEvent = item2.GetSndEvent(),
						CompressedSndData = item2.AudioData
					};
					sndList.Add(item);
				}
			}
			return sndList;
		}
	}
}
