using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace Relugames.Replay
{
	[DebuggerDisplay("{ToString(),nq}")]
	public sealed class UploadReplayDataRequest : IMessage<UploadReplayDataRequest>, IMessage, IEquatable<UploadReplayDataRequest>, IDeepCloneable<UploadReplayDataRequest>, IBufferMessage
	{
		public enum DataOneofCase
		{
			None = 0,
			Metadata = 1,
			Chunk = 2
		}

		private static readonly MessageParser<UploadReplayDataRequest> _parser = new MessageParser<UploadReplayDataRequest>(() => new UploadReplayDataRequest());

		private UnknownFieldSet _unknownFields;

		public const int MetadataFieldNumber = 1;

		public const int ChunkFieldNumber = 2;

		private object data_;

		private DataOneofCase dataCase_;

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public static MessageParser<UploadReplayDataRequest> Parser => _parser;

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public static MessageDescriptor Descriptor => ReplayTransferReflection.Descriptor.MessageTypes[0];

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		MessageDescriptor IMessage.Descriptor => Descriptor;

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public ReplayMetadata Metadata
		{
			get
			{
				if (dataCase_ != DataOneofCase.Metadata)
				{
					return null;
				}
				return (ReplayMetadata)data_;
			}
			set
			{
				data_ = value;
				dataCase_ = ((value != null) ? DataOneofCase.Metadata : DataOneofCase.None);
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public ByteString Chunk
		{
			get
			{
				if (!HasChunk)
				{
					return ByteString.Empty;
				}
				return (ByteString)data_;
			}
			set
			{
				data_ = ProtoPreconditions.CheckNotNull(value, "value");
				dataCase_ = DataOneofCase.Chunk;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public bool HasChunk => dataCase_ == DataOneofCase.Chunk;

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public DataOneofCase DataCase => dataCase_;

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public UploadReplayDataRequest()
		{
		}

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public UploadReplayDataRequest(UploadReplayDataRequest other)
			: this()
		{
			switch (other.DataCase)
			{
			case DataOneofCase.Metadata:
				Metadata = other.Metadata.Clone();
				break;
			case DataOneofCase.Chunk:
				Chunk = other.Chunk;
				break;
			}
			_unknownFields = UnknownFieldSet.Clone(other._unknownFields);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public UploadReplayDataRequest Clone()
		{
			return new UploadReplayDataRequest(this);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public void ClearChunk()
		{
			if (HasChunk)
			{
				ClearData();
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public void ClearData()
		{
			dataCase_ = DataOneofCase.None;
			data_ = null;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public override bool Equals(object other)
		{
			return Equals(other as UploadReplayDataRequest);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public bool Equals(UploadReplayDataRequest other)
		{
			if (other == null)
			{
				return false;
			}
			if (other == this)
			{
				return true;
			}
			if (!object.Equals(Metadata, other.Metadata))
			{
				return false;
			}
			if (Chunk != other.Chunk)
			{
				return false;
			}
			if (DataCase != other.DataCase)
			{
				return false;
			}
			return object.Equals(_unknownFields, other._unknownFields);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public override int GetHashCode()
		{
			int num = 1;
			if (dataCase_ == DataOneofCase.Metadata)
			{
				num ^= Metadata.GetHashCode();
			}
			if (HasChunk)
			{
				num ^= Chunk.GetHashCode();
			}
			num ^= (int)dataCase_;
			if (_unknownFields != null)
			{
				num ^= _unknownFields.GetHashCode();
			}
			return num;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public override string ToString()
		{
			return JsonFormatter.ToDiagnosticString(this);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public void WriteTo(CodedOutputStream output)
		{
			output.WriteRawMessage(this);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		void IBufferMessage.InternalWriteTo(ref WriteContext output)
		{
			if (dataCase_ == DataOneofCase.Metadata)
			{
				output.WriteRawTag(10);
				output.WriteMessage(Metadata);
			}
			if (HasChunk)
			{
				output.WriteRawTag(18);
				output.WriteBytes(Chunk);
			}
			if (_unknownFields != null)
			{
				_unknownFields.WriteTo(ref output);
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public int CalculateSize()
		{
			int num = 0;
			if (dataCase_ == DataOneofCase.Metadata)
			{
				num += 1 + CodedOutputStream.ComputeMessageSize(Metadata);
			}
			if (HasChunk)
			{
				num += 1 + CodedOutputStream.ComputeBytesSize(Chunk);
			}
			if (_unknownFields != null)
			{
				num += _unknownFields.CalculateSize();
			}
			return num;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public void MergeFrom(UploadReplayDataRequest other)
		{
			if (other == null)
			{
				return;
			}
			switch (other.DataCase)
			{
			case DataOneofCase.Metadata:
				if (Metadata == null)
				{
					Metadata = new ReplayMetadata();
				}
				Metadata.MergeFrom(other.Metadata);
				break;
			case DataOneofCase.Chunk:
				Chunk = other.Chunk;
				break;
			}
			_unknownFields = UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public void MergeFrom(CodedInputStream input)
		{
			input.ReadRawMessage(this);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		void IBufferMessage.InternalMergeFrom(ref ParseContext input)
		{
			uint num;
			while ((num = input.ReadTag()) != 0 && (num & 7) != 4)
			{
				switch (num)
				{
				default:
					_unknownFields = UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
					break;
				case 10u:
				{
					ReplayMetadata replayMetadata = new ReplayMetadata();
					if (dataCase_ == DataOneofCase.Metadata)
					{
						replayMetadata.MergeFrom(Metadata);
					}
					input.ReadMessage(replayMetadata);
					Metadata = replayMetadata;
					break;
				}
				case 18u:
					Chunk = input.ReadBytes();
					break;
				}
			}
		}
	}
}
