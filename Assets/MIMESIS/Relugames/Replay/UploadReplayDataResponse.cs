using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace Relugames.Replay
{
	[DebuggerDisplay("{ToString(),nq}")]
	public sealed class UploadReplayDataResponse : IMessage<UploadReplayDataResponse>, IMessage, IEquatable<UploadReplayDataResponse>, IDeepCloneable<UploadReplayDataResponse>, IBufferMessage
	{
		private static readonly MessageParser<UploadReplayDataResponse> _parser = new MessageParser<UploadReplayDataResponse>(() => new UploadReplayDataResponse());

		private UnknownFieldSet _unknownFields;

		public const int StatusFieldNumber = 1;

		private string status_ = "";

		public const int MessageFieldNumber = 2;

		private string message_ = "";

		public const int FileUrlFieldNumber = 3;

		private string fileUrl_ = "";

		public const int BytesReceivedFieldNumber = 4;

		private ulong bytesReceived_;

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public static MessageParser<UploadReplayDataResponse> Parser => _parser;

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public static MessageDescriptor Descriptor => ReplayTransferReflection.Descriptor.MessageTypes[2];

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		MessageDescriptor IMessage.Descriptor => Descriptor;

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public string Status
		{
			get
			{
				return status_;
			}
			set
			{
				status_ = ProtoPreconditions.CheckNotNull(value, "value");
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public string Message
		{
			get
			{
				return message_;
			}
			set
			{
				message_ = ProtoPreconditions.CheckNotNull(value, "value");
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public string FileUrl
		{
			get
			{
				return fileUrl_;
			}
			set
			{
				fileUrl_ = ProtoPreconditions.CheckNotNull(value, "value");
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public ulong BytesReceived
		{
			get
			{
				return bytesReceived_;
			}
			set
			{
				bytesReceived_ = value;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public UploadReplayDataResponse()
		{
		}

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public UploadReplayDataResponse(UploadReplayDataResponse other)
			: this()
		{
			status_ = other.status_;
			message_ = other.message_;
			fileUrl_ = other.fileUrl_;
			bytesReceived_ = other.bytesReceived_;
			_unknownFields = UnknownFieldSet.Clone(other._unknownFields);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public UploadReplayDataResponse Clone()
		{
			return new UploadReplayDataResponse(this);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public override bool Equals(object other)
		{
			return Equals(other as UploadReplayDataResponse);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public bool Equals(UploadReplayDataResponse other)
		{
			if (other == null)
			{
				return false;
			}
			if (other == this)
			{
				return true;
			}
			if (Status != other.Status)
			{
				return false;
			}
			if (Message != other.Message)
			{
				return false;
			}
			if (FileUrl != other.FileUrl)
			{
				return false;
			}
			if (BytesReceived != other.BytesReceived)
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
			if (Status.Length != 0)
			{
				num ^= Status.GetHashCode();
			}
			if (Message.Length != 0)
			{
				num ^= Message.GetHashCode();
			}
			if (FileUrl.Length != 0)
			{
				num ^= FileUrl.GetHashCode();
			}
			if (BytesReceived != 0L)
			{
				num ^= BytesReceived.GetHashCode();
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
			if (Status.Length != 0)
			{
				output.WriteRawTag(10);
				output.WriteString(Status);
			}
			if (Message.Length != 0)
			{
				output.WriteRawTag(18);
				output.WriteString(Message);
			}
			if (FileUrl.Length != 0)
			{
				output.WriteRawTag(26);
				output.WriteString(FileUrl);
			}
			if (BytesReceived != 0L)
			{
				output.WriteRawTag(32);
				output.WriteUInt64(BytesReceived);
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
			if (Status.Length != 0)
			{
				num += 1 + CodedOutputStream.ComputeStringSize(Status);
			}
			if (Message.Length != 0)
			{
				num += 1 + CodedOutputStream.ComputeStringSize(Message);
			}
			if (FileUrl.Length != 0)
			{
				num += 1 + CodedOutputStream.ComputeStringSize(FileUrl);
			}
			if (BytesReceived != 0L)
			{
				num += 1 + CodedOutputStream.ComputeUInt64Size(BytesReceived);
			}
			if (_unknownFields != null)
			{
				num += _unknownFields.CalculateSize();
			}
			return num;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("protoc", null)]
		public void MergeFrom(UploadReplayDataResponse other)
		{
			if (other != null)
			{
				if (other.Status.Length != 0)
				{
					Status = other.Status;
				}
				if (other.Message.Length != 0)
				{
					Message = other.Message;
				}
				if (other.FileUrl.Length != 0)
				{
					FileUrl = other.FileUrl;
				}
				if (other.BytesReceived != 0L)
				{
					BytesReceived = other.BytesReceived;
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
					Status = input.ReadString();
					break;
				case 18u:
					Message = input.ReadString();
					break;
				case 26u:
					FileUrl = input.ReadString();
					break;
				case 32u:
					BytesReceived = input.ReadUInt64();
					break;
				}
			}
		}
	}
}
