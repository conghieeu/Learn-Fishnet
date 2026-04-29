using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace Relugames.Replay
{
	[DebuggerDisplay("{ToString(),nq}")]
	public sealed class ReplayMetadata : IMessage<ReplayMetadata>, IMessage, IEquatable<ReplayMetadata>, IDeepCloneable<ReplayMetadata>, IBufferMessage
	{
		private static readonly MessageParser<ReplayMetadata> _parser = new MessageParser<ReplayMetadata>(() => new ReplayMetadata());

		private UnknownFieldSet _unknownFields;

		public const int FilenameFieldNumber = 1;

		private string filename_ = "";

		public const int MimeTypeFieldNumber = 2;

		private string mimeType_ = "";

		public const int TotalSizeFieldNumber = 3;

		private ulong totalSize_;

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public static MessageParser<ReplayMetadata> Parser => _parser;

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public static MessageDescriptor Descriptor => ReplayTransferReflection.Descriptor.MessageTypes[1];

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		MessageDescriptor IMessage.Descriptor => Descriptor;

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public string Filename
		{
			get
			{
				return filename_;
			}
			set
			{
				filename_ = ProtoPreconditions.CheckNotNull(value, "value");
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public string MimeType
		{
			get
			{
				return mimeType_;
			}
			set
			{
				mimeType_ = ProtoPreconditions.CheckNotNull(value, "value");
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public ulong TotalSize
		{
			get
			{
				return totalSize_;
			}
			set
			{
				totalSize_ = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public ReplayMetadata()
		{
		}

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public ReplayMetadata(ReplayMetadata other)
			: this()
		{
			filename_ = other.filename_;
			mimeType_ = other.mimeType_;
			totalSize_ = other.totalSize_;
			_unknownFields = UnknownFieldSet.Clone(other._unknownFields);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public ReplayMetadata Clone()
		{
			return new ReplayMetadata(this);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public override bool Equals(object other)
		{
			return Equals(other as ReplayMetadata);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public bool Equals(ReplayMetadata other)
		{
			if (other == null)
			{
				return false;
			}
			if (other == this)
			{
				return true;
			}
			if (Filename != other.Filename)
			{
				return false;
			}
			if (MimeType != other.MimeType)
			{
				return false;
			}
			if (TotalSize != other.TotalSize)
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
			if (Filename.Length != 0)
			{
				num ^= Filename.GetHashCode();
			}
			if (MimeType.Length != 0)
			{
				num ^= MimeType.GetHashCode();
			}
			if (TotalSize != 0L)
			{
				num ^= TotalSize.GetHashCode();
			}
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
			if (Filename.Length != 0)
			{
				output.WriteRawTag(10);
				output.WriteString(Filename);
			}
			if (MimeType.Length != 0)
			{
				output.WriteRawTag(18);
				output.WriteString(MimeType);
			}
			if (TotalSize != 0L)
			{
				output.WriteRawTag(24);
				output.WriteUInt64(TotalSize);
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
			if (Filename.Length != 0)
			{
				num += 1 + CodedOutputStream.ComputeStringSize(Filename);
			}
			if (MimeType.Length != 0)
			{
				num += 1 + CodedOutputStream.ComputeStringSize(MimeType);
			}
			if (TotalSize != 0L)
			{
				num += 1 + CodedOutputStream.ComputeUInt64Size(TotalSize);
			}
			if (_unknownFields != null)
			{
				num += _unknownFields.CalculateSize();
			}
			return num;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public void MergeFrom(ReplayMetadata other)
		{
			if (other != null)
			{
				if (other.Filename.Length != 0)
				{
					Filename = other.Filename;
				}
				if (other.MimeType.Length != 0)
				{
					MimeType = other.MimeType;
				}
				if (other.TotalSize != 0L)
				{
					TotalSize = other.TotalSize;
				}
				_unknownFields = UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
			}
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
					Filename = input.ReadString();
					break;
				case 18u:
					MimeType = input.ReadString();
					break;
				case 24u:
					TotalSize = input.ReadUInt64();
					break;
				}
			}
		}
	}
}
