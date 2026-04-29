using System;
using Google.Protobuf.Reflection;

namespace Relugames.Replay
{
	public static class ReplayTransferReflection
	{
		private static FileDescriptor descriptor;

		public static FileDescriptor Descriptor => descriptor;

		static ReplayTransferReflection()
		{
			descriptor = FileDescriptor.FromGeneratedCode(Convert.FromBase64String("Cj1NTV9TdG9yYWdlU2VydmVyL01NX1N0b3JhZ2VTZXJ2ZXIvUHJvdG9zL1Jl" + "cGxheVRyYW5zZmVyLnByb3RvIlcKF1VwbG9hZFJlcGxheURhdGFSZXF1ZXN0" + "EiMKCG1ldGFkYXRhGAEgASgLMg8uUmVwbGF5TWV0YWRhdGFIABIPCgVjaHVu" + "axgCIAEoDEgAQgYKBGRhdGEiSQoOUmVwbGF5TWV0YWRhdGESEAoIZmlsZW5h" + "bWUYASABKAkSEQoJbWltZV90eXBlGAIgASgJEhIKCnRvdGFsX3NpemUYAyAB" + "KAQiZQoYVXBsb2FkUmVwbGF5RGF0YVJlc3BvbnNlEg4KBnN0YXR1cxgBIAEo" + "CRIPCgdtZXNzYWdlGAIgASgJEhAKCGZpbGVfdXJsGAMgASgJEhYKDmJ5dGVz" + "X3JlY2VpdmVkGAQgASgEMlsKDlJlcGxheVRyYW5zZmVyEkkKEFVwbG9hZFJl" + "cGxheURhdGESGC5VcGxvYWRSZXBsYXlEYXRhUmVxdWVzdBoZLlVwbG9hZFJl" + "cGxheURhdGFSZXNwb25zZSgBQhOqAhBSZWx1Z2FtZXMuUmVwbGF5YgZwcm90" + "bzM="), new FileDescriptor[0], new GeneratedClrTypeInfo(null, null, new GeneratedClrTypeInfo[3]
			{
				new GeneratedClrTypeInfo(typeof(UploadReplayDataRequest), UploadReplayDataRequest.Parser, new string[2] { "Metadata", "Chunk" }, new string[1] { "Data" }, null, null, null),
				new GeneratedClrTypeInfo(typeof(ReplayMetadata), ReplayMetadata.Parser, new string[3] { "Filename", "MimeType", "TotalSize" }, null, null, null, null),
				new GeneratedClrTypeInfo(typeof(UploadReplayDataResponse), UploadReplayDataResponse.Parser, new string[4] { "Status", "Message", "FileUrl", "BytesReceived" }, null, null, null, null)
			}));
		}
	}
}
